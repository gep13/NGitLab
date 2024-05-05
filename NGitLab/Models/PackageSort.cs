using System.Runtime.Serialization;

namespace NGitLab.Models
{
    public enum PackageSort
    {
        [EnumMember(Value = "asc")]
        Ascending,
        [EnumMember(Value = "desc")]
        Descending,
    }
}
