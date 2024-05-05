using System;
using System.Collections.Generic;
using NGitLab.Models;

namespace NGitLab.Impl;

internal sealed class ContributorClient : IContributorClient
{
    private readonly API _api;
    private readonly string _contributorPath;

    public ContributorClient(API api, string repoPath)
    {
        _api = api;
        _contributorPath = repoPath + Contributor.Url;
    }

    [Obsolete("Argument projectId is redundant, please use ContributorClient(API api, string repoPath) instead.")]
    public ContributorClient(API api, string repoPath, int projectId)
        : this(api, repoPath)
    {
    }

    /// <remarks>
    /// HACK: We force the order_by and sort due to a pagination bug from GitLab
    /// </remarks>
    public IEnumerable<Contributor> All => _api.Get().GetAll<Contributor>(_contributorPath + $"?order_by=commits&sort=desc");
}
