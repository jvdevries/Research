using System;
using System.Text;

namespace DevUtil.Dev_Testing.TesterSuite.Tester.DataStores
{
    /// <summary>
    /// Represents the result of a successfully finished <see cref="Test"/>.
    /// </summary>
    public sealed class TestResult
    {
        // Use long instead of ulong because of LINQ compatibility.
        public long Time { get; }
        public bool ErrorOccured { get; }
        public Exception Error { get; }
        public TestSetting Setting { get; }

        public TestResult(long runTime, TestSetting setting, Exception error = null)
        {
            Time = runTime;
            ErrorOccured = error != null;
            Error = error;
            Setting = setting;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (var set in Setting)
                sb.Append(set + ",");

            sb.Append(ErrorOccured ? "-1" : Time.ToString());

            return sb.ToString();
        }
    }
}