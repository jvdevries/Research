using System.Collections;
using System.Linq.Expressions;
using Util.DataType.Space;

namespace Util.LINQ_Services
{
    public class Expressions
    {
        /// <summary>
        /// Counts the number of elements in a collection.
        /// </summary>
        /// <param name="collection">The collection of which the amount of elements is counted.</param>
        /// <param name="getEnumeratorName">The name of the enumerator method.</param>
        /// <returns>The <see cref="Expression"/> representing the Count Elements.</returns>
        public static Expression CountElements(ParameterExpression collection,
            string getEnumeratorName = nameof(IEnumerable.GetEnumerator))
        {
            var collectionElement = Expression.Parameter(typeof(object));
            var count = Expression.Parameter(typeof(int));
            var countIncrease = Expression.Assign(count, Expression.Increment(count));

            var exitLabel = Expression.Label(typeof(int));

            return Expression.Block(new[] {count},
                ForEach(collection, collectionElement, countIncrease, nameof(VoidDimension.GetAxisEnumerator)),
                Expression.Return(exitLabel, count),
                Expression.Label(exitLabel, Expression.Constant(0)));
        }

        /// <summary>
        /// Creates a foreach <see cref="Expression"/> akin to foreach (var element in collection) { toExecute(); }
        /// </summary>
        /// <param name="collection">The collection to iterate.</param>
        /// <param name="collectionElement">The iterated element of the collection.</param>
        /// <param name="toExecute">The <see cref="Expression"/> that is executed each iteration.</param>
        /// <param name="getEnumeratorName">The name of the enumerator method.</param>
        /// <returns>The <see cref="Expression"/> representing the foreach loop.</returns>
        public static Expression ForEach(ParameterExpression collection, ParameterExpression collectionElement,
            Expression toExecute, string getEnumeratorName = nameof(IEnumerable.GetEnumerator))
        {
            var enumerator = Expression.Variable(typeof(IEnumerator));

            var getEnumeratorMethod = collection.Type.GetMethod(getEnumeratorName);
            if (getEnumeratorMethod == null) return Expression.Constant(0);

            var moveNextMethod = typeof(IEnumerator).GetMethod(nameof(IEnumerator.MoveNext));
            if (moveNextMethod == null) return Expression.Constant(0);

            LabelTarget endLoop = Expression.Label(nameof(endLoop));
            return Expression.Block(new[] {enumerator},
                Expression.Assign(enumerator, Expression.Call(collection, getEnumeratorMethod)),
                Expression.Loop(
                    Expression.IfThenElse(
                        Expression.Equal(Expression.Call(enumerator, moveNextMethod), Expression.Constant(false)),
                        Expression.Break(endLoop),
                        Expression.Block(new[] {collectionElement},
                            // Because [].GetEnum => IEnum (not IEnum<>), Convert is used.
                            Expression.Assign(collectionElement, Expression.Convert(
                                Expression.Property(enumerator, nameof(IEnumerator.Current)), collectionElement.Type)),
                            toExecute
                        )
                    ),
                    endLoop)
            );
        }
    }
}