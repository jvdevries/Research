using System;
using System.Reflection;
using System.Threading.Tasks;

namespace DevUtil.Dev_Testing.TesterSuite.Tester.DataStores
{
    /// <summary>
    /// Represents a <see cref="Test"/> that is compatible with the <see cref="TesterBase"/>.
    /// </summary>
    public sealed class Test
    {
        private readonly Delegate _test;

        private Test(Delegate test)
        {
            _test = test;
        }

        public static Test Create(Delegate test)
        {
            var returnType = test.GetMethodInfo().ReturnType;
            if (returnType != typeof(Exception) && returnType != typeof(Task<>).MakeGenericType(typeof(Exception)))
                throw new ArgumentException(
                    $"{nameof(TesterBase)}:{nameof(TesterBase)}:{nameof(test)} requires an Exception or Task<Exception> return type for Tests.");

            return new Test(test);
        }

        public Delegate Get() => _test;
    }
}