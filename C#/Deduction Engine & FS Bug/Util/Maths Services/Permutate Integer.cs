using System.Collections.Generic;
using System.Linq;

namespace Util.Maths_Services
{
    /// <summary>
    /// Returns the collection of permutations of the input.
    /// </summary>
    public class PermutateInt
    {
        /// <summary>
        /// Provides permutations of integers. With 2-elements and permutations of any length, the method yields:
        ///  <see cref="allowDuplicateElements"/>
        ///     <see cref="orderMatters"/>
        ///  T  T  - 0, 1, 00, 01, 10, 11
        ///  F  T  - 0, 1, 01, 10
        ///  T  F  - 0, 1, 00, 01, 11
        ///  F  F  - 0, 1, 01
        /// </summary>
        /// <param name="elements">The amount of elements.</param>
        /// <param name="minNrOfElemInPermutation">The minimum number of elements of returned permutations.</param>
        /// <param name="maxNrOfElemInPermutation">The maximum number of elements of returned permutations.</param>
        /// <param name="allowDuplicateElements">Setting that allows/denies duplicate elements in permutations.</param>
        /// <param name="orderMatters">Setting that indicates if element-order matters in permutations.</param>
        /// <returns>An IEnumerator yielding permutations.</returns>
        public static IEnumerator<IReadOnlyList<int>> GetEnum(int elements, int minNrOfElemInPermutation,
            int maxNrOfElemInPermutation, bool allowDuplicateElements, bool orderMatters)
        {
            if (maxNrOfElemInPermutation == 0)
            {
                yield return new int[0];
                yield break;
            }

            var permutationBeingFormed = new List<int> {0};

            while (permutationBeingFormed.Count != 0)
            {
                // Stop when the last element exceeds the max.
                if (permutationBeingFormed.Last() > elements - 1)
                {
                    // Remove the last element.
                    permutationBeingFormed.RemoveAt(permutationBeingFormed.Count - 1);

                    // Last element removed, so exit.
                    if (permutationBeingFormed.Count == 0)
                        break;

                    // Load the next value, possible going over the max.
                    var nextValue = GetElementValue(permutationBeingFormed.Last() + 1, elements, permutationBeingFormed,
                        allowDuplicateElements, orderMatters);
                    permutationBeingFormed[permutationBeingFormed.Count - 1] = nextValue;
                    continue;
                }

                // Return value if acceptable.
                if (permutationBeingFormed.Count >= minNrOfElemInPermutation)
                    yield return permutationBeingFormed.ToArray();

                // If more elements are needed, add another element.
                if (permutationBeingFormed.Count < maxNrOfElemInPermutation)
                {
                    var nextElementValue = GetElementValue(0, elements, permutationBeingFormed, allowDuplicateElements,
                        orderMatters);
                    permutationBeingFormed.Add(nextElementValue);
                    continue;
                }

                // Update the value of the last element.
                var lastElementValue = GetElementValue(permutationBeingFormed.Last() + 1, elements,
                    permutationBeingFormed, allowDuplicateElements, orderMatters);
                permutationBeingFormed[permutationBeingFormed.Count - 1] = lastElementValue;
            }
        }

        /// <inheritdoc cref="GetEnum" select="summary|param"/>
        /// <param name="permutationsCache">A compatible cache. Can be created using <see cref="CreateCache"/>.</param>
        /// <returns>All possible permutations.</returns>
        public static IReadOnlyList<IReadOnlyList<int>> Get(int elements, int minNrOfElemInPermutation,
            int maxNrOfElemInPermutation,
            bool allowDuplicateElements, bool orderMatters,
            Dictionary<(int elements, int minNrOfElemInPermutation, int maxNrOfElemInPermutation, bool
                allowDuplicateElements, bool orderMatters), IReadOnlyList<IReadOnlyList<int>>> permutationsCache = null)
        {
            if (maxNrOfElemInPermutation == 0)
                return new List<IReadOnlyList<int>>();

            // Grab from cache if there.
            if (permutationsCache != null)
                if (permutationsCache.ContainsKey((elements, minNrOfElemInPermutation, maxNrOfElemInPermutation,
                    allowDuplicateElements, orderMatters)))
                    return permutationsCache[
                        (elements, minNrOfElemInPermutation, maxNrOfElemInPermutation, allowDuplicateElements,
                            orderMatters)];

            // Permutate.
            var enumerator = GetEnum(elements, minNrOfElemInPermutation, maxNrOfElemInPermutation,
                allowDuplicateElements, orderMatters);
            var permutations = new List<IReadOnlyList<int>>();

            while (enumerator.MoveNext())
                permutations.Add(enumerator.Current);

            // Store in cache.
            permutationsCache?.Add(
                (elements, minNrOfElemInPermutation, maxNrOfElemInPermutation, allowDuplicateElements,
                    orderMatters), permutations);

            return permutations;
        }

        public static Dictionary<(int elements, int minNrOfElemInPermutation, int maxNrOfElemInPermutation, bool
            allowDuplicateElements, bool orderMatters), IReadOnlyList<IReadOnlyList<int>>> CreateCache()
            => new Dictionary<(int elements, int minNrOfElemInPermutation, int maxNrOfElemInPermutation, bool
                allowDuplicateElements, bool orderMatters), IReadOnlyList<IReadOnlyList<int>>>();

        // Get the next value for the element.
        private static int GetElementValue(int initialValue, int maxElement,
            IReadOnlyCollection<int> elementBeingFormed,
            bool allowDuplicateElements, bool orderRelevant)
        {
            initialValue = initialValue - 1;
            do
            {
                initialValue++;
                if (!allowDuplicateElements)
                {
                    if (elementBeingFormed.Contains(initialValue))
                        continue;
                }

                if (!orderRelevant)
                {
                    if (elementBeingFormed.Count > 0 && initialValue < elementBeingFormed.Last())
                        continue;
                }

                return initialValue;
            } while (initialValue < maxElement + 1);

            return initialValue;
        }
    }
}