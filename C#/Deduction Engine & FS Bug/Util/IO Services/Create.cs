using System;
using System.IO;
using Util.Process_Services;
using Util.Random;

namespace Util.IO_Services
{
    public class Create
    {
        /// <summary>
        /// Create a New File with a <see cref="Guid"/> filename in the specified location with the specified size.
        /// </summary>
        /// <param name="dir">Location to place the file.</param>
        /// <param name="size">Size of the file.</param>
        /// <param name="fillRandomly">Write random data to the file.</param>
        /// <param name="bufferWriteSize">Amount of bytes to write (until file has achieved <see cref="size"/>).</param>
        /// <param name="maxTries">Maximum number of tries to write the file.</param>
        /// <returns>With a <see cref="FileInfo"/> for the file, a null in case the file could not be made.</returns>
        public static FileInfo NewFileTry(DirectoryInfo dir, long size, bool fillRandomly = false,
            int bufferWriteSize = 65536, int maxTries = 1)
        {
            if (dir == null)
                return null;

            while (maxTries != 0)
            {
                try
                {
                    var file = Path.Combine(dir.FullName, Guid.NewGuid().ToString());

                    var random = new RandomHelper();
                    using (var fs = new FileStream(file, FileMode.CreateNew))
                        // If we get here, the file was created by us.
                        try
                        {
                            if (fillRandomly == false)
                                fs.SetLength(size);
                            else
                            {
                                var totalBytesWritten = 0;
                                while (totalBytesWritten < size)
                                {
                                    if (totalBytesWritten + bufferWriteSize > size)
                                        bufferWriteSize = (int) (size - totalBytesWritten);

                                    var randomData = random.GetBytes(bufferWriteSize);
                                    fs.Write(randomData, 0, bufferWriteSize);

                                    totalBytesWritten = totalBytesWritten + bufferWriteSize;
                                }
                            }

                            return new FileInfo(file);
                        }
                        catch
                        {
                            maxTries--;
                            Try.Do(() =>
                            {
                                // ReSharper disable once AccessToDisposedClosure // Close before Delete...
                                fs.Close();
                                File.Delete(file);
                            });
                        }
                }
                catch (UnauthorizedAccessException)
                {
                    break;
                } // Can't recover from UAE.
                catch (IOException)
                {
                } // File already exists.
                catch (Exception)
                {
                    maxTries--;
                }
            }

            return null;
        }
    }
}