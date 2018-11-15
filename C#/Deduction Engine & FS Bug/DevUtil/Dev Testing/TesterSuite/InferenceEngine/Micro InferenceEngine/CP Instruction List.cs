using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Util.Maths_Services;
using static DevUtil.Dev_Testing.TesterSuite.InferenceEngine.Micro_InferenceEngine.MicroInferenceEngine;

namespace DevUtil.Dev_Testing.TesterSuite.InferenceEngine.Micro_InferenceEngine
{
    public sealed partial class MicroInferenceEngine
    {
        /// <summary>
        /// Generates & Executes ColumnPair Instruction List's. A CPIL is encoded as a stream of integers.
        /// Data-Format of the stream: [Column-Number][Value][Column-Number]([Inter-Operator][Column-Number][Value][Column-Number])*
        /// Where [] is represented by an integer, and an Inter-Operator is either && or ||.
        /// </summary>
        public class CPInstructionList
        {
            /// <summary>
            /// Generates all possible CPILs.
            /// </summary>
            public static IEnumerator<IReadOnlyList<int>> Generate(int rows, int columns, int[,] MIEData)
            {
                var permutationsCache = PermutateInt.CreateCache();
                var allCPValuePossibilities = GenerateCPValuePossibilities(columns, MIEData);
                var CPILsResultCache = new HashSet<string>();
                var CPOccurceInDataCache = new Dictionary<(int leftColumn, int intraOperand, int rightColumn), bool>();

                // Generate all possible relevant ColumnPairs (CPs).
                // 3 Columns yield 3 CPs: 1 -> {C1,C2}, 2 -> {C1,C3}, 3 -> {C2,C3}.
                var allCPs = PermutateInt.Get(columns, 2, 2, false, false);

                // Generate possible CP patterns (without inter & intra operations).
                // 3 CPs (A,B,C) yield 15 patterns: {A,B,C}, {AB,AC,BA,BC,CA,CB}, {ABC,ACB,BAC,BCA,CAB,CBA}
                var CPPatterns = PermutateInt.GetEnum(allCPs.Count, 0, allCPs.Count, false, true);

                while (CPPatterns.MoveNext())
                {
                    var CPPattern = CPPatterns.Current;

                    // Generate the intra CP patterns.
                    // 1 CP yield 5 possibilities.
                    // 2 CPs yield 5x5 possibilities.
                    var intraCPPatterns = PermutateInt.GetEnum(CPOperation.OpsLength, CPPattern.Count, CPPattern.Count, true, true);

                    while (intraCPPatterns.MoveNext())
                    {
                        var intraCPPattern = intraCPPatterns.Current;

                        if (intraCPPattern.Count - 1 < 1) // No inter CP operation needed.
                        {
                            var skipToNext = false;
                            var CPIL = new List<int>();
                            for (var i = 0; i < intraCPPattern.Count; i++)
                            {
                                // If an impossible CP is present, skip.
                                if (!CPOccursInData(allCPs[CPPattern[i]][0], intraCPPattern[i], allCPs[CPPattern[i]][1], MIEData, rows, CPOccurceInDataCache))
                                {
                                    skipToNext = true;
                                    break;
                                }
                                CPIL.Add(allCPs[CPPattern[i]][0]);
                                CPIL.Add(intraCPPattern[i]);
                                CPIL.Add(allCPs[CPPattern[i]][1]);
                            }
                            if (skipToNext)
                                continue;

                            var serializedCPILResults = SerializeAllPossibleCPILResults(CPIL, allCPValuePossibilities);
                            if (!CPILsResultCache.Contains(serializedCPILResults))
                            {
                                CPILsResultCache.Add(serializedCPILResults);
                                yield return CPIL;
                            }
                        }
                        else
                        {
                            // Generate all patterns for inter CP operations.
                            // 3 CP yields 4 patterns: A&&B&&C, A&&B||C, A||B&&C, A||B||C. 
                            var interCPsPatterns = PermutateInt.Get(ColumnXInterOp.OpsLength, intraCPPattern.Count - 1, intraCPPattern.Count - 1, true, true, permutationsCache);

                            foreach (var interCPsPattern in interCPsPatterns)
                            {
                                var CPIL = new List<int>();

                                // If an impossible CP is present, skip.
                                if (!CPOccursInData(allCPs[CPPattern[0]][0], intraCPPattern[0], allCPs[CPPattern[0]][1], MIEData, rows, CPOccurceInDataCache))
                                    break;

                                CPIL.Add(allCPs[CPPattern[0]][0]);
                                CPIL.Add(intraCPPattern[0]);
                                CPIL.Add(allCPs[CPPattern[0]][1]);

                                var skipToNext = false;
                                for (var i = 1; i < intraCPPattern.Count; i++)
                                {
                                    // If an impossible CP is present, skip.
                                    if (!CPOccursInData(allCPs[CPPattern[i]][0], intraCPPattern[i], allCPs[CPPattern[i]][1], MIEData, rows, CPOccurceInDataCache))
                                    {
                                        skipToNext = true;
                                        break;
                                    }

                                    CPIL.Add(interCPsPattern[i - 1]);
                                    CPIL.Add(allCPs[CPPattern[i]][0]);
                                    CPIL.Add(intraCPPattern[i]);
                                    CPIL.Add(allCPs[CPPattern[i]][1]);
                                }
                                if (skipToNext)
                                    break;
                                
                                var serializedCPILResults = SerializeAllPossibleCPILResults(CPIL, allCPValuePossibilities);
                                if (!CPILsResultCache.Contains(serializedCPILResults))
                                {
                                    CPILsResultCache.Add(serializedCPILResults);
                                    yield return CPIL;
                                }

                            }
                        }
                    }
                }
            }

            private static bool CPOccursInData(int leftColumn, int intraOperand, int rightColumn, int[,] MIEData, int rows, IDictionary<(int, int, int), bool> CPOccurceInDataCache)
            {
                if (CPOccurceInDataCache.ContainsKey((leftColumn, intraOperand, rightColumn)))
                    return CPOccurceInDataCache[(leftColumn,intraOperand,rightColumn)];

                for (var r = 0; r < rows; r++)
                {
                    var resultTrue = CPOperation.Execute(leftColumn, intraOperand, rightColumn, MIEData, r);
                    if (resultTrue)
                    {
                        CPOccurceInDataCache.Add((leftColumn, intraOperand, rightColumn), true);
                        return true;
                    }
                }

                CPOccurceInDataCache.Add((leftColumn, intraOperand, rightColumn), false);
                return false;
            }

            public static bool Execute(IReadOnlyList<int> IL, int[,] MIEData, int MIEDataSelectedRow)
            {
                if (IL.Count == 0)
                    return true;

                var result = CPOperation.Execute(IL[0], IL[1], IL[2], MIEData, MIEDataSelectedRow);
                var readIndex = 3;
                while (readIndex < IL.Count)
                {
                    var tempResult = CPOperation.Execute(IL[readIndex + 1], IL[readIndex + 2], IL[readIndex + 3], MIEData, MIEDataSelectedRow);
                    result = ColumnXInterOp.ExecuteInter(result, IL[readIndex], tempResult);
                    readIndex = readIndex + 4;
                }

                return result;
            }

            public static string String(IReadOnlyList<int> IL)
            {
                if (IL.Count == 0)
                    return string.Empty;

                var sb = new StringBuilder();
                sb.Append(CPOperation.ToString(IL[0], IL[1], IL[2]));
                var readIndex = 3;
                while (readIndex < IL.Count)
                {
                    sb.Append($" {ColumnXInterOp.String(IL[readIndex])}");
                    sb.Append($" {CPOperation.ToString(IL[readIndex + 1], IL[readIndex + 2], IL[readIndex + 3])}");
                    readIndex = readIndex + 4;
                }

                return sb.ToString();
            }

            private static string SerializeAllPossibleCPILResults(IList<int> CPIL, int[,] MIEData)
            {
                var sb = new StringBuilder();
                var rows = MIEData.GetLength(0);
                for (var i = 0; i < rows; i++)
                {
                    var CPILResult = ExecuteCPIL(CPIL, MIEData, i);
                    sb.Append($"{CPILResult.ToString().Substring(0, 1)}-");
                }

                return sb.ToString();
            }

            public static bool ExecuteCPIL(IList<int> CPIL, int[,] MIEData, int MIEDataIndex)
            {
                var readIndex = 0;

                // Initialize the first.
                var result =
                    CPOperation.Execute(CPIL[0], CPIL[1], CPIL[2], MIEData, MIEDataIndex);
                readIndex = readIndex + 3;
                while (readIndex < CPIL.Count)
                {
                    var cp = CPOperation.Execute(CPIL[readIndex + 1], CPIL[readIndex + 2], CPIL[readIndex + 3], MIEData, MIEDataIndex);
                    result = ColumnXInterOp.ExecuteInter(result, CPIL[readIndex], cp);
                    readIndex = readIndex + 4;
                }

                return result;
            }

            /// <summary>
            /// Generates all unique Column possibilities from the CP perspective (<see cref="normalizeCVInput(IReadOnlyList{int})">).
            /// </summary>
            private static int[,] GenerateCPValuePossibilities(int columns, int[,] MIEData)
            {
                var CVPs = new List<IReadOnlyList<int>>();
                var CVPsContains = new HashSet<string>();

                var rawCVPs = PermutateInt.GetEnum(columns, columns, columns, true, true);

                // Reduce raw by normalizing 002 to 001 and eliminating duplicates.
                while (rawCVPs.MoveNext())
                {
                    var rawCVP = rawCVPs.Current;
                    var normalizedCVP = normalizeCVInput(rawCVP);

                    var intArrAsString = string.Join("-", normalizedCVP);
                    if (!CVPsContains.Contains(intArrAsString))
                    {
                        CVPsContains.Add(intArrAsString);
                        CVPs.Add(normalizedCVP);
                    }
                }

                var result = new int[CVPs.Count, columns + 1];
                var CVPindex = 0;
                foreach (var CVP in CVPs)
                {
                    for (var j = 1; j < columns + 1; j++)
                        result[CVPindex, j] = CVP[j - 1];
                    CVPindex++;
                }

                return result;
            }

            /// <summary>
            /// From the CP perspective, Columns are compared to each other.
            /// It is irrelevant if, with 3 Columns, 002 is inputted or 001.
            /// </summary>
            /// <param name="CVPInput"></param>
            /// <returns></returns>
            public static IReadOnlyList<int> normalizeCVInput(IReadOnlyList<int> CVPInput)
            {
                var normalizedCVP = new int[CVPInput.Count];
                var index = 0;
                foreach (var rawCVPe in CVPInput)
                {
                    normalizedCVP[index] = rawCVPe;
                    index++;
                }

                var didReduction = true;
                while (didReduction)
                {
                    didReduction = false;

                    for (var i = 0; i < normalizedCVP.Length; i++)
                        if (normalizedCVP[i] != 0 && !normalizedCVP.Contains(normalizedCVP[i] - 1))
                        {
                            normalizedCVP[i] = normalizedCVP[i] - 1;
                            didReduction = true;
                        }
                }

                return normalizedCVP;
            }
        }
    }
}