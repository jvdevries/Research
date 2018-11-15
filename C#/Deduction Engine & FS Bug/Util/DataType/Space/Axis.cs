using System;
using System.Collections;

namespace Util.DataType.Space
{
    /// <summary>
    /// Represents an <see cref="IEnumerable"/> axis (i.e. an ordered collection of comparable values). Implemented as a deeply immutable data-class. Instantiation is done by means of <see cref="Create"/> methods.
    /// </summary>
    public class Axis : IEnumerable
    {
        private Axis() // Disallow class instantiation and inheriting.
        {
        }

        // ReSharper disable once InvalidXmlDocComment
        /// <summary>
        /// Instantiates a <see cref="Axis"/> with values whose Type are <see cref="unmanaged"/>, <see cref="IComparable"/>, and <see cref="IComparable{T}"/>. Must have at least 1 value.
        /// </summary>
        /// <typeparam name="T">A Value-Type with only Value-Types at any level of nesting.</typeparam>
        /// <param name="name">The name of the <see cref="Axis"/>.</param>
        /// <param name="firstValue">The first value on the axis (required).</param>
        /// <param name="otherValues">Other values on the axis (optional).</param>
        /// <returns>The <see cref="Axis"/>.</returns>
        public static Axis Create<T>(string name, T firstValue, params T[] otherValues)
            where T : unmanaged, IComparable, IComparable<T>
            => new InternalAxis<T>(name, firstValue, otherValues);

        /// <summary>
        /// Instantiates a <see cref="Axis"/> with value of Type <see cref="string"/>. Must have at least 1 value.
        /// </summary>
        /// <inheritdoc cref="OverloadedMethod(string, string, params string[])" select="param|returns" />
        public static Axis Create(string name, string firstValue, params string[] otherValues)
            => new InternalAxis<string>(name, firstValue, otherValues);

        /// <summary>
        /// Instantiates a <see cref="Axis"/> with value of Type <see cref="Enum"/>. Must have at least 1 value.
        /// </summary>
        /// <inheritdoc cref="Create{T}(string, T, T[])" select="param|returns" />
        public static Axis CreateEnum<T>(string name, T firstValue, params T[] otherValues) where T : Enum
            => new InternalAxis<T>(name, firstValue, otherValues);

        #region virtual methods

        /// <summary>
        /// Returns the name of the <see cref="Axis"/>.
        /// </summary>
        public virtual string Name => default;

        /// <summary>
        /// Returns the type of values stored in the <see cref="Axis"/>.
        /// </summary>
        public virtual Type UnderlyingType => default;

        /// <summary>
        /// Returns the amount of values stored in the <see cref="Axis"/>.
        /// </summary>
        public virtual int Count => default;

        // ReSharper disable once AssignNullToNotNullAttribute
        public virtual IEnumerator GetEnumerator() => default;

        #endregion virtual methods

        // Constructs the actual Axis.
        private class InternalAxis<T> : Axis, IEnumerable
        {
            private readonly T[] _values;

            public InternalAxis(string name, T firstValue, T[] otherValues)
            {
                if (firstValue == null)
                    throw new ArgumentNullException(
                        $"{nameof(Axis)}:{nameof(Create)} an {nameof(Axis)} must have at least one value.");

                _values = new T[otherValues.Length + 1];
                _values[0] = firstValue;
                otherValues.CopyTo(_values, 1);
                Name = name;
            }

            public override string Name { get; }

            public override Type UnderlyingType => typeof(T);

            public override int Count => _values.Length;

            public override IEnumerator GetEnumerator() => _values.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}