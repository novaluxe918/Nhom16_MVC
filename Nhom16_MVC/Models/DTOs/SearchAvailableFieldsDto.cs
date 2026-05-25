namespace Nhom16_MVC.Models.DTOs;

/// DTO cho request tìm kiếm sân trống theo khung giờ

public class SearchAvailableFieldsRequest
{

    public string? Ngay { get; set; } 


    public string? GioTu { get; set; } 

 
    public string? GioDen { get; set; } 

    //khu vực
    public string? Quan { get; set; } 

    // loại sân 
    public int? MaLoaiSan { get; set; }

    public string? TenSan { get; set; }
}


public class AvailableSlotDto
{
    public string GioBatDau { get; set; } = null!;

    public string GioKetThuc { get; set; } = null!;
}

public class AvailableFieldDetailDto
{

    public int MaSanChiTiet { get; set; }

    public string TenSanChiTiet { get; set; } = null!;

    public string? LoaiSan { get; set; }

    public long GiaThuebuoiSang { get; set; }

    public long GiaThuebuoiToi { get; set; }

    //danh sách sân trống
    public List<AvailableSlotDto> SlotsTrong { get; set; } = new();

    public int MaSanBong { get; set; }

    public string TenSan { get; set; } = null!;

    public string? HinhAnh { get; set; }

    public string? DiaChi { get; set; }

    public string? Quan { get; set; }

    public string? Huyen { get; set; }

    public string? Xa { get; set; }

    public string? ThanhPho { get; set; }

    public decimal? KinhDo { get; set; }

    public decimal? ViDo { get; set; }

    public string? MoTa { get; set; }

    public decimal? SoSaoDanhGia { get; set; }

    public int SoLuotDanhGia { get; set; }
}

public class SearchAvailableFieldsResponse
{
    /// Trạng thái 
    public bool Success { get; set; }

    /// Thông báo 
    public string? Message { get; set; }

    /// Danh sách sân con có slot trống (flat list)
    public List<AvailableFieldDetailDto> Data { get; set; } = new();

    /// Tổng số kết quả
    public int TotalCount { get; set; }
}

public class QuanItemDto
{
    public string Quan { get; set; } = null!;
}

public class LoaiSanItemDto 
{ 
    public int MaLoaiSan { get; set; }
    public string TenLoaiSan { get; set; } = null!;
}

