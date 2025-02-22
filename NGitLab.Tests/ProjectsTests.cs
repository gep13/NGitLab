﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NGitLab.Models;
using NGitLab.Tests.Docker;
using NUnit.Framework;

namespace NGitLab.Tests
{
    public class ProjectsTests
    {
        [Test]
        [NGitLabRetry]
        public async Task GetProjectByIdAsync()
        {
            using var context = await GitLabTestContext.CreateAsync();
            var project = context.CreateProject();
            var projectClient = context.Client.Projects;

            var projectResult = await projectClient.GetByIdAsync(project.Id, new SingleProjectQuery(), CancellationToken.None);
            Assert.AreEqual(project.Id, projectResult.Id);
        }

        [Test]
        [NGitLabRetry]
        public async Task GetByNamespacedPathAsync()
        {
            using var context = await GitLabTestContext.CreateAsync();
            var project = context.CreateProject();
            var projectClient = context.Client.Projects;

            var projectResult = await projectClient.GetByNamespacedPathAsync(project.PathWithNamespace, new SingleProjectQuery(), CancellationToken.None);
            Assert.AreEqual(project.Id, projectResult.Id);
        }

        [Test]
        [NGitLabRetry]
        public async Task GetProjectsAsync()
        {
            using var context = await GitLabTestContext.CreateAsync();
            var project = context.CreateProject();
            var projectClient = context.Client.Projects;

            var projects = new List<Project>();
            await foreach (var item in projectClient.GetAsync(new ProjectQuery()))
            {
                projects.Add(item);
            }

            CollectionAssert.IsNotEmpty(projects);
        }

        [Test]
        [NGitLabRetry]
        public async Task GetOwnedProjects()
        {
            using var context = await GitLabTestContext.CreateAsync();
            var project = context.CreateProject();
            var projectClient = context.Client.Projects;

            var projects = projectClient.Owned.Take(30).ToArray();
            CollectionAssert.IsNotEmpty(projects);
        }

        [Test]
        [NGitLabRetry]
        public async Task GetVisibleProjects()
        {
            using var context = await GitLabTestContext.CreateAsync();
            var project = context.CreateProject();
            var projectClient = context.Client.Projects;

            var projects = projectClient.Visible.Take(30).ToArray();

            CollectionAssert.IsNotEmpty(projects);
        }

        [Test]
        [NGitLabRetry]
        public async Task GetAccessibleProjects()
        {
            using var context = await GitLabTestContext.CreateAsync();
            var project = context.CreateProject();
            var projectClient = context.Client.Projects;

            var projects = projectClient.Accessible.Take(30).ToArray();

            CollectionAssert.IsNotEmpty(projects);
        }

        [Test]
        [NGitLabRetry]
        public async Task GetProjectsByQuery()
        {
            using var context = await GitLabTestContext.CreateAsync();
            var project = context.CreateProject();
            var projectClient = context.Client.Projects;

            var query = new ProjectQuery
            {
                Simple = true,
                Search = project.Name,
            };

            var projects = projectClient.Get(query).Take(10).ToArray();
            Assert.AreEqual(1, projects.Length);
        }

        [Test]
        [NGitLabRetry]
        public async Task GetProjectsStatistics()
        {
            using var context = await GitLabTestContext.CreateAsync();
            var project = context.CreateProject();
            var projectClient = context.Client.Projects;

            var projects = projectClient.Get(new ProjectQuery { Statistics = true }).Take(10).ToList();
            if (projects.Count == 0)
            {
                Assert.Fail("No projects found.");
            }

            projects.ForEach(p => Assert.IsNotNull(p.Statistics));
        }

        [Test]
        [NGitLabRetry]
        public async Task GetProjectsProperties()
        {
            using var context = await GitLabTestContext.CreateAsync();
            var project = context.CreateProject();
            var projectClient = context.Client.Projects;

            var projects = projectClient.Get(new ProjectQuery()).ToList();

            if (projects.Count == 0)
            {
                Assert.Fail("No projects found.");
            }

            projects.ForEach(p => Assert.IsNotNull(p.Links));
            projects.ForEach(p => Assert.IsNotNull(p.MergeMethod));
        }

        [Test]
        [NGitLabRetry]
        public async Task GetProjectsByQuery_VisibilityInternal()
        {
            using var context = await GitLabTestContext.CreateAsync();
            var project = context.CreateProject(p => p.VisibilityLevel = VisibilityLevel.Internal);
            var projectClient = context.Client.Projects;

            var query = new ProjectQuery
            {
                Simple = true,
                Visibility = VisibilityLevel.Internal,
            };

            var projects = projectClient.Get(query).ToList();

            CollectionAssert.IsNotEmpty(projects);
        }

        [Test]
        [NGitLabRetry]
        public async Task GetProjectByIdByQuery_Statistics()
        {
            using var context = await GitLabTestContext.CreateAsync();
            var project = context.CreateProject(p => p.VisibilityLevel = VisibilityLevel.Internal);
            var projectClient = context.Client.Projects;

            var query = new SingleProjectQuery
            {
                Statistics = true,
            };

            project = projectClient.GetById(project.Id, query);

            Assert.IsNotNull(project);
            Assert.IsNotNull(project.Statistics);
        }

        [Test]
        [NGitLabRetry]
        public async Task GetProjectLanguages()
        {
            using var context = await GitLabTestContext.CreateAsync();
            var project = context.CreateProject(p => p.VisibilityLevel = VisibilityLevel.Internal);
            var projectClient = context.Client.Projects;

            var file = new FileUpsert
            {
                Branch = project.DefaultBranch,
                CommitMessage = "add javascript file",
                Content = "var test = 0;",
                Path = "test.js",
            };

            context.Client.GetRepository(project.Id).Files.Create(file);
            var languages = projectClient.GetLanguages(project.Id.ToString(CultureInfo.InvariantCulture));
            Assert.That(languages.Count, Is.EqualTo(1));
            StringAssert.AreEqualIgnoringCase("javascript", languages.First().Key);
            Assert.That(languages.First().Value, Is.EqualTo(100));
        }

        [Test]
        [NGitLabRetry]
        public async Task GetProjectsCanSpecifyTheProjectPerPageCount()
        {
            using var context = await GitLabTestContext.CreateAsync();
            var project = context.CreateProject(p => p.VisibilityLevel = VisibilityLevel.Internal);
            var projectClient = context.Client.Projects;

            var query = new ProjectQuery
            {
                Simple = true,
                Visibility = VisibilityLevel.Internal,
                PerPage = 5,
            };

            var projects = projectClient.Get(query).Take(10).ToList();

            CollectionAssert.IsNotEmpty(projects);
            Assert.That(context.LastRequest.RequestUri.ToString(), Contains.Substring("per_page=5"));
        }

        [TestCase(false)]
        [TestCase(true)]
        [NGitLabRetry]
        public async Task CreateUpdateDelete(bool initiallySetTagsInsteadOfTopics)
        {
            using var context = await GitLabTestContext.CreateAsync();
            var projectClient = context.Client.Projects;

            var project = new ProjectCreate
            {
                Description = "desc",
                IssuesEnabled = true,
                MergeRequestsEnabled = true,
                Name = "CreateDelete_Test_" + context.GetRandomNumber().ToString(CultureInfo.InvariantCulture),
                NamespaceId = null,
                SnippetsEnabled = true,
                VisibilityLevel = VisibilityLevel.Internal,
                WikiEnabled = true,
            };

            var expectedTopics = new List<string> { "Tag-1", "Tag-2" };
            if (initiallySetTagsInsteadOfTopics)
                project.Tags = expectedTopics;
            else
                project.Topics = expectedTopics;

            var createdProject = projectClient.Create(project);

            Assert.AreEqual(project.Description, createdProject.Description);
            Assert.AreEqual(project.IssuesEnabled, createdProject.IssuesEnabled);
            Assert.AreEqual(project.MergeRequestsEnabled, createdProject.MergeRequestsEnabled);
            Assert.AreEqual(project.Name, createdProject.Name);
            Assert.AreEqual(project.VisibilityLevel, createdProject.VisibilityLevel);
            CollectionAssert.AreEquivalent(expectedTopics, createdProject.Topics);
            CollectionAssert.AreEquivalent(expectedTopics, createdProject.TagList);
            Assert.AreEqual(RepositoryAccessLevel.Enabled, createdProject.RepositoryAccessLevel);

            // Update
            expectedTopics = new List<string> { "Tag-3" };
            var updateOptions = new ProjectUpdate { Visibility = VisibilityLevel.Private, Topics = expectedTopics };
            var updatedProject = projectClient.Update(createdProject.Id.ToString(CultureInfo.InvariantCulture), updateOptions);
            Assert.AreEqual(VisibilityLevel.Private, updatedProject.VisibilityLevel);
            CollectionAssert.AreEquivalent(expectedTopics, updatedProject.Topics);
            CollectionAssert.AreEquivalent(expectedTopics, updatedProject.TagList);

            updateOptions.Visibility = VisibilityLevel.Public;
            updateOptions.Topics = null;    // If Topics are null, the project's existing topics will remain
            updatedProject = projectClient.Update(createdProject.Id.ToString(CultureInfo.InvariantCulture), updateOptions);
            Assert.AreEqual(VisibilityLevel.Public, updatedProject.VisibilityLevel);
            CollectionAssert.AreEquivalent(expectedTopics, updatedProject.Topics);
            CollectionAssert.AreEquivalent(expectedTopics, updatedProject.TagList);

            var updatedProject2 = projectClient.Update(createdProject.PathWithNamespace, new ProjectUpdate { Visibility = VisibilityLevel.Internal });
            Assert.AreEqual(VisibilityLevel.Internal, updatedProject2.VisibilityLevel);

            projectClient.Delete(createdProject.Id);
        }

        // No owner level (50) for project! See https://docs.gitlab.com/ee/api/members.html
        [TestCase(AccessLevel.Guest)]
        [TestCase(AccessLevel.Reporter)]
        [TestCase(AccessLevel.Developer)]
        [TestCase(AccessLevel.Maintainer)]
        [NGitLabRetry]
        public async Task Test_get_by_project_query_projectQuery_MinAccessLevel_returns_projects(AccessLevel accessLevel)
        {
            using var context = await GitLabTestContext.CreateAsync();
            var project = context.CreateProject();
            var projectClient = context.Client.Projects;

            // Arrange
            var query = new ProjectQuery
            {
                MinAccessLevel = accessLevel,
            };

            // Act
            var result = projectClient.Get(query).Take(10).ToArray();

            // Assert
            Assert.IsTrue(result.Any());
        }

        [Test]
        [NGitLabRetry]
        public async Task ForkProject()
        {
            using var context = await GitLabTestContext.CreateAsync();
            var projectClient = context.Client.Projects;

            var createdProject = context.CreateProject(p =>
            {
                p.Description = "desc";
                p.IssuesEnabled = true;
                p.MergeRequestsEnabled = true;
                p.Name = "ForkProject_Test_" + context.GetRandomNumber().ToString(CultureInfo.InvariantCulture);
                p.NamespaceId = null;
                p.SnippetsEnabled = true;
                p.VisibilityLevel = VisibilityLevel.Internal;
                p.WikiEnabled = true;
                p.Topics = new List<string> { "Tag-1", "Tag-2" };
            });

            context.Client.GetRepository(createdProject.Id).Files.Create(new FileUpsert
            {
                Branch = createdProject.DefaultBranch,
                CommitMessage = "add readme",
                Path = "README.md",
                RawContent = "this project should only live during the unit tests, you can delete if you find some",
            });

            var forkedProject = projectClient.Fork(createdProject.Id.ToString(CultureInfo.InvariantCulture), new ForkProject
            {
                Path = createdProject.Path + "-fork",
                Name = createdProject.Name + "Fork",
            });

            // Wait for the fork to be ready
            await GitLabTestContext.RetryUntilAsync(() => projectClient[forkedProject.Id], p => string.Equals(p.ImportStatus, "finished", StringComparison.Ordinal), TimeSpan.FromMinutes(2));

            var forks = projectClient.GetForks(createdProject.Id.ToString(CultureInfo.InvariantCulture), new ForkedProjectQuery());
            Assert.That(forks.Single().Id, Is.EqualTo(forkedProject.Id));

            // Create a merge request with AllowCollaboration (only testable on a fork, also the source branch must not be protected)
            context.Client.GetRepository(forkedProject.Id).Branches.Create(new BranchCreate { Name = "branch-test", Ref = createdProject.DefaultBranch });
            var mr = context.Client.GetMergeRequest(forkedProject.Id).Create(new MergeRequestCreate
            {
                AllowCollaboration = true,
                Description = "desc",
                SourceBranch = "branch-test",
                TargetBranch = createdProject.DefaultBranch,
                TargetProjectId = createdProject.Id,
                Title = "title",
            });

            Assert.That(mr.AllowCollaboration, Is.True);

            projectClient.Delete(forkedProject.Id);
            projectClient.Delete(createdProject.Id);
        }

        [Test]
        [NGitLabRetry]
        public async Task GetProjectsByLastActivity()
        {
            using var context = await GitLabTestContext.CreateAsync();
            var project = context.CreateProject();
            var projectClient = context.Client.Projects;

            var date = DateTimeOffset.UtcNow.AddMonths(-1);
            var query = new ProjectQuery
            {
                LastActivityAfter = date,
                OrderBy = "last_activity_at",
                Ascending = true,
            };

            var projects = projectClient.Get(query).Take(10).ToList();
            CollectionAssert.IsNotEmpty(projects);
            Assert.That(projects.Select(p => p.LastActivityAt), Is.All.GreaterThan(date.UtcDateTime));
        }

        [Test]
        [NGitLabRetry]
        public async Task IsEmpty()
        {
            using var context = await GitLabTestContext.CreateAsync();
            var projectClient = context.Client.Projects;

            var createdProject = context.CreateProject(prj =>
            {
                prj.Name = "Project_Test_" + context.GetRandomNumber().ToString(CultureInfo.InvariantCulture);
                prj.VisibilityLevel = VisibilityLevel.Internal;
            });
            Assert.IsTrue(createdProject.EmptyRepo);

            context.Client.GetRepository(createdProject.Id).Files.Create(new FileUpsert
            {
                Branch = createdProject.DefaultBranch,
                CommitMessage = "add readme",
                Path = "README.md",
                RawContent = "this project should only live during the unit tests, you can delete if you find some",
            });

            createdProject = projectClient[createdProject.Id];
            Assert.IsFalse(createdProject.EmptyRepo);
        }

        [Test]
        [NGitLabRetry]
        public async Task GetProjectByTopics()
        {
            // Arrange
            using var context = await GitLabTestContext.CreateAsync();

            var topicRequired1 = CreateTopic();
            var topicRequired2 = CreateTopic();
            var topicOptional1 = CreateTopic();
            var topicOptional2 = CreateTopic();

            context.CreateProject();
            context.CreateProject(p => p.Topics = new List<string> { topicRequired1, topicOptional1 });
            context.CreateProject(p => p.Topics = new List<string> { topicRequired1, topicRequired2 });
            context.CreateProject(p => p.Topics = new List<string> { topicRequired1, topicOptional2 });
            context.CreateProject(p => p.Topics = new List<string> { topicRequired1, topicOptional1, topicRequired2 });
            context.CreateProject(p => p.Topics = new List<string> { topicOptional1, topicOptional2, topicRequired2 });

            var projectClient = context.Client.Projects;

            var query = new ProjectQuery();
            query.Topics.Add(topicRequired1);
            query.Topics.Add(topicRequired2);

            // Act
            var projects = projectClient.Get(query); // Get projects that have both required topics

            // Assert
            Assert.AreEqual(2, projects.Count());

            static string CreateTopic() => Guid.NewGuid().ToString("N");
        }

        [TestCase(null)]
        [TestCase(SquashOption.Always)]
        [TestCase(SquashOption.Never)]
        [TestCase(SquashOption.DefaultOff)]
        [TestCase(SquashOption.DefaultOn)]
        [NGitLabRetry]
        public async Task CreateProjectWithSquashOption(SquashOption? inputSquashOption)
        {
            using var context = await GitLabTestContext.CreateAsync();
            var projectClient = context.Client.Projects;

            var project = new ProjectCreate
            {
                Description = "desc",
                Name = "CreateProjectWithSquashOption_Test_" + context.GetRandomNumber().ToString(CultureInfo.InvariantCulture),
                VisibilityLevel = VisibilityLevel.Internal,
                SquashOption = inputSquashOption,
            };

            var createdProject = projectClient.Create(project);

            var expectedSquashOption = inputSquashOption ?? SquashOption.DefaultOff;
            Assert.AreEqual(expectedSquashOption, createdProject.SquashOption);

            projectClient.Delete(createdProject.Id);
        }
    }
}
