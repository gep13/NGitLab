using System.IO;

namespace NGitLab.Models
{
    public class PackagePublish
    {
        public string PackageName { get; set; }

        public string PackageVersion { get; set; }

        public string FileName { get; set; }

        public Stream FileStream { get; set; }

        public PackagePublishStatus? Status { get; set; }
    }
}
