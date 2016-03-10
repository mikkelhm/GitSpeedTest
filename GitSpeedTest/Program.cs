using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;

namespace GitSpeedTest
{
    class Program
    {
        private static readonly string WorkingDir = @"D:\Temp\GitSpeedTest";
        static void Main(string[] args)
        {
            File.Delete(@"d:\temp\UmbracoSpeed.txt");
            using (new AwesomeStopwatch("=== Using LigGit2"))
            {
                Common.CleanUp(WorkingDir);
                // make some mes
                using (new AwesomeStopwatch("Updating files 1st"))
                {
                    CreateFolder(WorkingDir, 0);
                }
                Common.CommitViaLibGit(WorkingDir);
                // make some mes again
                using (new AwesomeStopwatch("Updating files 2nd"))
                {
                    CreateFolder(WorkingDir, 0);
                }
                Common.CommitViaLibGit(WorkingDir);
            }
            using (new AwesomeStopwatch("=== Using LigGit2 stage *"))
            {
                Common.CleanUp(WorkingDir);
                // make some mes
                using (new AwesomeStopwatch("Updating files 1st"))
                {
                    CreateFolder(WorkingDir, 0);
                }
                Common.CommitViaLibGitStageStar(WorkingDir);
                // make some mes again
                using (new AwesomeStopwatch("Updating files 2nd"))
                {
                    CreateFolder(WorkingDir, 0);
                }
                Common.CommitViaLibGitStageStar(WorkingDir);
            }
            using (new AwesomeStopwatch("=== Using LigGit2 specific files"))
            {
                Common.CleanUp(WorkingDir);
                // make some mes
                var files = new List<string>();
                using (new AwesomeStopwatch("Updating files 1st"))
                {
                    CreateFolder(WorkingDir, 0, files);
                }
                Common.CommitSpecificViaLibGit(WorkingDir, files);
                // make some mes again
                using (new AwesomeStopwatch("Updating files 2nd"))
                {
                    files.Clear();
                    CreateFolder(WorkingDir, 0, files);
                }
                Common.CommitSpecificViaLibGit(WorkingDir, files);
            }
            using (new AwesomeStopwatch("=== Using shell out"))
            {
                Common.CleanUp(WorkingDir);
                // make some mes
                using (new AwesomeStopwatch("Updating files 1st"))
                {
                    CreateFolder(WorkingDir, 0);
                }
                Common.CommitViaShell(WorkingDir);
                // make some mes again
                using (new AwesomeStopwatch("Updating files 2nd"))
                {
                    CreateFolder(WorkingDir, 0);
                }
                Common.CommitViaShell(WorkingDir);
            }
            Console.WriteLine("DONE");
            Console.ReadLine();
        }

        private static void CreateFolder(string parent, int currentDept, IList<string> files = null)
        {
            if (currentDept >= 5)
                return;
            for (int i = 0; i < 5; i++)
            {
                var dirName = Path.Combine(parent, "Folder-" + i);
                if (!Directory.Exists(dirName))
                    Directory.CreateDirectory(dirName);
                CreateFolder(dirName, currentDept + 1, files);
                for (int f = 0; f < 5; f++)
                {
                    var fileName = Path.Combine(dirName, "File-" + f + ".txt");
                    File.WriteAllText(fileName, DateTime.Now.Ticks.ToString());
                    if (files != null)
                        files.Add(fileName);
                }
            }
        }
    }
}
