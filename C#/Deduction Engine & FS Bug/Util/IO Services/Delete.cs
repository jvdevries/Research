using System.IO;
using Util.Process_Services;

namespace Util.IO_Services
{
    public class Delete
    {
        /// <summary>
        /// Tries to delete files.
        /// </summary>
        /// <param name="files">The files to delete.</param>
        public static void TryFiles(params FileInfo[] files)
        {
            foreach (var file in files)
                Try.Do(() => File.Delete(file.FullName));
        }
    }
}