namespace DevUtil.Dev_Testing.TesterSuite.InferenceEngine.Micro_InferenceEngine
{
    public sealed partial class MicroInferenceEngine
    {
        public class CVOperation
        {
            public static bool Execute(int leftColumn, int value, int[,] MIEData, int MIEDataRowSelected)
            {
                var data = MIEData[MIEDataRowSelected, leftColumn + 1];
                var val = value;
                return MIEData[MIEDataRowSelected, leftColumn + 1] == value;
            }

            public static string String(int leftColumn, int value)
                => $"C{leftColumn + 1}=={value}";
        }
    }
}