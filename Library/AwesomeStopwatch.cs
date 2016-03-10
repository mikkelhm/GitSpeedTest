using System;
using System.Diagnostics;
using System.IO;

namespace Library
{
    public class AwesomeStopwatch : IDisposable
    {
        private Stopwatch _stopwatch = new Stopwatch();
        private string _msg;
        private string _prefix;
        public AwesomeStopwatch()
        {
            _stopwatch.Start();
        }

        public AwesomeStopwatch(string msg, string prefix = "") : this()
        {
            _msg = msg;
            _prefix = prefix;
            Console.WriteLine(_prefix + "Starting " + msg);
            File.AppendAllText(Common.LogPath, _prefix + "Starting " + msg + Environment.NewLine);
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            Console.WriteLine(_prefix + "Done {0}: {1}", _msg, _stopwatch.Elapsed);
            File.AppendAllText(Common.LogPath, string.Format("{3}Done {0}: {1}{2}", _msg, _stopwatch.Elapsed, Environment.NewLine, _prefix));
        }
    }
}
