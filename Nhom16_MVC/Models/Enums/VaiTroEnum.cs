using NpgsqlTypes; 

namespace Nhom16_MVC.Models.Enums
{
    public enum VaiTroEnum
    {
        [PgName("nguoiThue")]  
        NguoiThue = 0,

        [PgName("chuSan")]    
        ChuSan = 1,

        [PgName("admin")]    
        Admin = 2
    }
}