using System;
using System.Threading;
using System.IO;

namespace Cymbal
{
    /// <summary>
    /// Helper class which retries opening a locked file a certain number of times before failing.
    /// </summary>
    public class FileOpener
    {
        public static bool TryOpen(string path, ref FileStream fs, int retries = 5)
        {
            bool success = false;

            while (!success && retries-- > 0)
            {
                try
                {
                    fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None);
                    success = true;
                }
                catch (IOException e)
                {
                    // This is ok... other exceptions get thrown.
                }

                if (!success)
                {
                    // Wait for a second before retrying
                    Thread.Sleep(1000);
                }
            }

            return success;
        }
    }
}
