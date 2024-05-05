using System.Runtime.Serialization;

namespace NGitLab.Models
{
    public enum PackagePublishStatus
    {
        [EnumMember(Value = "default")]
        Default,
        [EnumMember(Value = "hidden")]
        Hidden
    }
}
