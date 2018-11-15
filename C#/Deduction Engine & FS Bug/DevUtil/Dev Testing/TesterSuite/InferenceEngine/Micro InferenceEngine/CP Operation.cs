using System;

namespace DevUtil.Dev_Testing.TesterSuite.InferenceEngine.Micro_InferenceEngine
{
    public sealed partial class MicroInferenceEngine
    {
        public struct CPOperation
        {
            // Other classes require from these operations:
            // - They are numbered from 0 to N, with each whole number from 0 to N being present.
            // - N is correctly set in an accessible counter.
            private const int SmallerEqual = 0;
            private const int Smaller = 1;
            private const int Equal = 2;
            private const int Greater = 3;
            private const int GreaterEqual = 4;
            private const int UnEqual = 5;
            public const int OpsLength = 6;

            public static bool Execute(int leftColumn, int intraOperand, int rightColumn, int[,] MIEData, int MIEDataRowSelected)
            {
                switch (intraOperand)
                {
                    case SmallerEqual:
                        return MIEData[MIEDataRowSelected, leftColumn + 1] <= MIEData[MIEDataRowSelected, rightColumn + 1];
                    case Smaller:
                        return MIEData[MIEDataRowSelected, leftColumn + 1] < MIEData[MIEDataRowSelected, rightColumn + 1];
                    case Equal:
                        return MIEData[MIEDataRowSelected, leftColumn + 1] == MIEData[MIEDataRowSelected, rightColumn + 1];
                    case Greater:
                        return MIEData[MIEDataRowSelected, leftColumn + 1] > MIEData[MIEDataRowSelected, rightColumn + 1];
                    case GreaterEqual:
                        return MIEData[MIEDataRowSelected, leftColumn + 1] >= MIEData[MIEDataRowSelected, rightColumn + 1];
                    case UnEqual:
                        return MIEData[MIEDataRowSelected, leftColumn + 1] != MIEData[MIEDataRowSelected, rightColumn + 1];
                    default:
                        throw new InvalidOperationException();
                }
            }

            public static string ToString(int leftColumn, int intraOperand, int rightColumn)
                => $"C{leftColumn + 1}{OperationToString(intraOperand)}C{rightColumn + 1}";

            private static string OperationToString(int operation)
            {
                switch (operation)
                {
                    case SmallerEqual:
                        return "<=";
                    case Smaller:
                        return " <";
                    case Equal:
                        return "==";
                    case Greater:
                        return "> ";
                    case GreaterEqual:
                        return ">=";
                    case UnEqual:
                        return "!=";
                    default:
                        throw new InvalidOperationException();
                }
            }
        }
    }
}