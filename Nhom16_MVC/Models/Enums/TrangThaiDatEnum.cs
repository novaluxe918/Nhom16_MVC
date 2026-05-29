
using NpgsqlTypes;

namespace Nhom16_MVC.Models.Enums;



public enum TrangThaiDatEnum
{
    [PgName("cho_xac_nhan")]
    ChoXacNhan = 0,

    [PgName("da_xac_nhan")]
    DaXacNhan = 1,

    [PgName("da_huy")] 
    DaHuy = 2,

    [PgName("hoan_thanh")]
    HoanThanh = 3
}