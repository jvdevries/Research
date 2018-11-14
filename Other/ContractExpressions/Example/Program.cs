using System;
using System.Threading.Tasks;
using DevUtil.Aspects.Contracts;
using Util.DataType.Space;

namespace Example
{
    [ReliesOnStateContract(typeof(Dimension), "000102030405060708090A0B0C0D0E0F")]
    public class Program
    {
        public void Run()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var brokenReliances = StateContractManager.GetBrokenReliances(assembly);
                if (brokenReliances.Count > 0)
                    foreach (var brokenReliance in brokenReliances)
                        Console.WriteLine($"Broken Reliance: {brokenReliance.reliantClass} Hash: {brokenReliance.contractHash}");
            }

            Console.ReadLine();
        }

        private static void Main(string[] args)
            => new Program().Run();
    }
}