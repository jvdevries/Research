using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DevUtil.Dev_Testing.TesterSuite.Tester;
using DevUtil.Dev_Testing.TesterSuite.Tester.DataStores;
using Util.DataType.Space;
using Util.FileSystem_Services;
using Util.IO_Services;

namespace DevUtil.Dev_Testing.CompatibilityTesters.IO.FileRead.Async_Read
{
    public class AsyncTester : TesterBase
    {
        public AsyncTester(DirectoryInfo testDir, FileOptions flag, int fileSize, params int[] fileSizes) : base(GetTestDimension(testDir, fileSize, fileSizes), GetTest(testDir, flag))
        {
        }

        private static TestDimension GetTestDimension(DirectoryInfo testDir, int fileSize, params int[] fileSizes)
        {
            DriveInfoUnmanaged.GetSectorInfo(new DriveLetter(testDir.Root.Name), out var dummy,
                out var bytesPerPhysicalSector);


            var BPS = bytesPerPhysicalSector; // Alias

            var fileSizeAxis = Axis.Create("File Size", fileSize, fileSizes);

            var fsBufferSizeAxis = Axis.Create("FileStream Buffer", 0, BPS - 2, BPS - 1, BPS, BPS + 1, BPS + 2);

            var rBufferSizeAxis = Axis.Create("Read Buffer", 0, BPS - 2, BPS - 1, BPS, BPS + 1, BPS + 2);

            return new TestDimension(fileSizeAxis, fsBufferSizeAxis, rBufferSizeAxis);
        }

        private static Test GetTest(DirectoryInfo testDir, FileOptions flags)
        {
            async Task<Exception> AsyncRead(int fileSizeVar, int fsBufferSizeVar, int rBufferSizeVar)
            {
                var readBuffer = new byte[rBufferSizeVar];

                var testFileInfo = Create.NewFileTry(testDir, fileSizeVar, true);
                var testFileCopyInfo = new FileInfo(testFileInfo.FullName + "COPY");

                try
                {
                    using (var testFile = new FileStream(testFileInfo.FullName, FileMode.Open, FileAccess.Read,
                        FileShare.Read, fsBufferSizeVar, flags))
                    using (var testCopy = new FileStream(testFileCopyInfo.FullName, FileMode.Create, FileAccess.Write,
                        FileShare.None))
                    {
                        int bytesRedThisPass; // Deliberate spelling mistake on past tense of read.
                        var bytesToRead = fileSizeVar;
                        do
                        {
                            var bytesToReadThisPass = readBuffer.Length;
                            if (bytesToRead < bytesToReadThisPass)
                                bytesToReadThisPass = bytesToRead;

                            bytesRedThisPass = await testFile.ReadAsync(readBuffer, 0, bytesToReadThisPass);

                            bytesToRead = bytesToRead - bytesRedThisPass;

                            testCopy.Write(readBuffer, 0, bytesRedThisPass);
                        } while (bytesRedThisPass != 0);
                    }

                    var filesMatch = File.ReadAllBytes(testFileInfo.FullName)
                        .SequenceEqual(File.ReadAllBytes(testFileCopyInfo.FullName));

                    Delete.TryFiles(testFileInfo, testFileCopyInfo);

                    if (!filesMatch)
                        throw new IOException("Files do not match.");
                }
                catch (Exception e)
                {
                    Delete.TryFiles(testFileInfo, testFileCopyInfo);
                    return e;
                }

                return null;
            }

            return Test.Create((Func<int, int, int, Task<Exception>>) AsyncRead);
        }
    }
}