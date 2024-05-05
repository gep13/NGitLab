using System.Runtime.Serialization;

namespace NGitLab.Models
{
    public enum PackageStatus
    {
        [EnumMember(Value = "default")]
        Default,
        [EnumMember(Value = "hidden")]
        Hidden,
        [EnumMember(Value = "processing")]
        Processing,
        [EnumMember(Value = "error")]
        Error,
        [EnumMember(Value = "pending_destruction")]
        PendingDestruction,
    }
}
