using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util.Extensions
{
    public static class IEnumerator_Ext
    {
        public static T[] Gather<T>(this IEnumerator<T> drainTarget)
        {
            var result = new List<T>();

            while (drainTarget.MoveNext())
                result.Add(drainTarget.Current);

            return result.ToArray();
        }
    }
}
