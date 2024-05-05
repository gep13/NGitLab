using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using NGitLab.Models;

namespace NGitLab.Impl
{
    public class PackageClient : IPackageClient
    {
        private const string PublishPackageUrl = "/projects/{0}/packages/generic/{1}/{2}/{3}";
        private const string GetPackagesUrl = "/projects/{0}/packages";
        private const string GetPackageUrl = "/projects/{0}/packages/{1}";

        private readonly API _api;

        public PackageClient(API api)
        {
            _api = api;
        }

        public Task<Package> PublishGenericPackageAsync(ProjectId projectId, PackagePublish packagePublish, CancellationToken cancellationToken = default)
        {
            var formData = new FileContent(packagePublish.FileStream);

            var url = string.Format(CultureInfo.InvariantCulture, PublishPackageUrl,
                projectId.ValueAsUriParameter(),
                Uri.EscapeDataString(packagePublish.PackageName),
                Uri.EscapeDataString(packagePublish.PackageVersion),
                Uri.EscapeDataString(packagePublish.FileName));

            if (packagePublish.Status.HasValue)
            {
                url = Utils.AddParameter(url, "status", packagePublish.Status.Value);
            }

            // Make GitLab return information about the uploaded file (by default, the respone is empty)
            url = Utils.AddParameter(url, "select", "package_file");

            return _api.Put().With(formData).ToAsync<Package>(url, cancellationToken);
        }

        public GitLabCollectionResponse<PackageSearchResult> Get(ProjectId projectId, PackageQuery packageQuery)
        {
            var url = CreateGetUrl(projectId, packageQuery);
            return _api.Get().GetAllAsync<PackageSearchResult>(url);
        }

        public Task<PackageSearchResult> GetByIdAsync(ProjectId projectId, long packageId, CancellationToken cancellationToken = default)
        {
            return _api.Get().ToAsync<PackageSearchResult>(string.Format(CultureInfo.InvariantCulture, GetPackageUrl, projectId.ValueAsUriParameter(), packageId), cancellationToken);
        }

        private static string CreateGetUrl(ProjectId projectId, PackageQuery query)
        {
            var url = string.Format(CultureInfo.InvariantCulture, GetPackagesUrl, projectId.ValueAsUriParameter());

            url = Utils.AddParameter(url, "order_by", query.OrderBy);
            url = Utils.AddParameter(url, "sort", query.Sort);
            url = Utils.AddParameter(url, "status", query.Status);
            url = Utils.AddParameter(url, "page", query.Page);
            url = Utils.AddParameter(url, "per_page", query.PerPage);

            if (query.PackageType != PackageType.All)
            {
                url = Utils.AddParameter(url, "package_type", query.PackageType);
            }

            if (!string.IsNullOrWhiteSpace(query.PackageName))
            {
                url = Utils.AddParameter(url, "package_name", query.PackageName);
            }

            if (!string.IsNullOrWhiteSpace(query.PackageVersion))
            {
                url = Utils.AddParameter(url, "package_version", query.PackageVersion);
            }

            if (query.IncludeVersionless)
            {
                url = Utils.AddParameter(url, "include_versionless", true);
            }

            return url;
        }
    }
}
