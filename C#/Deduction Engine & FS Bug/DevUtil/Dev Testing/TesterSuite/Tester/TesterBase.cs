using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DevUtil.Dev_Testing.TesterSuite.Tester.DataStores;
using Util.DataType.Space;

namespace DevUtil.Dev_Testing.TesterSuite.Tester
{
    // Runs are done in Axis-element order.
    public abstract class TesterBase : IEnumerable<TestResult>
    {
        private readonly Test _test;
        private readonly TestDimension _testDimension;

        protected TesterBase(TestDimension testDimension, Test test)
        {
            CheckTest(testDimension, test);

            _test = test;
            _testDimension = testDimension;
        }

        /// <inheritDoc/>
        public int Count => _testDimension.Count;

        /// <inheritDoc/>
        public int AxisCount => _testDimension.AxisCount;

        private void CheckTest(TestDimension testDimension, Test test)
        {
            if (testDimension == null)
                throw new ArgumentNullException(
                    $"{nameof(TesterBase)}:{nameof(TesterBase)}:{nameof(testDimension)} cannot be NULL.");

            var dimensionAxisMatchTest = test.Get().GetMethodInfo().GetParameters()
                .Select(x => new {x.ParameterType, x.Position})
                .OrderBy(x => x.Position)
                .Select(x => x.ParameterType)
                .SequenceEqual(testDimension.AxisTypes);

            if (!dimensionAxisMatchTest)
                throw new ArgumentException(
                    $"{nameof(TesterBase)}:{nameof(TesterBase)}:{nameof(test)} does not work Type-wise with {nameof(testDimension)}.");
        }

        public IEnumerator<TestResult> GetEnumerator()
        {
            foreach (var testParameters in _testDimension)
                yield return Run(testParameters, _test);
        }

        private TestResult Run(Point testParameters, Test test)
        {
            TestResult result;
            try
            {
                var sw = Stopwatch.StartNew();
                var testResult = test.Get().DynamicInvoke(testParameters.AxisValues);

                if (testResult != null) // Possible async test-result.
                {
                    if (testResult.GetType() == typeof(Task<>).MakeGenericType(typeof(Exception)))
                        testResult = (testResult as Task<Exception>)?.Result;
                }
                sw.Stop();
                var runTime = sw.ElapsedTicks;

                result = testResult == null
                    ? new TestResult(runTime, testParameters)
                    : new TestResult(runTime, testParameters, testResult as Exception);
            }
            catch (Exception e)
            {
                result = new TestResult(0, testParameters, e);
            }

            return result;
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}