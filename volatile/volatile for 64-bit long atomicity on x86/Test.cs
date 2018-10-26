using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;

class Example
{
    readonly Func<long> _volatileLongReadInvoke;
    volatile bool _runAtomicTest = true;

    long _64BitWideValue;
    int _64BitWideValueCreatorCounter;
    int _highUnEqualLowOccurences;

    enum TestUsing
    {
        Assignment,
        VolatileClass,
        ThreadVolatile,
        Volatile
    }

    public Example()
    {
        DynamicMethod volatileLongRead = new DynamicMethod(nameof(volatileLongRead), 
            typeof(long), new[] { typeof(Example) }, typeof(Example));
        var volatileLongReadField = typeof(Example).GetField(nameof(_64BitWideValue),
            BindingFlags.NonPublic | BindingFlags.Instance);

        if (volatileLongReadField == null)
        {
            Console.WriteLine($"Reflection on {nameof(_64BitWideValue)} for {nameof(volatileLongReadField)} failed");
            Environment.Exit(0);
        }

        var il = volatileLongRead.GetILGenerator();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Volatile);
        il.Emit(OpCodes.Ldfld, volatileLongReadField);
        il.Emit(OpCodes.Ret);

        _volatileLongReadInvoke = (Func<long>)volatileLongRead.CreateDelegate(typeof(Func<long>), this);
    }

    void Run(int testRuns, int printPer, TestUsing testUsing)
    {
        var testsDoneCounter = 0;

        var sw = new Stopwatch();
        sw.Start();

        var Task1 = new Task(() =>
        {
            while (_runAtomicTest && _64BitWideValueCreatorCounter < testRuns)
            {
                _64BitWideValueCreatorCounter++;
                long new64BitWideValue = _64BitWideValueCreatorCounter;
                new64BitWideValue = (new64BitWideValue << 32);
                new64BitWideValue = new64BitWideValue | (long)_64BitWideValueCreatorCounter;

                Volatile.Write(ref _64BitWideValue, new64BitWideValue);

                testsDoneCounter++;
                if (testsDoneCounter % printPer == 0)
                    PrintResults(_highUnEqualLowOccurences, sw, testUsing);
            }
            StopRunning();
        });

        var Task2 = new Task(() =>
        {
            while (_runAtomicTest)
            {
                long _64BitWideValueCopy = 0;
                if (testUsing == TestUsing.Assignment)
                    _64BitWideValueCopy = _64BitWideValue;
                if (testUsing == TestUsing.VolatileClass)
                    _64BitWideValueCopy = Volatile.Read(ref _64BitWideValue);
                if (testUsing == TestUsing.ThreadVolatile)
                    _64BitWideValueCopy = Thread.VolatileRead(ref _64BitWideValue);
                if (testUsing == TestUsing.Volatile)
                    _64BitWideValueCopy = _volatileLongReadInvoke();

                int read32BitWideHighValue = (int)(_64BitWideValueCopy & int.MaxValue);
                int read32BitWideLowValue = (int)(_64BitWideValueCopy >> 32);
                if (read32BitWideHighValue != read32BitWideLowValue)
                {
                    _highUnEqualLowOccurences++;
                    StopRunning();
                }
            }
        });

        Task1.Start();
        Task2.Start();
        Task.WaitAll(Task1, Task2);

        sw.Stop();

        PrintResults(_highUnEqualLowOccurences, sw, testUsing);
    }

    void StopRunning()
    {
        _runAtomicTest = false;
        Thread.MemoryBarrier();
    }

    void PrintResults(int highUnEqualLowOccurences, Stopwatch sw, TestUsing testUsing)
    {
        Console.WriteLine($"Found {highUnEqualLowOccurences} occurence in {GetRunTime(sw)} using {testUsing}");
    }

    string GetRunTime(Stopwatch sw)
    {
        return (sw.Elapsed.TotalMilliseconds < 60000.0)
            ? string.Format("{0:0.#}", sw.Elapsed.TotalMilliseconds / 1000) + " seconds"
            : string.Format("{0:0.##}", sw.Elapsed.TotalMilliseconds / 60000) + " minutes";
    }

    static void Main(string[] args)
    {
        if (args.Length != 3 || !typeof(TestUsing).IsEnumDefined(int.Parse(args[2])))
        {
            Console.WriteLine("Call with test-runs, print-per, and test-using arguments.");
            Console.WriteLine($"Test-using can be {TestUsing.Assignment.ToString()}({(int)TestUsing.Assignment}), " +
                              $"{TestUsing.VolatileClass.ToString()}({(int)TestUsing.VolatileClass}), " +
                              $"{TestUsing.ThreadVolatile.ToString()}({(int)TestUsing.ThreadVolatile}), " +
                $"{TestUsing.Volatile.ToString()}({(int)TestUsing.Volatile})");
            Environment.Exit(1);
        }

        var p = new Example();
        p.Run(int.Parse(args[0]), int.Parse(args[1]), (TestUsing)int.Parse(args[2]));
    }
}