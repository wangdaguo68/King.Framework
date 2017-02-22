namespace King.Framework.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public static class TopologicalSorter
    {
        public static IEnumerable<T> Sort<T>(this IEnumerable<T> items, Func<T, IEnumerable<T>> fnItemsBeforeMe)
        {
            return items.Sort<T>(fnItemsBeforeMe, null);
        }

        public static IEnumerable<T> Sort<T>(this IEnumerable<T> items, Func<T, IEnumerable<T>> fnItemsBeforeMe, IEqualityComparer<T> comparer)
        {
            HashSet<T> seen = (comparer != null) ? new HashSet<T>(comparer) : new HashSet<T>();
            HashSet<T> done = (comparer != null) ? new HashSet<T>(comparer) : new HashSet<T>();
            List<T> result = new List<T>();
            foreach (T local in items)
            {
                SortItem<T>(local, fnItemsBeforeMe, seen, done, result);
            }
            return result;
        }

        private static void SortItem<T>(T item, Func<T, IEnumerable<T>> fnItemsBeforeMe, HashSet<T> seen, HashSet<T> done, List<T> result)
        {
            if (!done.Contains(item))
            {
                if (seen.Contains(item))
                {
                    throw new InvalidOperationException("Cycle in topological sort");
                }
                seen.Add(item);
                IEnumerable<T> enumerable = fnItemsBeforeMe(item);
                if (enumerable != null)
                {
                    foreach (T local in enumerable)
                    {
                        SortItem<T>(local, fnItemsBeforeMe, seen, done, result);
                    }
                }
                result.Add(item);
                done.Add(item);
            }
        }
    }
}
