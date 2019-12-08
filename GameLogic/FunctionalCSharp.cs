using System;
using System.Collections.Generic;
using System.Text;

namespace NeonTDS
{
    public static class FreakingFunctionalCSharp
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T item in source)
            {
                action(item);
            }
        }
    }
}
