using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// ToDo: Provide a Inferred Knowledge checker.
// ToDo: Provide a cyclic approach that attempts to infer on smaller groups of data (requires outcome-caching centralized architecture).
// ToDo: Eliminate semantically equivalent outputted ILs (see FileStream AsyncRead with NonBuff-flag with FileSize ~1024).
// ToDo: Eliminate spurious candidates (C1> C2 and C1>=C2).
// ToDo: Support dynamic columns (for %2 for example).
// ToDo: Parallel usage in Generation of CVIL & CPIL. 
// ToDo: CPIL evaluation can be sped up by providing and caching normalized MIEData instead of pure MIEData.
// ToDo: Test performance with a pure OO approach, encapsulating CV, CP, CVIL, CPIL, TestResults.
// ToDo: Globalized caches can be used to reduce duplicate re-evaluation.
// ToDo: Recheck the IList etc. types.
namespace DevUtil.Dev_Testing.TesterSuite.InferenceEngine.Micro_InferenceEngine
{
    /// <summary>
    /// Infers information from integer-type test-data.
    /// Example-Input:
    /// 0, 512, 510, 510
    /// 1, 512, 510, 514
    /// Example-Output:
    /// C1 == 512 && C1 < C2
    /// </summary>
    public sealed partial class MicroInferenceEngine : MarshalByRefObject
    {
        enum TestResults { Mixed, OnlyPositive, OnlyNegative }
        /// <summary>
        /// Attempts to infer information from the provided data.
        /// </summary>
        /// <param name="MIEData">The MIE compatible data: the first column contains the target (must be 0 or 1).</param>
        /// <returns></returns>
        public string Infer(int[,] MIEData, bool includeInputInReport = false, bool includeWarningInReport = true)
        {
            if (MIEData == null || MIEData.GetLength(0) == 0 || MIEData.GetLength(1) == 0)
                return $"{nameof(MicroInferenceEngine)}:{nameof(Infer)}:{nameof(MIEData)} must have elements in both dimensions.";

            var rows = MIEData.GetLength(0);
            var columns = MIEData.GetLength(1) - 1; // Exclude results

            // ToDo: Merge the MIEData checks & TestResult Evaluations.

            if (EvaluateTestResults(MIEData, rows, columns) == TestResults.OnlyNegative)
                return "No positive test-cases found." + Environment.NewLine;

            if (EvaluateTestResults(MIEData, rows, columns) == TestResults.OnlyPositive)
                return "No negative test-cases found." + Environment.NewLine;

            var CVILs = GatherEnumeratorResults(CVInstructionList.Generate(rows, columns, MIEData));
            var CPILs = GatherEnumeratorResults(CPInstructionList.Generate(rows, columns, MIEData));
            var ILs = EvaluateILs(CVILs, CPILs, MIEData, rows);

            string report = string.Empty;
            if (ILs.Count == 0)
                report = CreateNanoInferTestReport(MIEData, columns, rows, includeInputInReport,
                    includeWarningInReport);
            else
                report = CreateInferTestReport(MIEData, ILs, includeInputInReport, includeWarningInReport);

            return report;
        }

        private TestResults EvaluateTestResults(int[,] MIEData, int rows, int columns)
        {
            var positiveTestResultFound = false;
            var negativeTestResultFound = false;

            for (var r = 0; r < rows; r++)
            {
                if (MIEData[r, 0] == 1)
                    positiveTestResultFound = true;
                if (MIEData[r, 0] == 0)
                    negativeTestResultFound = true;
                if (positiveTestResultFound && negativeTestResultFound)
                    return TestResults.Mixed;

            }

            if (positiveTestResultFound && !negativeTestResultFound)
                return TestResults.OnlyPositive;
            if (!positiveTestResultFound && negativeTestResultFound)
                return TestResults.OnlyNegative;

            throw new InvalidOperationException();
        }

        private string CreateInferTestReport(int[,] MIEData, IReadOnlyDictionary<(IReadOnlyList<int>, IReadOnlyList<int>), bool> ILs, bool includeInputInReport, bool includeWarningInReport)
        {
            // Select First IL with smallest count.
            var SmallestILOps = (from IL in ILs
                                 where (IL.Key.Item1?.Count ?? 0)
                                 + (IL.Key.Item2?.Count ?? 0)
                                 == (from ILmin in ILs
                                     select (ILmin.Key.Item1?.Count ?? 0)
                                            + (ILmin.Key.Item2?.Count ?? 0)).Min()
                                 select IL);

            var sb = new StringBuilder();
            var index = 0;
            if (includeInputInReport)
                sb.Append(String(MIEData));
            if (includeWarningInReport)
                sb.AppendLine($"Order is from left to right, equal ordering on operators.");
            foreach (var ILOps in SmallestILOps)
            {
                index++;
                var CVILString = ILOps.Key.Item1 == null ? "" : CVInstructionList.String(ILOps.Key.Item1);
                var CPILString = ILOps.Key.Item2 == null ? "" : CPInstructionList.String(ILOps.Key.Item2);
                if (SmallestILOps.Count() > 1)
                    sb.Append($"Candidate {index}: ");
                if (ILOps.Key.Item1 != null && ILOps.Key.Item2 != null)
                    sb.AppendLine((CVILString + " && " + CPILString).Trim());
                else if (ILOps.Key.Item1 != null)
                    sb.AppendLine(CVILString.Trim());
                else if (ILOps.Key.Item2 != null)
                    sb.AppendLine(CPILString.Trim());
            }
            return sb.ToString();
        }

        private string CreateNanoInferTestReport(int[,] MIEData, int columns, int rows, bool includeInputInReport, bool includeWarningInReport)
        {
            var sb = new StringBuilder();

            if (includeInputInReport)
                sb.Append(String(MIEData));

            if (includeWarningInReport)
                sb.AppendLine($"Order is from left to right, equal ordering on operators.");

            // Output inferred.
            for (var r = 0; r < rows; r++)
            {
                if (MIEData[r, 0] == 1)
                {
                    for (var c = 1; c < columns + 1; c++)
                        sb.Append(c == columns ? $"C{c}=={MIEData[r, c]}" : $"C{c}=={MIEData[r, c]} && ");
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }

        public string String(int[,] MIEData)
        {
            if (MIEData == null || MIEData.GetLength(0) == 0 || MIEData.GetLength(1) == 0)
                return $"{nameof(MicroInferenceEngine)}:{nameof(Infer)}:{nameof(MIEData)} must have elements in both dimensions.";

            var rows = MIEData.GetLength(0);
            var columns = MIEData.GetLength(1) - 1; // Exclude results

            var sb = new StringBuilder();

            for (var r = 0; r < rows; r++)
            {
                for (var c = 1; c < columns + 1; c++)
                {
                    if (c == columns)
                        sb.Append(MIEData[r, c]);
                    else
                        sb.Append(MIEData[r, c] + ",");
                }
                sb.AppendLine($",{MIEData[r, 0].ToString()}");
            }

            return sb.ToString();
        }

        private static IReadOnlyDictionary<(IReadOnlyList<int>, IReadOnlyList<int>), bool> EvaluateILs(IReadOnlyList<IReadOnlyList<int>> CVILs, IReadOnlyList<IReadOnlyList<int>> CPILs, int[,] MIEData, int rows)
        {
            var CVILcache = new bool[rows, CVILs.Count];
            var CPILcache = new bool[rows, CPILs.Count];
            var CorrectILs = new ConcurrentDictionary<(IReadOnlyList<int>, IReadOnlyList<int>), bool>();

            // Load the CVIL cache
            Parallel.For(0, CVILs.Count, CVILIndex =>
            {
                for (var MIEDataSelectedRow = 0; MIEDataSelectedRow < rows; MIEDataSelectedRow++)
                    CVILcache[MIEDataSelectedRow,CVILIndex] = CVInstructionList.Execute(CVILs[CVILIndex], MIEData, MIEDataSelectedRow);
            });

            // Load the CPIL cache
            Parallel.For(0, CPILs.Count, CPILIndex =>
            {
                for (var MIEDataSelectedRow = 0; MIEDataSelectedRow < rows; MIEDataSelectedRow++)
                    CPILcache[MIEDataSelectedRow, CPILIndex] = CPInstructionList.Execute(CPILs[CPILIndex], MIEData, MIEDataSelectedRow);
            });

            // -1 is a special null CVIL entry.
            Parallel.For(-1, CVILs.Count, CVILIndex => 
            {
                // Execute & Evaluate.
                for (var CPILIndex = 0; CPILIndex < CPILs.Count; CPILIndex++)
                {
                    var toAdd = true;
                    for (var MIEDataSelectedRow = 0; MIEDataSelectedRow < rows; MIEDataSelectedRow++)
                    {
                        var CVILExecuteResult = CVILIndex == -1 || CVILcache[MIEDataSelectedRow, CVILIndex];
                        var CPILExecuteResult = CPILcache[MIEDataSelectedRow, CPILIndex];
                        var ILResult = CVILExecuteResult && CPILExecuteResult;
                        var testResult = MIEData[MIEDataSelectedRow, 0] == 1;
                        if (ILResult != testResult)
                        {
                            toAdd = false;
                            break;
                        }
                    }

                    if (toAdd)
                        CorrectILs.TryAdd(CVILIndex == -1 ? (null, CPILs[CPILIndex]) : (CVILs[CVILIndex], CPILs[CPILIndex]), false);
                }

                // Execute & Evaluate.
                for (var CPILIndex = 0; CPILIndex < CPILs.Count; CPILIndex++)
                {
                    var toAdd = true;
                    for (var MIEDataSelectedRow = 0; MIEDataSelectedRow < rows; MIEDataSelectedRow++)
                    {
                        var CVILExecuteResult = CVILIndex == -1 || CVILcache[MIEDataSelectedRow, CVILIndex];
                        var CPILExecuteResult = CPILcache[MIEDataSelectedRow, CPILIndex];
                        var ILResult = CVILExecuteResult || CPILExecuteResult;
                        var testResult = MIEData[MIEDataSelectedRow, 0] == 1;
                        if (ILResult != testResult)
                        {
                            toAdd = false;
                            break;
                        }
                    }

                    if (toAdd)
                        CorrectILs.TryAdd(CVILIndex == -1 ? (null, CPILs[CPILIndex]) : (CVILs[CVILIndex], CPILs[CPILIndex]), false);
                }
            });

            return CorrectILs;
        }

        public IReadOnlyList<IReadOnlyList<T>> GatherEnumeratorResults<T>(IEnumerator<IReadOnlyList<T>> drainTarget)
        {
            var result = new List<IReadOnlyList<T>>();

            while (drainTarget.MoveNext())
                result.Add(drainTarget.Current);

            return result;
        }
    }
}