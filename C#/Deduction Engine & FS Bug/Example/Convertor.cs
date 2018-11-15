using System.Linq;
using DevUtil.Dev_Testing.TesterSuite.Tester;

namespace Example
{
    public class TesterConvertor
    {
        public static int[,] ToInferenceStructure(TesterBase tester)
        {
            if (tester == null || tester.Count == 0)
                return new int[0, 0];

            var columns = tester.First().Setting.Length + 1; // Add result structure.
            var inferenceStructure = new int[tester.Count, columns];

            var rowsParsed = 0;
            foreach (var testResult in tester)
            {
                inferenceStructure[rowsParsed, 0] = testResult.ErrorOccured ? 0 : 1;

                var columnsParsed = 1;

                foreach (var testSetting in testResult.Setting)
                {
                    inferenceStructure[rowsParsed, columnsParsed] = int.Parse(testSetting.ToString());
                    columnsParsed++;
                }

                rowsParsed++;
            }

            return inferenceStructure;
        }
    }
}