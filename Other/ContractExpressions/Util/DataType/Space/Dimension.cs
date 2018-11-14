using System;
using System.Linq.Expressions;
using Util.Extensions;
using Util.LINQ_Services;

namespace Util.DataType.Space
{
    /// <inheritdoc/>
    public sealed class Dimension : VoidDimension
    {
        /// <summary>
        /// Creates a <see cref="Dimension"/>, which is a <see cref="VoidDimension"/> which has at least two <see cref="Axis"/>.
        /// </summary>
        public Dimension(Axis firstAxis, Axis secondAxis, params Axis[] otherAxes) : base(firstAxis,
            secondAxis.Add(otherAxes))
        {
        }

        public static Expression<Func<Dimension, bool>> StateContract
        {
            get
            {
                var thisType = Expression.Parameter(typeof(Dimension));
                var count = Expression.Parameter(typeof(int));
                var exitLabel = Expression.Label(typeof(bool));

                var atLeastTwoAxis = Expression.Block(new[] { count },
                    Expression.Assign(count, Expressions.CountElements(thisType, nameof(GetAxisEnumerator))),
                    Expression.IfThen(Expression.GreaterThan(count, Expression.Constant(1)),
                        Expression.Return(exitLabel, Expression.Constant(true))),
                    Expression.Label(exitLabel, Expression.Constant(false)));

                return Expression.Lambda<Func<Dimension, bool>>(atLeastTwoAxis, thisType);
            }
        }
    }
}