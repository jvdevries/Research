using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Util.Extensions
{
    public static class ExpressionExt
    {
        public static string GetViewHashCode(this Expression str)
        {
            var debugViewProperty =
                typeof(Expression).GetProperty("DebugView", BindingFlags.Instance | BindingFlags.NonPublic);
            var debugView = debugViewProperty?.GetValue(str) as string;

            using (var hasher = MD5.Create())
            {
                // Throw should never happen, since DebugView is a part of MS Expression implementation.
                var inputBytes = Encoding.ASCII.GetBytes(debugView ?? throw new InvalidOperationException());
                var hashBytes = hasher.ComputeHash(inputBytes);

                var hashCode = new StringBuilder();
                for (var i = 0; i < hashBytes.Length; i++)
                    hashCode.Append(i.ToString("X2"));

                return hashCode.ToString();
            }
        }
    }
}