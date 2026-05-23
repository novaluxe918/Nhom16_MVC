namespace Nhom16_MVC.Models.Enums;

/// Trạng thái của đặt sân

public enum TrangThaiDatEnum
{

    [System.Runtime.Serialization.EnumMember(Value = "cho_xac_nhan")]
    ChoXacNhan = 0,

    [System.Runtime.Serialization.EnumMember(Value = "da_xac_nhan")]
    DaXacNhan = 1,

    [System.Runtime.Serialization.EnumMember(Value = "da_huy")]
    DaHuy = 2,

    [System.Runtime.Serialization.EnumMember(Value = "hoan_thanh")]
    HoanThanh = 3
}
