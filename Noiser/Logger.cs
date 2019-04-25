using System;
using System.IO;

namespace Noiser
{
    public class Logger
    {
        private static readonly string LogPath = "log.txt";
        private void TryLog(string line)
        {
            try
            {
                File.AppendAllText(LogPath, line + Environment.NewLine);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not log into file for some reason: {e}");
            }
            finally
            {
                Console.WriteLine(line);
            }
        }

        public void LogStart() => TryLog($"Start at {DateTime.Now}");

        public void LogEnd() => TryLog($"Stop at {DateTime.Now}");

        public void LogInfo(string info) => TryLog(info);
    }
}
