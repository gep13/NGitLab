using System.Runtime.Serialization;

namespace NGitLab.Models
{
    public enum PackageOrderBy
    {
        [EnumMember(Value = "created_at")]
        CreatedAt,
        [EnumMember(Value = "name")]
        Name,
        [EnumMember(Value = "version")]
        Version,
        [EnumMember(Value = "type")]
        Type,
    }
}
