using System.IO;
using Util.FileSystem_Services;

namespace DevUtil.Dev_Testing.CompatibilityTesters.IO.FileRead.Sync_Read
{
    public class SyncNonBufferedFlagsTester : SyncTester
    {
        public SyncNonBufferedFlagsTester(DirectoryInfo testDir, int fileSize, params  int[] fileSizes) : base(testDir, FileOptions.Asynchronous | (FileOptions)DriveInfoUnmanaged.FileFlagNoBuffering, fileSize, fileSizes)
        {
        }
    }
}