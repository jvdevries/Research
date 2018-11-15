using System.IO;
using Util.FileSystem_Services;

namespace DevUtil.Dev_Testing.CompatibilityTesters.IO.FileRead.Async_Read.NonBuffered
{
    public class SyncNonBufferedFlagsTester : AsyncTester
    {
        public SyncNonBufferedFlagsTester(DirectoryInfo testDir, int fileSize, params int[] fileSizes) : base(testDir, (FileOptions) DriveInfoUnmanaged.FileFlagNoBuffering, fileSize, fileSizes)
        {
        }
    }
}