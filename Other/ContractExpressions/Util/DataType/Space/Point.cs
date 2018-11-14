namespace Util.DataType.Space
{
    /// <summary>
    /// Represents a point in a Dimension.
    /// </summary>
    public struct Point
    {
        public object[] AxisValues { get; }

        public Point(object[] axisValues)
        {
            AxisValues = axisValues ?? new object[0];
        }

        public override string ToString()
            => string.Join(",", AxisValues);
    }
}