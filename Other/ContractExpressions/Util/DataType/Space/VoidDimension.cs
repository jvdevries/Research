using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Util.DataType.Space
{
    /// <summary>
    /// Represents the collection of all possible <see cref="Point"/> in the <see cref="VoidDimension"/> formed by <see cref="Axis"/>.
    /// </summary>
    public class VoidDimension : IEnumerable<Point>
    {
        private readonly Axis[] _axes;
        private readonly Lazy<int> _count;

        public VoidDimension(Axis firstAxis, params Axis[] otherAxes)
        {
            if (firstAxis == null) throw new ArgumentNullException();

            _count = new Lazy<int>(() => otherAxes.Aggregate(firstAxis.Count, (current, axis) => current * axis.Count));
            _axes = new Axis[otherAxes.Length + 1];
            _axes[0] = firstAxis;
            otherAxes.CopyTo(_axes, 1);
        }

        /// <summary>
        /// Returns the amount of <see cref="Point"/> in the <see cref="VoidDimension"/>.
        /// </summary>
        public int Count => _count.Value;

        /// <summary>
        /// Returns the amount of <see cref="Axis"/> in the <see cref="VoidDimension"/>.
        /// </summary>
        public int AxisCount => _axes.Length;

        /// <summary>
        /// Returns the types of the <see cref="Axis"/> in the <see cref="VoidDimension"/>.
        /// </summary>
        public IEnumerable<Type> AxisTypes => _axes.Select(x => x.UnderlyingType);

        /// <summary>
        /// Returns an enumerator for the <see cref="Axis"/> of the <see cref="VoidDimension"/>.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Axis> GetAxisEnumerator() => (_axes as IEnumerable<Axis>).GetEnumerator();

        /// <summary>
        /// Walks through the <see cref="VoidDimension"/> in a reverse-depth-first fashion. So the Dimension X{0,1} Y{A,B} yields 0A, 0B, 1A, 1B.
        /// </summary>
        /// <returns>The point in the <see cref="VoidDimension"/> currently being walked.</returns>
        public IEnumerator<Point> GetEnumerator()
        {
            // To walk through the dimension, an enumerator is kept for every axis.
            var dimensionWalker = new IEnumerator[_axes.Length];
            var dimensionWalkerValues = new object[_axes.Length]; // Contains the current values of the enums.

            // Load initial values.
            for (var i = 0; i < _axes.Length; i++)
            {
                dimensionWalker[i] = _axes[i].GetEnumerator();
                dimensionWalker[i].MoveNext();
                dimensionWalkerValues[i] = dimensionWalker[i].Current;
            }

            var doWhile = true;
            while (doWhile)
            {
                yield return new Point(dimensionWalkerValues);

                if (dimensionWalker[_axes.Length - 1].MoveNext())
                {
                    // Update value.
                    dimensionWalkerValues[_axes.Length - 1] = dimensionWalker[_axes.Length - 1].Current;
                    continue;
                }

                // Find the next Axis for which the Value can be increased (by a single step).
                for (var i = _axes.Length - 2; i >= 0; i--)
                {
                    if (!dimensionWalker[i].MoveNext())
                    {
                        // This axis cannot be increased further.

                        if (i != 0) continue;

                        // Not even the last processed Axis can be increased, so stop.
                        doWhile = false;
                        break;
                    }

                    // This Axis can be increased.
                    dimensionWalkerValues[i] = dimensionWalker[i].Current;

                    // Reset the previous Axis.
                    for (var j = i + 1; j < dimensionWalker.Length; j++)
                    {
                        dimensionWalker[j] = _axes[j].GetEnumerator();
                        dimensionWalker[j].MoveNext();
                        dimensionWalkerValues[j] = dimensionWalker[j].Current;
                    }

                    break;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}