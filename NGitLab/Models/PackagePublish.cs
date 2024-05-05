using System.IO;
using System.Runtime.Serialization;

namespace NGitLab.Models
{
    public class PackagePublish
    {
        public string PackageName { get; set; }

        public string PackageVersion { get; set; }

        public Stream PackageStream { get; set; }

        public string FileName { get; set; }

        public PackagePublishStatus? Status { get; set; }
    }
}
