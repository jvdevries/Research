using System;

namespace DevUtil.Dev_Testing.TesterSuite.InferenceEngine.Micro_InferenceEngine
{
    public sealed partial class MicroInferenceEngine
    {
        /// <summary>
        /// Provides the inter operation for ColumnValue and ColumnPair types.
        /// </summary>
        public struct ColumnXInterOp
        {
            // Other classes require from these operations (outside NOP):
            // - They are numbered from 0 to N, with each whole number from 0 to N being present.
            // - N is correctly set (NOP is not counted) in an accessible counter.
            public const int Nop = -1;
            private const int Or = 0;
            private const int And = 1;
            public const int OpsLength = 2; // Exclude NOP.


            /// <summary>
            /// Executes the Inter part of a fake <see cref="InterOperation{T}"/>.
            /// </summary>
            public static bool ExecuteInter(bool leftResult, int interOperand, bool rightResult)
            {
                switch (interOperand)
                {
                    case Or:
                        return leftResult || rightResult;
                    case And:
                        return leftResult && rightResult;
                    default:
                        throw new InvalidOperationException();
                }
            }

            public static string String(int interOperand)
            {
                switch (interOperand)
                {
                    case Nop:
                        return "";
                    case Or:
                        return "||";
                    case And:
                        return "&&";
                    default:
                        throw new InvalidOperationException();
                }
            }
        }
    }
}