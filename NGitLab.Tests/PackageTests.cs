using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NGitLab.Models;
using NGitLab.Tests.Docker;
using NUnit.Framework;

namespace NGitLab.Tests
{
    public class PackageTests
    {
        [Test]
        [NGitLabRetry]
        public async Task Test_publish_package()
        {
            using var context = await GitLabTestContext.CreateAsync();
            var project = context.CreateProject();
            var packagesClient = context.Client.Packages;

            using var contentStream = CreateMemoryStream("Some Package Content");

            var packagePublish = new PackagePublish
            {
                FileName = "README.md",
                PackageName = "Packages",
                PackageVersion = "1.0.0",
                FileStream = contentStream,
            };

            var newGenericPackage = await packagesClient.PublishGenericPackageAsync(project.Id, packagePublish);

            var packageQuery = new PackageQuery { PackageType = PackageType.Generic };
            var genericPackages = packagesClient.Get(project.Id, packageQuery).ToList();
            var singleGenericPackage = await packagesClient.GetByIdAsync(project.Id, newGenericPackage.PackageId);

            Assert.AreEqual(1, genericPackages.Count);
            Assert.AreEqual(newGenericPackage.PackageId, genericPackages[0].PackageId);
            Assert.AreEqual(singleGenericPackage.PackageId, newGenericPackage.PackageId);
        }

        private static MemoryStream CreateMemoryStream(string content)
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            return new MemoryStream(bytes);
        }
    }
}
