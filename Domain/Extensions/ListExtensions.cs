using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Extensions
{
    public static class ListExtensions
    {
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int size)
        {
            T[] bucket = null;
            var count = 0;

            foreach (var item in source)
            {
                if (bucket == null)
                    bucket = new T[size];

                bucket[count++] = item;

                if (count != size)
                    continue;

                yield return bucket.Select(x => x);

                bucket = null;
                count = 0;
            }

            // Return the last bucket with all remaining elements
            if (bucket != null && count > 0)
            {
                Array.Resize(ref bucket, count);
                yield return bucket.Select(x => x);
            }
        }

        public static IList<T> Swap<T>(this IList<T> source, T item1, T item2)
        {
            int index = source.IndexOf(item1);
            if (index < 0) return source;

            source.RemoveAt(index);
            source.Insert(index, item2);
            return source;
        }

    }
}
