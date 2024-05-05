using System.Threading;
using System.Threading.Tasks;
using NGitLab.Models;

namespace NGitLab
{
    public interface IPackageClient
    {
        /// <summary>
        /// Add a package file with the proposed information to the GitLab Generic Package Repository for the selected Project Id.
        /// </summary>
        /// <param name="projectId">The numeric  project of or project path (namespace and project name) of the project to upload a package to.</param>
        /// <param name="packagePublish">The information about the package file to publish.</param>
        /// <returns>The package if it was created. Null if not.</returns>
        Task<Package> PublishGenericPackageAsync(ProjectId projectId, PackagePublish packagePublish, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all project packages based on the provided query parameters.
        /// </summary>
        /// /// <param name="projectId">The numeric  project of or project path (namespace and project name) of the project to search for packages in.</param>
        /// <param name="packageQuery">The query parameters to be used for the search.</param>
        /// <returns></returns>
        GitLabCollectionResponse<PackageSearchResult> Get(ProjectId projectId, PackageQuery packageQuery);

        /// <summary>
        /// Gets a single project package using the provided project and package ids.
        /// </summary>
        /// <param name="projectId">The numeric  project id or project path (namespace and project name) of the project that the package resides in.</param>
        /// <param name="packageId">The package id that is being selected.</param>
        /// <param name="cancellationToken">The cancellation token used to halt the request.</param>
        /// <returns></returns>
        Task<PackageSearchResult> GetByIdAsync(ProjectId projectId, long packageId, CancellationToken cancellationToken = default);
    }
}
