using System;
using System.Diagnostics;

namespace StopwatchInconsistentResults
{
    class StopwatchInconsistentResults
    {
        static void Main()
        {
            Object[] TestArray = MakeTestArray(100000000);

            double freq = Stopwatch.Frequency;
            long start = Stopwatch.GetTimestamp();
            int sum = CastAndCalculateSum(TestArray);
            long stop = Stopwatch.GetTimestamp();
            long ElapsedTicks = stop - start;
            double ElapsedMsec = ElapsedTicks * (1000.0 / freq);

            Console.WriteLine("Sum " + sum);
            Console.WriteLine("Freq " + freq);
            Console.WriteLine("Ticks " + (stop - start));
            Console.WriteLine("Stopwatch/QPC is bad!!!"); // speed toogle
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
                    int x = (int) o;
                    sum += x;
                }
            }
            return sum;
        }
    }
}