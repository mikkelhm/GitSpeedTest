using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;

namespace GitSpeedTestUmbraco
{
    class Program
    {
        static void Main(string[] args)
        {
            var workingDir = Path.Combine(Common.RootDir, "UmbracoSpeed");
            if (File.Exists(Common.LogPath))
                File.Delete(Common.LogPath);
            using (new AwesomeStopwatch("downloading packages"))
            {
                Common.DownloadFile(Common.GetUmbPackageUrl("7.3.0"), Common.GetUmbPath("7.3.0"));
                Common.DownloadFile(Common.GetUmbPackageUrl("7.4.1"), Common.GetUmbPath("7.4.1"));
                Common.DownloadFile(Common.GetCourierPackageUrl("2.52.3"), Common.GetCourierPath("2.52.3"));
            }
            while (true)
            {
                using (new AwesomeStopwatch("Using Shell", "=== "))
                {
                    Common.CleanUp(workingDir);
                    InitUmbraco(workingDir);
                    Common.CommitViaShell(workingDir);
                    UpgradeUmbraco(workingDir);
                    Common.CommitViaShell(workingDir);
                    AddCourier(workingDir);
                    Common.CommitViaShell(workingDir);
                }
                using (new AwesomeStopwatch("Using LibGit2", "=== "))
                {
                    Common.CleanUp(workingDir);
                    InitUmbraco(workingDir);
                    Common.CommitViaLibGit(workingDir);
                    UpgradeUmbraco(workingDir);
                    Common.CommitViaLibGit(workingDir);
                    AddCourier(workingDir);
                    Common.CommitViaLibGit(workingDir);
                }
                using (new AwesomeStopwatch("Using LibGit2 stage *", "=== "))
                {
                    Common.CleanUp(workingDir);
                    InitUmbraco(workingDir);
                    Common.CommitViaLibGitStageStar(workingDir);
                    UpgradeUmbraco(workingDir);
                    Common.CommitViaLibGitStageStar(workingDir);
                    AddCourier(workingDir);
                    Common.CommitViaLibGitStageStar(workingDir);
                }
                using (new AwesomeStopwatch(", trueUsing LibGit2 for specific files", "=== "))
                {
                    Common.CleanUp(workingDir);
                    var files = InitUmbraco(workingDir);
                    Common.CommitSpecificViaLibGit(workingDir, files);
                    files = UpgradeUmbraco(workingDir);
                    Common.CommitSpecificViaLibGit(workingDir, files);
                    files = AddCourier(workingDir);
                    Common.CommitSpecificViaLibGit(workingDir, files);
                }
                
            }

            Console.WriteLine("DONE");
            Console.ReadLine();
        }

        private static IEnumerable<string> InitUmbraco(string workingDir)
        {
            using (new AwesomeStopwatch("Initializing Umbraco 7.3"))
            {
                return Zip.ExtractZipFile(Common.GetUmbPath("7.3.0"), workingDir);
            }
        }

        private static IEnumerable<string> UpgradeUmbraco(string workingDir)
        {
            using (new AwesomeStopwatch("Upgrading Umbraco 7.4.1"))
            {
                return Zip.ExtractZipFile(Common.GetUmbPath("7.4.1"), workingDir);
            }
        }

        private static IEnumerable<string> AddCourier(string workingDir)
        {
            using (new AwesomeStopwatch("Adding Courier.Concorde.UI.v2.52.3"))
            {
                return Zip.ExtractZipFile(Common.GetCourierPath("2.52.3"), workingDir);
            }
        }
    }
}
