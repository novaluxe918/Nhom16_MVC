using Nhom16_MVC.Data;
using Nhom16_MVC.Models.DTOs;
using Nhom16_MVC.Models.Entities;
using Nhom16_MVC.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Nhom16_MVC.Services;

/// Service xử lý logic tìm kiếm sân trống theo khung giờ

public class AvailableFieldService
{
    private readonly AppDbContext _context;

    public AvailableFieldService(AppDbContext context)
    {
        _context = context;
    }


    /// Tìm kiếm sân trống theo khung giờ
 
    public async Task<SearchAvailableFieldsResponse> SearchAvailableFieldsAsync(SearchAvailableFieldsRequest request)
    {
        var response = new SearchAvailableFieldsResponse();

        //Validate input
        var validationResult = ValidateInput(request);
        if (!validationResult.IsValid)
        {
            response.Success = false;
            response.Message = validationResult.ErrorMessage;
            return response;
        }

        try
        {
            // Parse input
            var ngay = DateOnly.ParseExact(request.Ngay, "yyyy-MM-dd");
            var gioTu = TimeOnly.ParseExact(request.GioTu, "HH:mm");
            var gioDen = TimeOnly.ParseExact(request.GioDen, "HH:mm");

            //Tạo danh sách slot 1 tiếng trong khung giờ
            var slots = GenerateSlots(gioTu, gioDen);
            if (slots.Count == 0)
            {
                response.Success = false;
                response.Message = "Không thể tạo slot trong khung giờ này";
                return response;
            }

            // Lấy tất cả sân bóng với thông tin đánh giá

            //  TẠO KHUNG LEGO CƠ BẢN: Lấy các sân đã duyệt
            var query = _context.sanbong
                .Include(s => s.sanbongchitiet)
                    .ThenInclude(sc => sc.maloaisanNavigation)
                .Include(s => s.sanbongchitiet)
                    .ThenInclude(sc => sc.danhgia)
                .Where(s => s.daduyet == true)
                .AsQueryable(); 

            // LẮP RÁP CÁC ĐIỀU KIỆN LỌC 

            // - Nếu người dùng có chọn lọc theo Tên Sân 
            if (!string.IsNullOrWhiteSpace(request.TenSan))
            {
                var keyword = request.TenSan.Trim().ToLower();
                query = query.Where(s => s.tensan.ToLower().Contains(keyword));
            }

            // - Nếu người dùng có chọn Quận từ Combobox
            if (!string.IsNullOrWhiteSpace(request.Quan))
            {
                query = query.Where(s => s.quan == request.Quan);
            }

            // - Nếu người dùng có chọn Loại Sân
            if (request.MaLoaiSan.HasValue && request.MaLoaiSan.Value > 0)
            {
                
                query = query.Where(s => s.sanbongchitiet.Any(sc => sc.maloaisan == request.MaLoaiSan.Value));
            }

            
            var sanbongs = await query.ToListAsync();


            if (sanbongs.Count == 0)
            {
                response.Success = true;
                response.Message = "Không tìm thấy sân bóng nào";
                response.Data = new List<AvailableFieldDetailDto>();
                response.TotalCount = 0;
                return response;
            }

            //Với mỗi sân chính, tìm các sân con có slot trống
 
            foreach (var sanbong in sanbongs)
            {
                foreach (var sanChiTiet in sanbong.sanbongchitiet)
                {

                    if (request.MaLoaiSan.HasValue && request.MaLoaiSan.Value > 0 && sanChiTiet.maloaisan != request.MaLoaiSan.Value)
                    {
                        continue;
                    }
                    var danhgias = sanChiTiet.danhgia.ToList();
                    decimal soSaoDanhGia = danhgias.Count > 0
                        ? Math.Round((decimal)danhgias.Average(d => (double)d.diemso), 1)
                        : 0;

                    // Lấy tất cả booking của sân con trong ngày này
                    var batDauNgay = ngay.ToDateTime(TimeOnly.MinValue);   
                    var ketThucNgay = batDauNgay.AddDays(1);               

                    var bookingsInDay = await _context.chitietdatsan
                        .Where(c => c.masanchitiet == sanChiTiet.masanchitiet &&
                                   c.giobatdau >= batDauNgay &&
                                   c.giobatdau < ketThucNgay &&
                                   c.trangthaidatsan != TrangThaiDatEnum.DaHuy)
                        .ToListAsync();

                    // Tìm slot trống
                    var emptySlots = new List<AvailableSlotDto>();
                    foreach (var slot in slots)
                    {
                        if (IsSlotAvailable(slot, bookingsInDay,ngay))
                        {
                            emptySlots.Add(slot);
                        }
                    }

                    // Nếu sân con có ít nhất 1 slot trống, thêm vào danh sách flat
                    if (emptySlots.Count > 0)
                    {
                        var giaTrungBinh = (sanChiTiet.giathuebuoisang + sanChiTiet.giathuebuoitoi) / 2;

                        response.Data.Add(new AvailableFieldDetailDto
                        {
                            // Thông tin sân con
                            MaSanChiTiet = sanChiTiet.masanchitiet,
                            TenSanChiTiet = sanChiTiet.tensanchitiet,
                            LoaiSan = sanChiTiet.maloaisanNavigation?.tenloaisan,
                            GiaThuebuoiSang = sanChiTiet.giathuebuoisang,
                            GiaThuebuoiToi = sanChiTiet.giathuebuoitoi,
                            SlotsTrong = emptySlots,

                            // Thông tin sân mẹ (gắn kèm)
                            MaSanBong = sanbong.masanbong,
                            TenSan = sanbong.tensan,
                            HinhAnh = sanbong.hinhanh,
                            DiaChi = sanbong.diachi,
                            Quan = sanbong.quan,
                            Huyen = sanbong.huyen,
                            Xa = sanbong.xa,
                            ThanhPho = sanbong.thanhpho,
                            KinhDo = sanbong.kinhdo,
                            ViDo = sanbong.vido,
                            MoTa = sanbong.mota,
                            SoSaoDanhGia = Math.Round(soSaoDanhGia, 1),
                            SoLuotDanhGia = danhgias.Count
                        });
                    }
                }
            }

            response.Success = true;
            response.TotalCount = response.Data.Count;
            response.Message = response.Data.Count > 0 
                ? $"Tìm thấy {response.Data.Count} sân con có slot trống" 
                : "Không tìm thấy sân nào có slot trống trong khung giờ này";

            return response;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Lỗi khi tìm kiếm: {ex.Message}";
            return response;
        }
    }

    private ValidationResult ValidateInput(SearchAvailableFieldsRequest request)
    {
        // Kiểm tra input không null
        if (string.IsNullOrWhiteSpace(request.Ngay) || 
            string.IsNullOrWhiteSpace(request.GioTu) || 
            string.IsNullOrWhiteSpace(request.GioDen))
        {
            return new ValidationResult 
            { 
                IsValid = false, 
                ErrorMessage = "Vui lòng nhập đầy đủ thông tin (ngày, giờ từ, giờ đến)" 
            };
        }

        // Parse ngày
        if (!DateOnly.TryParseExact(request.Ngay, "yyyy-MM-dd", out var ngay))
        {
            return new ValidationResult 
            { 
                IsValid = false, 
                ErrorMessage = "Định dạng ngày không hợp lệ (sử dụng yyyy-MM-dd)" 
            };
        }

        // Parse giờ từ
        if (!TimeOnly.TryParseExact(request.GioTu, "HH:mm", out var gioTu))
        {
            return new ValidationResult 
            { 
                IsValid = false, 
                ErrorMessage = "Định dạng giờ từ không hợp lệ (sử dụng HH:mm)" 
            };
        }

        // Parse giờ đến
        if (!TimeOnly.TryParseExact(request.GioDen, "HH:mm", out var gioDen))
        {
            return new ValidationResult 
            { 
                IsValid = false, 
                ErrorMessage = "Định dạng giờ đến không hợp lệ (sử dụng HH:mm)" 
            };
        }

        // Kiểm tra giờ đến phải sau giờ từ
        if (gioDen <= gioTu)
        {
            return new ValidationResult 
            { 
                IsValid = false, 
                ErrorMessage = "Giờ đến phải sau giờ từ" 
            };
        }

        // Kiểm tra khoảng cách tối thiểu 1 tiếng
        var duration = gioDen - gioTu;
        if (duration.TotalMinutes < 60)
        {
            return new ValidationResult 
            { 
                IsValid = false, 
                ErrorMessage = "Khung giờ tối thiểu 1 tiếng (60 phút)" 
            };
        }

        // Kiểm tra không chọn giờ quá khứ nếu ngày là hôm nay
        var today = DateOnly.FromDateTime(DateTime.Now);
        if (ngay == today)
        {
            var now = TimeOnly.FromDateTime(DateTime.Now);
            if (gioTu <= now)
            {
                return new ValidationResult 
                { 
                    IsValid = false, 
                    ErrorMessage = "Không thể chọn giờ quá khứ cho ngày hôm nay" 
                };
            }
        }

        // Kiểm tra không chọn ngày quá khứ
        if (ngay < today)
        {
            return new ValidationResult 
            { 
                IsValid = false, 
                ErrorMessage = "Không thể chọn ngày trong quá khứ" 
            };
        }

        return new ValidationResult { IsValid = true };
    }


    /// Tạo danh sách slot 1 tiếng trong khung giờ
    private List<AvailableSlotDto> GenerateSlots(TimeOnly gioTu, TimeOnly gioDen)
    {
        var slots = new List<AvailableSlotDto>();
        var currentStart = gioTu;

        while (currentStart.AddHours(1) <= gioDen)
        {
            var slotEnd = currentStart.AddHours(1);
            slots.Add(new AvailableSlotDto
            {
                GioBatDau = currentStart.ToString("HH:mm"),
                GioKetThuc = slotEnd.ToString("HH:mm")
            });
            currentStart = slotEnd;
        }

        return slots;
    }

    
    private bool IsSlotAvailable(AvailableSlotDto slot, List<chitietdatsan> bookings, DateOnly ngay)
    {
        // Ghép ngày + giờ thành DateTime đầy đủ
        var slotStart = ngay.ToDateTime(TimeOnly.ParseExact(slot.GioBatDau, "HH:mm"));
        var slotEnd = ngay.ToDateTime(TimeOnly.ParseExact(slot.GioKetThuc, "HH:mm"));

        foreach (var booking in bookings)
        {
            // Giờ cùng kiểu DateTime → so sánh được
            if (booking.giobatdau < slotEnd && booking.gioketthuc > slotStart)
                return false;
        }
        return true;
    }

    private class ValidationResult
    {
        public bool IsValid { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
