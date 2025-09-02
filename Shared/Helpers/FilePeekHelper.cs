using System;
using System.IO;
using System.Threading.Tasks;

namespace Shared.Helpers
{
    public static class FilePeekHelper
    {
        /// <summary>
        /// Asynchronously reads the first <paramref name="numLines"/> lines from the specified file.
        /// </summary>
        /// <param name="lsPath">Full path to the file to read.</param>
        /// <param name="numLines">
        /// Number of lines to read from the beginning of the file (default = 2).
        /// If the file contains fewer lines, only the available lines are returned.
        /// </param>
        /// <returns>
        /// An array of strings containing up to <paramref name="numLines"/> lines,
        /// in the order they appear in the file.
        /// </returns>
        /// <remarks>
        /// The file is opened with <see cref="FileShare.ReadWrite"/> so it will not block
        /// other processes from writing to or deleting the file while being read.  
        /// If the file is deleted or becomes unavailable during reading, only the lines
        /// successfully read so far are returned.
        /// </remarks>
        public static async Task<string[]> PeekFileAsync(string lsPath, int numLines = 2)
        {
            Assert.IsNotNull(lsPath, nameof(lsPath));
            if (numLines <= 0)
                throw new ArgumentException(nameof(numLines));
            string[] lines = new string[numLines];
            int i = 0;
            try
            {
                using (FileStream fwStream = new FileStream(
                    lsPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite,
                    bufferSize: 4096, useAsync: true))
                using (StreamReader reader = new StreamReader(fwStream))
                {
                    while (i < numLines && !reader.EndOfStream)
                    {
                        string line = await reader.ReadLineAsync();
                        if (line == null) break;
                        lines[i++] = line;
                    }
                }
            } catch (FileNotFoundException) { 
            } catch (IOException) { }
            if (i < numLines)
                Array.Resize(ref lines, i);
            return lines;
        }
    }
}