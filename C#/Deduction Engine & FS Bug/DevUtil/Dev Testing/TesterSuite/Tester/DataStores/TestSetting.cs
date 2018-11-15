using System.Collections;
using System.Collections.Generic;
using Point = Util.DataType.Space.Point;

namespace DevUtil.Dev_Testing.TesterSuite.Tester.DataStores
{
    /// <summary>
    /// Represents the Setting in which a <see cref="Test"/> is ran.
    /// </summary>
    public sealed class TestSetting : IEnumerable<object>
    {
        private object[] _parameters;

        public IEnumerator<object> GetEnumerator()
            => ((IEnumerable<object>) _parameters).GetEnumerator();

        public int Length => _parameters.Length;

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        // This is an alias for Point, but this unfortunately requires casting due to Point being sealed.
        public static implicit operator TestSetting(Point p) => new TestSetting {_parameters = p.AxisValues};
        public static implicit operator Point(TestSetting ts) => new Point(ts._parameters);
    }
}