using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DevUtil.Aspects.Contracts;
using DevUtil.Aspects.Services.Remoting_MiddleWare;
using ARNB = DevUtil.Dev_Testing.CompatibilityTesters.IO.FileRead.Async_Read.NonBuffered;
using SR = DevUtil.Dev_Testing.CompatibilityTesters.IO.FileRead.Sync_Read;
using DevUtil.Dev_Testing.TesterSuite.InferenceEngine.Micro_InferenceEngine;
using DevUtil.Dev_Testing.TesterSuite.Tester;
using Util.IO_Services;
using System.Linq;
using DevUtil.Dev_Testing.CompatibilityTesters.IO.FileRead.Async_Read;
using Util.FileSystem_Services;

namespace Example
{
    public class Program
    {
        public void Run(string[] args)
        {
            #region InputCheck
#if DEBUG
            if (args == null || args.Length == 0)
                args = new[] { @"S:\" };
#endif
            if (args == null || args.Length == 0)
            {
                Console.WriteLine("Please call by providing the input directory.");
                Environment.Exit(0);
            }

            DirectoryInfo dir = null;
            try
            {
                dir = new DirectoryInfo(args[0]);
            }
            catch
            {
                Console.WriteLine($"Could not turn {args[0]} into a {nameof(DirectoryInfo)} object.");
                Environment.Exit(0);
            }
            #endregion

            #region CheckContracts
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var brokenReliances = StateContractManager.GetBrokenReliances(assembly);
                if (brokenReliances.Count > 0)
                    foreach (var brokenReliance in brokenReliances)
                        Console.WriteLine($"Broken Reliance: {brokenReliance.reliantClass}");
            }
            #endregion

            // Initialize.

            //var RO = new RO();
            //await RO.StartLocationStore();
            //await RO.RegisterService(new MicroInferenceEngine());
            //var MIE = RO.GetRemoteObject<MicroInferenceEngine>();
            var MIE = new MicroInferenceEngine();

            if (MIE == null)
            {
                Console.WriteLine($"Failed to create {nameof(MIE)}.");
                Console.ReadLine();
                Environment.Exit(0);
            }

            var set1 = new[] { 510, 511, 512, 513, 514 };
            var set2 = new[] { 1022, 1023, 1024, 1025, 1026 };
            var set3 = new[] { 4094, 4095, 4096, 4097, 4098 };
            var set4 = new[] { 8190, 8191, 8192, 8193, 8194 };
            var setALL = new[] { 510, 511, 512, 513, 514, 1022, 1023, 1024, 1025, 1026, 4094, 4095, 4096, 4097, 4098, 8190, 8191, 8192, 8193, 8194 };
            RunAndPrintDirTest<ARNB.SyncNonBufferedFlagsTester>("ARead with Sync NonBuff flag:", MIE, dir, 666, setALL);
            //RunAndPrintDirTest<ARNB.AsyncNonBufferedFlagsTester>("ARead with ASync NonBuff flag:", MIE, dir, 666, setALL);

            RunAndPrintDirTest<ARNB.AsyncNonBufferedFlagsTester>("ARead with Async NonBuff flag:", MIE, dir, 666, set1);
            RunAndPrintDirTest<ARNB.AsyncNonBufferedFlagsTester>("ARead with Async NonBuff flag:", MIE, dir, 666, set2);
            RunAndPrintDirTest<ARNB.AsyncNonBufferedFlagsTester>("ARead with Async NonBuff flag:", MIE, dir, 666, set3);
            RunAndPrintDirTest<ARNB.AsyncNonBufferedFlagsTester>("ARead with Async NonBuff flag:", MIE, dir, 666, set4);

            RunAndPrintDirTest<SR.SyncNonBufferedFlagsTester>("SRead with Sync NonBuff flag:", MIE, dir, 666, set1);
            RunAndPrintDirTest<SR.SyncNonBufferedFlagsTester>("SRead with Sync NonBuff flag:", MIE, dir, 666, set2);
            RunAndPrintDirTest<SR.SyncNonBufferedFlagsTester>("SRead with Sync NonBuff flag:", MIE, dir, 666, set3);
            RunAndPrintDirTest<SR.SyncNonBufferedFlagsTester>("SRead with Sync NonBuff flag:", MIE, dir, 666, set4);
            Console.ReadLine();
        }

        private static void RunAndPrintDirTest<T>(string message, MicroInferenceEngine ie, DirectoryInfo dir, int fileSize, params int[] fileSizes) where T : TesterBase
        {
            object[] args = { dir, fileSize, fileSizes };
            var o = Activator.CreateInstance(typeof(T), args) as TesterBase;
            Console.WriteLine("* " + message);
                Console.WriteLine(ie.Infer(TesterConvertor.ToInferenceStructure(o),false, false));
        }

        private static void Main(string[] args)
            => new Program().Run(args);
    }
}