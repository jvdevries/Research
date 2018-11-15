using System.IO;
using Util.FileSystem_Services;

namespace DevUtil.Dev_Testing.CompatibilityTesters.IO.FileRead.Async_Read.NonBuffered
{
    public class AsyncNonBufferedFlagsTester : AsyncTester
    {
        public AsyncNonBufferedFlagsTester(DirectoryInfo testDir, int fileSize, params int[] fileSizes) : base(testDir, FileOptions.Asynchronous | (FileOptions)DriveInfoUnmanaged.FileFlagNoBuffering, fileSize, fileSizes)
        {
        }
    }
}