using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using LibGit2Sharp;

namespace Library
{
    public class Common
    {
        public static readonly string RootDir = ConfigurationManager.AppSettings["Root"];
        public static string LogPath = Path.Combine(RootDir, "UmbracoSpeed.txt");

        public static void CleanUp(string workingDir)
        {
            using (new AwesomeStopwatch("Cleaning up"))
            {
                if (!Directory.Exists(workingDir))
                    Directory.CreateDirectory(workingDir);
                else
                {
                    DeleteDirectory(workingDir);
                }
                Repository.Init(workingDir);
                using (var repository = new Repository(workingDir))
                {
                    repository.Config.Set("core.autocrlf", false);
                }
            }
        }

        private static void DeleteDirectory(string targetDir)
        {
            File.SetAttributes(targetDir, FileAttributes.Normal);

            string[] files = Directory.GetFiles(targetDir);
            string[] dirs = Directory.GetDirectories(targetDir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(targetDir, false);
        }

        public static void CommitViaLibGit(string workingDir)
        {
            using (new AwesomeStopwatch("Committing via LibGit", "***** "))
            {
                using (var repository = new Repository(workingDir))
                {
                    var repositoryStatus = repository.RetrieveStatus();
                    if (repositoryStatus.IsDirty == false)
                        return;

                    if (repositoryStatus.Added.Any())
                        repository.Stage(repositoryStatus.Added.Select(x => x.FilePath));

                    if (repositoryStatus.Untracked.Any())
                        repository.Stage(repositoryStatus.Untracked.Select(x => x.FilePath));

                    if (repositoryStatus.Modified.Any())
                        repository.Stage(repositoryStatus.Modified.Select(x => x.FilePath));

                    if (repositoryStatus.Missing.Any())
                        repository.Remove(repositoryStatus.Missing.Select(x => x.FilePath));

                    var author = new Signature("Umbraco as a Service", "support@umbraco.io",
                        new DateTimeOffset(DateTime.UtcNow));
                    var commit = repository.Commit("Updates via LibGit2Sharp", author, author);
                }
            }
        }

        public static void CommitViaLibGitStageStar(string workingDir)
        {
            using (new AwesomeStopwatch("Committing via LibGit stage *", "***** "))
            {
                using (var repository = new Repository(workingDir))
                {
                    repository.Stage("*");

                    var author = new Signature("Umbraco as a Service", "support@umbraco.io",
                        new DateTimeOffset(DateTime.UtcNow));
                    var commit = repository.Commit("Updates via LibGit2Sharp stage *", author, author);
                }
            }
        }

        public static void CommitSpecificViaLibGit(string workingDir, IEnumerable<string> files)
        {
            using (new AwesomeStopwatch("Committing via LibGit", "***** "))
            {
                using (var repository = new Repository(workingDir))
                {
                    if (files.Any())
                        repository.Stage(files);

                    var author = new Signature("Umbraco as a Service", "support@umbraco.io",
                        new DateTimeOffset(DateTime.UtcNow));
                    var commit = repository.Commit("Updates specific files via LibGit2Sharp", author, author);
                }
            }
        }

        public static void CommitViaShell(string workingDir)
        {
            using (new AwesomeStopwatch("Committing via Shell", "***** "))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo("git.exe");

                startInfo.UseShellExecute = false;
                startInfo.WorkingDirectory = workingDir;
                startInfo.RedirectStandardInput = true;
                startInfo.RedirectStandardOutput = true;
                startInfo.Arguments = "add -A ";

                Process process = new Process();
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit(); // Waits here for the process to exit.
                using (var repository = new Repository(workingDir))
                {
                    var author = new Signature("Umbraco as a Service", "support@umbraco.io",
                        new DateTimeOffset(DateTime.UtcNow));
                    var commit = repository.Commit("Updates via Shell out -A", author, author);
                }
            }
        }

        public static void DownloadFile(string url, string savePath)
        {
            if (!File.Exists(savePath))
            {
                using (var webclient = new WebClient())
                {
                    webclient.DownloadFile(url, savePath);
                }
            }
        }

        public static string GetUmbPath(string version)
        {
            return Path.Combine(RootDir, string.Format("UmbracoCms.{0}.zip", version));
        }
        public static string GetCourierPath(string version)
        {
            return Path.Combine(RootDir, string.Format("Courier.Concorde.UI.v{0}.zip", version));
        }
        public static string GetUmbPackageUrl(string version)
        {
            return string.Format("{0}UmbracoCms.{1}.zip", "http://umbracoreleases.blob.core.windows.net/download/", version);
        }
        public static string GetCourierPackageUrl(string version)
        {
            return string.Format("{0}Courier.Concorde.UI.v{1}.zip", "https://umbraconightlies.blob.core.windows.net/umbraco-deploy-release/", version);
        }
    }

}
