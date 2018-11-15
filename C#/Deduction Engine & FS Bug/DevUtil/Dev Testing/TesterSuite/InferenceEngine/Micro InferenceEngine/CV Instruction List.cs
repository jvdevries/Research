using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Util.Maths_Services;
using static DevUtil.Dev_Testing.TesterSuite.InferenceEngine.Micro_InferenceEngine.MicroInferenceEngine;

namespace DevUtil.Dev_Testing.TesterSuite.InferenceEngine.Micro_InferenceEngine
{
    public sealed partial class MicroInferenceEngine
    {
        /// <summary>
        /// Generates & Executes ColumnValue Instruction List's. A CVIL is encoded as a stream of integers.
        /// Data-Format of the stream: [Column-Number][Value]([Inter-Operator][Column-Number][Value])*
        /// Where [] is represented by an integer, and an Inter-Operator is either && or ||.
        /// </summary>
        public class CVInstructionList
        {
            /// <summary>
            /// Generate all possible unique CVIL's using <see cref="MicroInferenceEngine"/> data.
            /// </summary>
            /// <param name="rows">The number of rows in the <see cref="MicroInferenceEngine"/> data.</param>
            /// <param name="columns">The number of columns (minus the result column) in the <see cref="MicroInferenceEngine"/> data</param>
            /// <param name="data">The <see cref="MicroInferenceEngine"/> data, with the result column first.</param>
            /// <returns>An <see cref="IEnumerator"/> yielding a stream of integers, representing a CVIL.</returns>
            public static IEnumerator<IReadOnlyList<int>> Generate(int rows, int columns, int[,] data)
            {
                var CILsSerialized = new HashSet<string>();
                var CVILsSerialized = new HashSet<string>();

                var allValueCombinations =
                    GetCartesianProductOfValues(data, columns,
                        rows); // C1:{A,B} C2:{X,Y} => {A,X}, {A,Y}, {B,X}, {B,Y},
                var allCILInputPatterns =
                    PermutateInt.Get(2, columns, columns, true, true); // C1, C2 => {0,0}, {0,1}, {1,0}, {1,1}

                // Form column-order-patterns, such as {C1,C2,C3}.
                // Note: a CIL such as C1&&(C2||C3) is formed from {C2,C3,C1}.
                var COPatterns = PermutateInt.GetEnum(columns, 0, columns - 1, false, true);
                while (COPatterns.MoveNext())
                {
                    var COPattern = COPatterns.Current;

                    // Form column-instruction-lists (CILs), such as C1 || C2 && C3.
                    var interCPatterns = PermutateInt.GetEnum(ColumnXInterOp.OpsLength,
                        COPattern.Count - 1, COPattern.Count - 1,
                        true, true);
                    while (interCPatterns.MoveNext())
                    {
                        var interCPattern = interCPatterns.Current;

                        // Form CVIL when non-duplicate CIL.
                        var allCILResults = GatherCILOutcomes(COPattern, interCPattern, allCILInputPatterns);
                        if (!CILsSerialized.Contains(allCILResults))
                        {
                            CILsSerialized.Add(allCILResults);

                            // Form CVIL by inserting all possible values to the CIL.
                            for (var k = 0; k < allValueCombinations.GetLength(0); k++)
                            {
                                var CVIL = new List<int>();
                                for (var i = 0; i < COPattern.Count; i++)
                                {
                                    if (i != 0)
                                        CVIL.Add(interCPattern[i - 1]);
                                    CVIL.Add(COPattern[i]);
                                    CVIL.Add(allValueCombinations[k, COPattern[i]]);
                                }

                                // Prevent duplicates.
                                var sb = new StringBuilder();
                                foreach (var instruction in CVIL)
                                    sb.Append(instruction + ",");

                                if (!CVILsSerialized.Contains(sb.ToString()))
                                {
                                    CVILsSerialized.Add(sb.ToString());
                                    yield return CVIL;
                                }
                            }
                        }
                    }
                }
            }

            public static string String(IReadOnlyList<int> IL)
            {
                if (IL.Count == 0)
                    return string.Empty;

                var sb = new StringBuilder();
                sb.Append(CVOperation.String(IL[0], IL[1]));
                var readIndex = 2;
                while (readIndex < IL.Count)
                {
                    sb.Append($" {ColumnXInterOp.String(IL[readIndex])}");
                    sb.Append($" {CVOperation.String(IL[readIndex + 1], IL[readIndex + 2])}");
                    readIndex = readIndex + 3;
                }

                return sb.ToString();
            }

            private static string GatherCILOutcomes(IReadOnlyList<int> COPattern, IReadOnlyList<int> interCPattern,
                IEnumerable<IReadOnlyList<int>> allCILInputPatterns)
            {
                var sb = new StringBuilder();

                // Generate all possible truth tables for the CIL.
                foreach (var input in allCILInputPatterns)
                {
                    var result = input[COPattern[0]] == 1;

                    for (var i = 0; i < interCPattern.Count; i++)
                        result = ColumnXInterOp.ExecuteInter(result, interCPattern[i],
                            input[COPattern[i + 1]] == 1);

                    sb.Append(result.ToString().Substring(0, 1));
                }

                return sb.ToString();
            }
             
            private static int[,] GetCartesianProductOfValues(int[,] MIEData, int columns, int rows)
            {
                var columnValues = new List<int>[columns];
                for (var i = 0; i < columns; i++)
                    columnValues[i] = new List<int>();

                for (var j = 0; j < rows; j++)
                for (var i = 0; i < columns; i++)
                    if (!columnValues[i].Contains(MIEData[j, i + 1]))
                        columnValues[i].Add(MIEData[j, i + 1]);

                return CartesianProduct<int>.Get2DArray(columnValues);
            }

            public static bool Execute(IReadOnlyList<int> IL, int[,] MIEData, int MIEDataSelectedRow)
            {
                if (IL.Count == 0)
                    return true;

                var result = CVOperation.Execute(IL[0], IL[1], MIEData, MIEDataSelectedRow);
                var readIndex = 2;
                while (readIndex < IL.Count)
                {
                    var tempResult = CVOperation.Execute(IL[readIndex + 1], IL[readIndex + 2], MIEData, MIEDataSelectedRow);
                    result = ColumnXInterOp.ExecuteInter(result, IL[readIndex], tempResult);
                    readIndex = readIndex + 3;
                }

                return result;
            }
        }
    }
}