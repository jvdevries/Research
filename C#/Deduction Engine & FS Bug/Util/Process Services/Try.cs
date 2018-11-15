using System;

namespace Util.Process_Services
{
    /// <summary>
    /// Perform a <see cref="Delegate"/>, either eating the <see cref="Exception"/> or invoking a custom error <see cref="Delegate"/>.
    /// </summary>
    public class Try
    {
        public static void Do(Action toTry) => Do(Wrap(toTry));
        public static T Do<T>(Func<T> toTry) where T : class => DoTryError(toTry, RetNull<T>());

        private static Func<T> RetNull<T>() where T : class => () => null;

        private static Func<object> Wrap(Action toWrap) => () =>
        {
            toWrap.Invoke();
            return null;
        };

        public static T DoTryError<T>(Func<T> toTry, Func<T> errorReturn)
        {
            try
            {
                return toTry.Invoke();
            }
            catch
            {
                return errorReturn.Invoke();
            }
        }
    }
}