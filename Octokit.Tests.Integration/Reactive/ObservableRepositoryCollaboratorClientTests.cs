﻿using System.Reactive.Linq;
using System.Threading.Tasks;
using Octokit;
using Octokit.Reactive;
using Octokit.Tests.Integration;
using Xunit;
using Octokit.Tests.Integration.Helpers;

public class ObservableRepositoryCollaboratorClientTests
{
    public class TheGetAllMethod
    {
        [IntegrationTest]
        public async Task ReturnsAllCollaborators()
        {
            var github = Helper.GetAuthenticatedClient();
            var repoName = Helper.MakeNameWithTimestamp("public-repo");

            using (var context = await github.CreateRepositoryContext(new NewRepository(repoName)))
            {
                var fixture = new ObservableRepoCollaboratorsClient(github);

                // add a collaborator
                await fixture.Add(context.RepositoryOwner, context.RepositoryName, "m-zuber-octokit-integration-tests");

                var collaborators = await fixture.GetAll(context.RepositoryOwner, context.RepositoryName).ToList();
                Assert.NotNull(collaborators);
                Assert.Equal(2, collaborators.Count);
                Assert.NotNull(collaborators[0].Permissions);
                Assert.NotNull(collaborators[1].Permissions);
            }
        }

        [IntegrationTest]
        public async Task ReturnsCorrectCountOfCollaboratorsWithoutStart()
        {
            var github = Helper.GetAuthenticatedClient();
            var repoName = Helper.MakeNameWithTimestamp("public-repo");

            using (var context = await github.CreateRepositoryContext(new NewRepository(repoName)))
            {
                var fixture = new ObservableRepoCollaboratorsClient(github);

                // add some collaborators
                await fixture.Add(context.RepositoryOwner, context.RepositoryName, "m-zuber-octokit-integration-tests");

                var options = new ApiOptions
                {
                    PageSize = 1,
                    PageCount = 1
                };

                var collaborators = await fixture.GetAll(context.RepositoryOwner, context.RepositoryName, options).ToList();
                Assert.NotNull(collaborators);
                Assert.Equal(1, collaborators.Count);
            }
        }

        [IntegrationTest]
        public async Task ReturnsCorrectCountOfCollaboratorsWithStart()
        {
            var github = Helper.GetAuthenticatedClient();
            var repoName = Helper.MakeNameWithTimestamp("public-repo");

            using (var context = await github.CreateRepositoryContext(new NewRepository(repoName)))
            {
                var fixture = new ObservableRepoCollaboratorsClient(github);

                // add some collaborators
                await fixture.Add(context.RepositoryOwner, context.RepositoryName, "m-zuber-octokit-integration-tests");

                var options = new ApiOptions
                {
                    PageSize = 1,
                    PageCount = 1,
                    StartPage = 2
                };

                var collaborators = await fixture.GetAll(context.RepositoryOwner, context.RepositoryName, options).ToList();
                Assert.NotNull(collaborators);
                Assert.Equal(1, collaborators.Count);
            }
        }

        [IntegrationTest]
        public async Task ReturnsDistinctResultsBasedOnStartPage()
        {
            var github = Helper.GetAuthenticatedClient();
            var repoName = Helper.MakeNameWithTimestamp("public-repo");

            using (var context = await github.CreateRepositoryContext(new NewRepository(repoName)))
            {
                var fixture = new ObservableRepoCollaboratorsClient(github);

                // add some collaborators
                await fixture.Add(context.RepositoryOwner, context.RepositoryName, "m-zuber-octokit-integration-tests");

                var startOptions = new ApiOptions
                {
                    PageSize = 1,
                    PageCount = 1
                };

                var firstPage = await fixture.GetAll(context.RepositoryOwner, context.RepositoryName, startOptions).ToList();

                var skipStartOptions = new ApiOptions
                {
                    PageSize = 1,
                    PageCount = 1,
                    StartPage = 2
                };

                var secondPage = await fixture.GetAll(context.RepositoryOwner, context.RepositoryName, skipStartOptions).ToList();

                Assert.NotEqual(firstPage[0].Id, secondPage[0].Id);
            }
        }
    }

    public class TheIsCollaboratorMethod
    {
        [IntegrationTest]
        public async Task ReturnsTrueIfUserIsCollaborator()
        {
            var github = Helper.GetAuthenticatedClient();
            var repoName = Helper.MakeNameWithTimestamp("public-repo");

            using (var context = await github.CreateRepositoryContext(new NewRepository(repoName)))
            {
                var fixture = new ObservableRepoCollaboratorsClient(github);

                // add a collaborator
                fixture.Add(context.RepositoryOwner, context.RepositoryName, "m-zuber-octokit-integration-tests");

                var isCollab = await fixture.IsCollaborator(context.RepositoryOwner, context.RepositoryName, "m-zuber-octokit-integration-tests");

                Assert.True(isCollab);
            }
        }
    }
}