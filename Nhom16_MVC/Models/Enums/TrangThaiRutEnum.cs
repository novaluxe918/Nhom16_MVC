namespace Nhom16_MVC.Models.Enums;


/// Trạng thái của yêu cầu rút tiền

public enum TrangThaiRutEnum
{

    [System.Runtime.Serialization.EnumMember(Value = "cho_xu_ly")]
    ChoXuLy = 0,

    [System.Runtime.Serialization.EnumMember(Value = "da_chuyen")]
    DaChuyen = 1,

    [System.Runtime.Serialization.EnumMember(Value = "that_bai")]
    ThatBai = 2
}
