using System;
using System.Threading.Tasks;

public class Example
{
    volatile bool stop;

	public void LoopTillStop(bool stopBool)
	{
		while (!stopBool) ;
	}

    public static void Main(string[] args)
    {
    	if (args.Length == 0)
    	{
    		Console.Write("Give number of seconds as argument.");
            Environment.Exit(0);
   		}
    
        var e = new Example();

        new Task(() =>
        {
        	// Wait to ensure LoopTillStop is entered.
            Task.Delay(100);
            e.stop = true;
        }).Start();

        new Task(async () =>
        {
            await Task.Delay(int.Parse(args[0]) * 1000);
            Console.WriteLine("Time elapsed, ending program.");
            Environment.Exit(0);
        }).Start();
        
        e.LoopTillStop(e.stop);
    }
}