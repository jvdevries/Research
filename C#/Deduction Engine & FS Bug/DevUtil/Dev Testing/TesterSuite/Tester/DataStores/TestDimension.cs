using System;
using System.Linq.Expressions;
using DevUtil.Aspects.Extensions;
using Util.DataType.Space;
using Util.LINQ_Services;

namespace DevUtil.Dev_Testing.TesterSuite.Tester.DataStores
{
    /// <inheritdoc/>
    public sealed class TestDimension : VoidDimension
    {
        /// <summary>
        /// Creates a <see cref="TestDimension"/>, which is a <see cref="VoidDimension"/> which has at least two <see cref="Axis"/>.
        /// </summary>
        public TestDimension(Axis firstAxis, Axis secondAxis, params Axis[] otherAxes) : base(firstAxis,
            secondAxis.InArray(otherAxes))
        {
        }

        public static Expression<Func<TestDimension, bool>> StateContract
        {
            get
            {
                var thisType = Expression.Parameter(typeof(TestDimension));
                var count = Expression.Parameter(typeof(int));
                var exitLabel = Expression.Label(typeof(bool));

                var atLeastTwoAxis = Expression.Block(new[] { count },
                    Expression.Assign(count, Expressions.CountElements(thisType, nameof(GetAxisEnumerator))),
                    Expression.IfThen(Expression.GreaterThan(count, Expression.Constant(1)),
                        Expression.Return(exitLabel, Expression.Constant(true))),
                    Expression.Label(exitLabel, Expression.Constant(false)));

                return Expression.Lambda<Func<TestDimension, bool>>(atLeastTwoAxis, thisType);
            }
        }
    }
}