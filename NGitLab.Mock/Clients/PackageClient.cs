using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NGitLab.Models;

namespace NGitLab.Mock.Clients
{
    internal sealed class PackageClient : ClientBase, IPackageClient
    {
        public PackageClient(ClientContext context)
            : base(context)
        {
        }

        public Task<Package> PublishGenericPackageAsync(ProjectId projectId, PackagePublish packagePublish, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public GitLabCollectionResponse<PackageSearchResult> Get(ProjectId projectId, PackageQuery packageQuery)
        {
            throw new System.NotImplementedException();
        }

        public Task<PackageSearchResult> GetByIdAsync(ProjectId projectId, long packageId, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
    }
}
