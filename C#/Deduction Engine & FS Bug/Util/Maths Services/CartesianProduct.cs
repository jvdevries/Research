using System;
using System.Collections.Generic;
using System.Linq;

namespace Util.Maths_Services
{
    /// <summary>
    /// Returns the collection of elements formed by the Cartesian Product.
    /// </summary>
    /// <typeparam name="T">The base element of the input (and thus also the output) collection.</typeparam>
    public class CartesianProduct<T>
    {
        /// <summary>
        /// Creates a Linq statement yielding the Cartesian Product from the inputted collections.
        /// </summary>
        /// <param name="collections">The input collections. At least one collection with one element must be provided</param>
        /// <returns>A Linq statement giving the Cartesian Product, or an <see cref="ArgumentNullException">.</returns>
        public static IEnumerable<IEnumerable<T>> GetLinq(params IEnumerable<T>[] collections)
        {
            return collections.Aggregate(
                new List<IEnumerable<T>> {Enumerable.Empty<T>()} as IEnumerable<IEnumerable<T>>,
                (x, y) =>
                    from xE in x.AsParallel()
                    from yE in y
                    select xE.Append(yE));
        }

        private static void InputVerification(IReadOnlyList<IReadOnlyList<T>> collections,
            [System.Runtime.CompilerServices.CallerMemberName]
            string memberName = "")
        {
            if (collections == null || collections.Count == 0 || collections[0].Count == 0)
                throw new ArgumentException(
                    $"{nameof(CartesianProduct<T>)}:{memberName}:{nameof(collections)} requires at least a collection-entry with one element.");
        }

        private static (int rows, int columns, int[] columnDividers) ProcessInputCollections(
            IReadOnlyList<IReadOnlyList<T>> collections)
        {
            var rows = collections.Aggregate(1, (x, y) => x * y.Count);
            var columns = collections.Count;

            var columnDividers = new int[columns];
            var totalDividerSum = 1;
            columnDividers[columns - 1] = 1;
            for (var i = columns - 2; i >= 0; i--)
            {
                totalDividerSum = totalDividerSum * collections[i + 1].Count;
                columnDividers[i] = totalDividerSum;
            }

            return (rows, columns, columnDividers);
        }

        private static IReadOnlyList<T> GetRow(int row, IReadOnlyList<IReadOnlyList<T>> input,
            IReadOnlyList<int> columnDividers)
        {
            var result = new List<T>(input.Count);

            for (var i = 0; i < input.Count; i++)
                result.Add(input[i][row / columnDividers[i] % input[i].Count]);

            return result;
        }

        public static IReadOnlyList<IReadOnlyList<T>> Get(params IReadOnlyList<T>[] collections)
        {
            InputVerification(collections);
            var parameters = ProcessInputCollections(collections);

            var result = new List<IReadOnlyList<T>>(parameters.rows);

            for (var i = 0; i < parameters.rows; i++)
                result.Add(GetRow(i, collections, parameters.columnDividers));

            return result;
        }

        public static T[,] Get2DArray(params IReadOnlyList<T>[] collections)
        {
            InputVerification(collections);
            var parameters = ProcessInputCollections(collections);

            var result = new T[parameters.rows, parameters.columns];

            for (var i = parameters.columns - 1; i >= 0; i--)
            {
                var subIndex = 0;
                var index = 0;

                for (var j = 0; j < parameters.rows; j++)
                {
                    result[j, i] = collections[i][index];

                    subIndex++;
                    if (subIndex == parameters.columnDividers[i])
                    {
                        index++;
                        subIndex = 0;
                        if (index >= collections[i].Count)
                            index = 0;
                    }
                }
            }

            return result;
        }
    }
}