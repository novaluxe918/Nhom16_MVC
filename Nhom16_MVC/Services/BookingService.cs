using Nhom16_MVC.Data;
using Nhom16_MVC.Models.DTOs;
using Nhom16_MVC.Models.Entities;
using Nhom16_MVC.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Nhom16_MVC.Services
{
    public class BookingService
    {
        private readonly AppDbContext _context;

        public BookingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<searchSuggestResponse> XuLyDatSanTransactionAsync(int manguoiThue,int maSanChiTiet ,DatSanYeuCauDto request)
        {
            var response = new searchSuggestResponse { Success = false };

            //Xác thực thông tin thực thể người dùng và sân con có tồn tại không
            var nguoiDung = await _context.nguoidung.FirstOrDefaultAsync(u => u.manguoidung == manguoiThue);


            var sanCon = await _context.sanbongchitiet.FirstOrDefaultAsync(sc => sc.masanchitiet == maSanChiTiet);

            if (nguoiDung == null) { response.Message = "Tài khoản người dùng không hợp lệ."; return response; }
            if (sanCon == null) { response.Message = "Mã sân con không tồn tại trên hệ thống."; return response; }
            // Tính toán tổng chi phí dựa trên quy tắc phân tách khung giờ sáng/tối
            long tongTienLichDat = 0;
            var danhSachChiTietDat = new List<chitietdatsan>();

            foreach (var slot in request.DanhSachSlotDat)
            {
                var datePart = DateOnly.ParseExact(slot.Ngay, "yyyy-MM-dd");
                var timePart = TimeOnly.ParseExact(slot.GioBatDau, "HH:mm");

                DateTime dtBatDau = datePart.ToDateTime(timePart);
                DateTime dtKetThuc = dtBatDau.AddHours(1);

                // Từ 5h sáng đến trước 18h tính giá buổi sáng
                long giaSlotNay = (dtBatDau.Hour >= 5 && dtBatDau.Hour < 18)
                    ? sanCon.giathuebuoisang
                    : sanCon.giathuebuoitoi;

                tongTienLichDat += giaSlotNay;

                // Tạo đối tượng thực thể chuẩn bị ghi dữ liệu
                danhSachChiTietDat.Add(new chitietdatsan
                {
                    masanchitiet = maSanChiTiet,
                    giobatdau = dtBatDau,
                    gioketthuc = dtKetThuc,
                    trangthaidatsan = TrangThaiDatEnum.ChoXacNhan, // Trạng thái ban đầu theo máy trạng thái
                    covande = false
                });
            }

            //  Kiểm tra điều kiện số dư ví tài khoản điện tử
            if (nguoiDung.sodutaikhoan < tongTienLichDat)
            {
                response.Message = $"Số dư tài khoản không đủ. Cần có: {tongTienLichDat:N0}đ, Hiện tại có: {nguoiDung.sodutaikhoan:N0}đ. Vui lòng nạp thêm tiền.";
                return response;
            }

            // KÍCH HOẠT VÙNG TRANSACTION AN TOÀN TUYỆT ĐỐI (ACID)
            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                //Thao tác 1: Trừ tiền trực tiếp vào tài khoản người thuê
                nguoiDung.sodutaikhoan -= tongTienLichDat;
                _context.nguoidung.Update(nguoiDung);

                // Tạo hóa đơn tổng 
                var hoaDonTong = new datsan
                {
                    nguoithue = manguoiThue,
                    ngaydat = DateOnly.FromDateTime(DateTime.Now),
                    ngaythanhtoan = DateTime.Now,
                    sotienthanhtoan = tongTienLichDat
                };
                await _context.datsan.AddAsync(hoaDonTong);
                await _context.SaveChangesAsync(); 

                //  Đồng bộ liên kết mã hóa đơn vào danh sách chi tiết và lưu hàng loạt
                foreach (var chiTiet in danhSachChiTietDat)
                {
                    chiTiet.madatsan = hoaDonTong.madatsan;
                    await _context.chitietdatsan.AddAsync(chiTiet);
                }

                await _context.SaveChangesAsync(); 

                //  xác nhận đóng băng giao dịch thành công
                await dbTransaction.CommitAsync();

                response.Success = true;
                response.Message = "Đặt sân thành công! Tài khoản của bạn đã được trừ tiền giữ chỗ.";
                return response;
            }
            catch (DbUpdateException ex)
            {
                // Thực hiện hủy bỏ toàn bộ thao tác, hoàn trả lại tiền cho người dùng lập tức nếu lỗi dữ liệu
                await dbTransaction.RollbackAsync();

                // Bắt mã ngoại lệ loại trừ trùng lịch 
                if (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23P01")
                {
                    response.Message = "Slot giờ bạn chọn vừa có người khác đặt mất trước một tích tắc. Vui lòng tải lại trang và chọn khung giờ khác.";
                }
                else
                {
                    response.Message = $"Lỗi xử lý hệ thống cơ sở dữ liệu: {ex.InnerException?.Message ?? ex.Message}";
                }
                return response;
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                response.Message = $"Hệ thống gặp sự cố ngoài ý muốn: {ex.Message}";
                return response;
            }
        }

        //   hủy đặt sân 
        public async Task<searchSuggestResponse> HuyDatSanVaHoanTienAsync(int maNguoiDung,int maChiTietDatSan)
        {
            var response = new searchSuggestResponse { Success = false };
            //tìm tt hóa đơn đã đặt và giá 
            var chiTiet = await _context.chitietdatsan
                .Include(c=>c.madatsanNavigation)
                .Include(c=>c.masanchitietNavigation)
                .FirstOrDefaultAsync(c=>c.machitietdatsan == maChiTietDatSan);

            if (chiTiet == null) { response.Message = "Không tìm thấy thông tin đơn đặt sân."; return response; }

            if (chiTiet.madatsanNavigation.nguoithue != maNguoiDung) { response.Message = "Bạn không có quyền hủy đơn của người khác."; return response; }

            if (chiTiet.trangthaidatsan == TrangThaiDatEnum.DaHuy) { response.Message = "Đơn này đã được hủy trước đó."; return response; }

            TimeSpan thoiGianConLai = chiTiet.giobatdau - DateTime.Now;
            if (thoiGianConLai.TotalHours < 2)
            {
                response.Message = "Chỉ được phép hủy sân trước thời gian đá ít nhất 2 tiếng.";
                return response;
            }

            long soTienHoan = (chiTiet.giobatdau.Hour >= 5 && chiTiet.giobatdau.Hour < 18)
                ? chiTiet.masanchitietNavigation.giathuebuoisang
                : chiTiet.masanchitietNavigation.giathuebuoitoi;

            //kích hoạt transaction 
            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                //chuyển đổi sang trạng thái đã hủy 
                //chiTiet.trangthaidatsan = TrangThaiDatEnum.DaHuy;
                //_context.chitietdatsan.Update(chiTiet);


                await _context.Database.ExecuteSqlRawAsync(
                    "UPDATE chitietdatsan SET trangthaidatsan = 'da_huy'::trang_thai_dat WHERE machitietdatsan = {0}",
                    maChiTietDatSan);

                //hoàn tiền 
                
                var nguoiDung = await _context.nguoidung.FirstOrDefaultAsync(u => u.manguoidung == maNguoiDung);

                nguoiDung.sodutaikhoan += soTienHoan;
                //_context.nguoidung.Update(nguoiDung);

                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                response.Success = true;
                response.Message = $"Hủy sân thành công. Bạn đã được hoàn lại {soTienHoan:N0}đ vào ví.";
                return response;
            }catch(Exception ex)
            {
                await dbTransaction.RollbackAsync();

                
                Exception rootCause = ex;
                while (rootCause.InnerException != null)
                {
                    rootCause = rootCause.InnerException;
                }

                response.Message = $"Lỗi DB: {rootCause.Message}";
                return response;
            }
        }

        //Lấy lịch sử đặt sân
        public async Task<List<LichSuDatSanDto>> GetLichSuDatSanAsync(int userId)
        {
            var lichSu = await _context.datsan
                .Where(ds => ds.nguoithue == userId)
                .Select(ds => new LichSuDatSanDto
                {
                    MaDatSan = ds.madatsan,
                    NgayDat = ds.ngaydat.ToString("dd/MM/yyyy"),
                    SoTienThanhToan = ds.sotienthanhtoan,
                    MaSanChiTiet = ds.chitietdatsan.FirstOrDefault().masanchitiet,

                    // Lấy giờ từ chi tiết đặt sân
                    GioBatDau = ds.chitietdatsan.FirstOrDefault().giobatdau.ToString("HH:mm"),
                    GioKetThuc = ds.chitietdatsan.FirstOrDefault().gioketthuc.ToString("HH:mm"),

                    
                    TrangThai = ds.chitietdatsan.FirstOrDefault().trangthaidatsan.ToString(),

                    
                    TenSanChiTiet = ds.chitietdatsan.FirstOrDefault().masanchitietNavigation.tensanchitiet,


                    HinhAnhSan = ds.chitietdatsan.FirstOrDefault()
                                    .masanchitietNavigation
                                    .media_sanbongchitiet
                                    .Where(m => m.loaimedia == "hinh_anh") 
                                    .Select(m => m.mediaid)               
                                    .FirstOrDefault(),


                    DiaChi = ds.chitietdatsan.FirstOrDefault()
                                .masanchitietNavigation
                                .masanbongNavigation.thanhpho ?? "Đà Nẵng"
                })
                .OrderByDescending(x => x.MaDatSan)
                .ToListAsync();

            return lichSu;
        }

    }
}