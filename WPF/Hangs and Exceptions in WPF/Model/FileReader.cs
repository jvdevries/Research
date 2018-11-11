using System;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace Model
{
    /// <summary>
    /// Provides readers for reading setuperr.log in %WINDIR%.
    /// It is surmised that this file always exists and is empty (YMMV).
    /// </summary>
    public class FileReader
    {
        public static string SetupErrorFullFileName =
            Environment.GetEnvironmentVariable("windir") + Path.DirectorySeparatorChar + "setuperr.log";
        private readonly int _delayTime = 150;

        public FileReader(int delayValue)
        {
            _delayTime = delayValue;
        }

        public async Task<string> CallUnawaited()
            => new Func<Task<string>>(async () =>
            {
                using (var stream = File.OpenRead(SetupErrorFullFileName))
                using (var reader = new StreamReader(stream))
                {
                    var t = reader.ReadToEndAsync();
                    await t;
                    var r = t.Result;
                    return r;
                }
            }).Invoke().Result;

        public async Task<string> CallAwaited()
        {
            using (var stream = File.OpenRead(SetupErrorFullFileName))
            using (var reader = new StreamReader(stream))
            {
                var t = reader.ReadToEndAsync();
                await t;
                var r = t.Result;
                return r;
            }
        }
    }
}