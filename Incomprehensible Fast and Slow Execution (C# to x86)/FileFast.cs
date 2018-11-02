using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace StopwatchInconsistentResults
{
    class StopwatchInconsistentResults
    {
        static void Main()
        {
            Object[] TestArray = MakeTestArray(100000000);

            long start = Stopwatch.GetTimestamp();
            int sum = CastAndCalculateSum(TestArray);
            long stop = Stopwatch.GetTimestamp();
            long ElapsedTicks = stop - start;

            StringBuilder output = new StringBuilder();

            output.AppendLine("Sum " + sum);
            output.AppendLine("Ticks " + (stop - start));
            output.AppendLine("Ticks " + (ElapsedTicks));
            output.AppendLine("Stopwatch/QPC is bad!!!"); // speed toogle
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