using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

namespace StopwatchInconsistentResults
{
    class StopwatchInconsistentResults
    {
		[DllImport("KERNEL32")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

		static void Main()
        {
			var Proc = Process.GetCurrentProcess();
			Proc.ProcessorAffinity = (IntPtr)1;

			Object[] TestArray = MakeTestArray(100000000);

            QueryPerformanceCounter(out long start);
            int sum = CastAndCalculateSum(TestArray);
			QueryPerformanceCounter(out long stop);
            long ElapsedTicks = stop - start;

            StringBuilder output = new StringBuilder();

            output.AppendLine("Sum " + sum);
            output.AppendLine("Ticks " + (stop - start));

            File.WriteAllText(@"file.txt", output.ToString());
        }

        static Object[] MakeTestArray(int arrSize)
        {
            Object[] arr = new Object[arrSize];

            for (int i = 0; i < (arr.Length - 2); i = i + 3)
            {
                arr[i] = 1;
                arr[i + 1] = null;
                arr[i + 2] = null;
            }

            return arr;
        }

        static int CastAndCalculateSum(object[] arr)
        {
            int sum = 0;
            foreach (object o in arr)
            {
                if (o is int)
                {
                    int x = (int)o;
                    sum += x;
                }
            }
            return sum;
        }
    }
}