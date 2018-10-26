using System;
using System.Diagnostics;
using System.Threading;

public class Program
{
    int X, Y, r1, r2, r3, r4 = 0;
    volatile int start = 0;

    void Thread1()
    {
        while (start == 0) ;    // Wait for Thread 2 to start, or
                                // we will end before it starts.

        Volatile.Write(ref X, 1);
        Volatile.Write(ref r1, Volatile.Read(ref X));
        Volatile.Write(ref r2, Volatile.Read(ref Y));
    }

    void Thread2()
    {
        start = 1;
        Thread.MemoryBarrier(); // Ensure Thread 1 can see the 
                                // start before executing the 
                                // next instructions.

        Volatile.Write(ref Y, 1);
        Volatile.Write(ref r3, Volatile.Read(ref Y));
        Volatile.Write(ref r4, Volatile.Read(ref X));
    }

    private void Run(int maxTries, int printPer)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        for (int runsCounter = 1; runsCounter < maxTries; runsCounter++)
        {
            r1 = r2 = r3 = r4 = X = Y = 0;
            start = 0;
            Thread.MemoryBarrier(); // Ensures other Threads
                                    // see that start is 0.

            Thread t1 = new Thread(Thread1);
            Thread t2 = new Thread(Thread2);

            t1.Start();
            t2.Start();

            while (t1.IsAlive || t2.IsAlive) ;

            if (r2 == 0 && r4 == 0)
                OccurenceFoundExit(runsCounter, sw);

            t1.Join();
            t2.Join();

            if (runsCounter % printPer == 0)
                PrintRunTime(runsCounter, sw);
        }
        sw.Stop();
        OccurenceNotFoundExit(maxTries, sw);
    }

    void OccurenceFoundExit(int runs, Stopwatch sw)
    {
        Console.WriteLine("Found occurence after " + runs + " runs, which took " + GetRunTime(runs, sw));
        Environment.Exit(1);
    }

    void OccurenceNotFoundExit(int runs, Stopwatch sw)
    {
        Console.WriteLine("Occurence not found after " + runs + " runs, which took " + GetRunTime(runs, sw));
        Environment.Exit(1);
    }

    void PrintRunTime(int runs, Stopwatch sw)
    {
        Console.WriteLine("Done " + runs + " runs, which took " + GetRunTime(runs, sw));
    }

    string GetRunTime(int runs, Stopwatch sw)
    {
        return (sw.Elapsed.TotalMilliseconds < 60000.0)
            ? string.Format("{0:0.#}", sw.Elapsed.TotalMilliseconds / 1000) + " seconds"
            : string.Format("{0:0.##}", sw.Elapsed.TotalMilliseconds / 60000) + " minutes";
    }

    public static void Main(string[] args)
    {
        Program p = new Program();
        p.Run(int.Parse(args[0]), int.Parse(args[1]));
    }
}