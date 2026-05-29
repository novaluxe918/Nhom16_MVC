using Microsoft.EntityFrameworkCore;
using Nhom16_MVC.Data;
using Nhom16_MVC.Libraries;
using Nhom16_MVC.Models.DTOs;
using Nhom16_MVC.Models.Entities;
using Nhom16_MVC.Models.Enums;

namespace Nhom16_MVC.Services
{
    public class GiaoDichService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration; // Đọc file appsettings.json
        private readonly IHttpContextAccessor _httpContextAccessor;
        public GiaoDichService(AppDbContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        // rút tiền
        public async Task<searchSuggestResponse> TaoYeuCauRutTienAsync(int maNguoiDung, RutTienRequestDto request)
        {
            var response = new searchSuggestResponse { Success = false };
            if (request.SoTien < 50000) { response.Message = "Số tiền rút tối thiểu là 50.000đ"; return response; }

            var nguoiDung = await _context.nguoidung.FirstOrDefaultAsync(u => u.manguoidung == maNguoiDung);
            if (nguoiDung == null) { response.Message = "Người dùng không tồn tại."; return response; }

            if (nguoiDung.sodutaikhoan < request.SoTien)
            {
                response.Message = $"Số dư ví không đủ. Bạn chỉ có tối đa {nguoiDung.sodutaikhoan:N0}đ.";
                return response;
            }

            //tạo mã giao dịch 
            string maGd = $"RUT-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}";

            //tạo bản ghi cho lệnh rút tiền 
            var yeuCau = new yeucauruttien
            {
                manguoidung = maNguoiDung,
                sotien = request.SoTien,
                tennganhang = request.TenNganHang,
                sotaikhoan = request.SoTaiKhoan,
                mota = request.MoTa ?? "Rút tiền về tài khoản",
                magiaodich = maGd,
                trangthai = TrangThaiRutEnum.ChoXuLy // Nằm chờ Admin duyệt
            };

            await _context.yeucauruttien.AddAsync(yeuCau);
            await _context.SaveChangesAsync();

            response.Success = true;
            response.Message = "Tạo lệnh rút tiền thành công. Vui lòng chờ Admin phê duyệt.";
            return response;
        }

        //Nạp tiền
        //Hàm tạo url thanh toán 
        public async Task<string> TaoUrlNapTienVnPayAsync(int maNguoiDung, long soTien)
        {
            string maGd = $"NAP-{DateTime.Now.Ticks}";

            //Luu xuống db _cho xu ly
            var napTienRecord = new naptien
            {
                nguoinap = maNguoiDung,
                sotien = soTien,
                magiaodich = maGd,
                phuongthuc = PhuongThucNapEnum.VNPay,
                trangthai = TrangThaiNapEnum.ChoXuLy
            };
            await _context.naptien.AddAsync(napTienRecord);
            await _context.SaveChangesAsync();

            string vnp_TmnCode = _configuration["VnPay:TmnCode"]!;
            string vnp_HashSecret = _configuration["VnPay:HashSecret"]!;
            string vnp_Url = _configuration["VnPay:BaseUrl"]!;
            string vnp_Returnurl = _configuration["VnPay:ReturnUrl"]!;

            //thông tin build gửi về vnpay
            var pay = new VnPayLibrary();
            pay.AddRequestData("vnp_Version", "2.1.0");
            pay.AddRequestData("vnp_Command", "pay");
            pay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            pay.AddRequestData("vnp_Amount", (soTien * 100).ToString()); // VNPAY yêu cầu nhân 100
            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", "VND");
            pay.AddRequestData("vnp_IpAddr", _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "127.0.0.1");
            pay.AddRequestData("vnp_Locale", "vn");
            pay.AddRequestData("vnp_OrderInfo", $"Nap tien vao vi Sportsync. Ma GD: {maGd}");
            pay.AddRequestData("vnp_OrderType", "other"); // Loại hàng hóa
            pay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            pay.AddRequestData("vnp_TxnRef", maGd); // Mã tham chiếu (khớp với DB của bạn)

            string paymentUrl = pay.CreateRequestUrl(vnp_Url, vnp_HashSecret);

            return paymentUrl;
        }

        //hàm xử lý kq khi vnpay trả về
        public async Task<string> XuLyVnPayReturnAsync(IQueryCollection collections)
        {
            var pay = new VnPayLibrary();

            // Đổ toàn bộ dữ liệu VNPAY trả về vào thư viện để kiểm tra
            foreach (var (key, value) in collections)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    pay.AddResponseData(key, value.ToString());
                }
            }
            string maGd = pay.GetResponseData("vnp_TxnRef");
            string vnp_ResponseCode = pay.GetResponseData("vnp_ResponseCode");
            string vnp_SecureHash = collections["vnp_SecureHash"].ToString();
            string hashSecret = _configuration["VnPay:HashSecret"]!;

            //kiểm tra chữ ký bảo mật 
            bool checkSignature = pay.ValidateSignature(vnp_SecureHash, hashSecret);
            if (!checkSignature)
            {
                return "Lỗi bảo mật: Chữ ký không hợp lệ!";
            }

            //tìm đơn nạp tiền cho db
            var donNapTien = await _context.naptien.FirstOrDefaultAsync(n => n.magiaodich == maGd);
            if (donNapTien == null) return "Không tìm thấy giao dịch này trên hệ thống.";
            if (donNapTien.trangthai != TrangThaiNapEnum.ChoXuLy) return "Giao dịch này đã được xử lý rồi.";


            if (vnp_ResponseCode == "00")
            {
                // Thành công
                donNapTien.trangthai = TrangThaiNapEnum.ThanhCong;
                // Nhờ vào Trigger "trg_NapTien_CapNhatSoDu" trong PostgreSQL, khi bạn SaveChanges(), DB sẽ tự cộng tiền.
            }
            else
            {
                donNapTien.trangthai = TrangThaiNapEnum.ThatBai;
            }

            await _context.SaveChangesAsync();

            return vnp_ResponseCode == "00" ? "Nạp tiền thành công!" : "Nạp tiền thất bại hoặc bị hủy.";

        }
    }
}
