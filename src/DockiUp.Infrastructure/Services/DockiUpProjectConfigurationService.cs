using DockiUp.Application.Interfaces;
using LibGit2Sharp;
using System.Text;

namespace DockiUp.Infrastructure.Services
{
    public class DockiUpProjectConfigurationService : IDockiUpProjectConfigurationService
    {
        private const string ComposeFileName = "dockiup_compose.yml";

        public async Task CloneRepositoryAsync(string projectPath, string gitUrl)
        {
            await Task.Run(() => Repository.Clone(gitUrl, projectPath));
        }

        public async Task UpdateRepositoryAsync(string projectPath)
        {
            await Task.Run(() =>
            {
                using var repo = new Repository(projectPath);

                Commands.Fetch(repo, "origin", Array.Empty<string>(), null, "");

                Branch branch = repo.Branches["main"] ?? repo.Branches["master"];
                if (branch == null)
                {
                    Console.WriteLine("Could not find a main or master branch.");
                    return;
                }

                Branch remoteBranch = branch.TrackedBranch;
                if (remoteBranch == null)
                {
                    Console.WriteLine($"Local branch '{branch.FriendlyName}' is not tracking a remote branch.");
                    return;
                }

                MergeResult mergeResult = Commands.Pull(repo, new Signature("DockiUp", "dockiup@pianonic.ch", DateTime.Now), new PullOptions());

                if (mergeResult.Status == MergeStatus.UpToDate)
                {
                    Console.WriteLine("Repository is up-to-date.");
                }
                else if (mergeResult.Status == MergeStatus.FastForward)
                {
                    Console.WriteLine("Fast-forward merge completed.");
                }
                else if (mergeResult.Status == MergeStatus.NonFastForward)
                {
                    Console.WriteLine("Non-fast-forward merge. There might be merge conflicts.");
                }
                else if (mergeResult.Status == MergeStatus.Conflicts)
                {
                    Console.WriteLine("Merge conflicts detected. Please resolve them manually.");
                }
                else
                {
                    Console.WriteLine($"Pull completed with status: {mergeResult.Status}");
                }
            });
        }

        public async Task<string> WriteComposeFileAsync(string projectPath, string composeContent)
        {
            string filePath = Path.Combine(projectPath, ComposeFileName);
            await File.WriteAllTextAsync(filePath, composeContent, Encoding.UTF8);
            return filePath;
        }
    }
}
