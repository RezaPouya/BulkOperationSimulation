using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Operator.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> GetElements<T>(this List<T> enumerable, int minIndex, int maxIndex)
        {
            if (maxIndex < minIndex)
                throw new ArgumentException("max index cannot be lesser than min index");

            var result = new List<T>((maxIndex - minIndex) + 1);

            int count = (int)enumerable.Count();

            int max = maxIndex > count - 1 ? (count - 1) : maxIndex;

            for (int i = 0; i < max; i++)
            {
                result.Add(enumerable[i]);
            }

            return result;
        }
    }
}
