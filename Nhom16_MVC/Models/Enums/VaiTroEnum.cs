using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Nhom16_MVC.Models.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum VaiTroEnum
    {
        [EnumMember(Value = "nguoiThue")]
        nguoiThue = 0,

        [EnumMember(Value = "chuSan")]
        chuSan = 1,

        [EnumMember(Value = "admin")]
        admin = 2
    }
}