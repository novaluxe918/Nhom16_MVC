namespace Nhom16_MVC.Models.Enums;

/// Trạng thái của giao dịch nạp tiền
public enum TrangThaiNapEnum
{

    [System.Runtime.Serialization.EnumMember(Value = "cho_xu_ly")]
    ChoXuLy = 0,

    [System.Runtime.Serialization.EnumMember(Value = "thanh_cong")]
    ThanhCong = 1,

    [System.Runtime.Serialization.EnumMember(Value = "that_bai")]
    ThatBai = 2
}
