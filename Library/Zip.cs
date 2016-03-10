using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace Library
{
    public class Zip
    {

        /// <summary>
        /// Extracts an zip file to a specified output folder using the ICSharpCode.SharpZipLib 
        /// NB. Empty folders in the zip aren't extracted
        /// </summary>
        /// <param name="archiveFilenameIn">The zip file to extract</param>
        /// <param name="outFolder">The folder to put the file in. The folder is created if it doesnt exist</param>
        public static IEnumerable<string> ExtractZipFile(string archiveFilenameIn, string outFolder)
        {
            var files = new List<string>();
            ZipFile zipFile = null;
            try
            {
                var fileStream = File.OpenRead(archiveFilenameIn);
                zipFile = new ZipFile(fileStream);
                foreach (ZipEntry zipEntry in zipFile)
                {
                    // Ignore directories
                    if (zipEntry.IsFile == false)
                        continue;

                    var entryFileName = zipEntry.Name;
                    files.Add(entryFileName);

                    var buffer = new byte[2048];
                    var zipStream = zipFile.GetInputStream(zipEntry);

                    var fullZipToPath = Path.Combine(outFolder, entryFileName);
                    var directoryName = Path.GetDirectoryName(fullZipToPath);

                    if (string.IsNullOrEmpty(directoryName) == false)
                        Directory.CreateDirectory(directoryName);

                    // Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
                    // of the file, but does not waste memory.
                    // The "using" will close the stream even if an exception occurs.
                    using (var streamWriter = File.Create(fullZipToPath))
                    {
                        StreamUtils.Copy(zipStream, streamWriter, buffer);
                    }
                }
            }
            finally
            {
                if (zipFile != null)
                {
                    zipFile.IsStreamOwner = true; // Makes close also shut the underlying stream
                    zipFile.Close(); // Ensure we release resources
                }
            }
            return files;
        }
    }
}
