namespace King.Framework.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime.CompilerServices;

    public static class ReadOnlyExtensions
    {
        public static ReadOnlyCollection<T> ToReadOnly<T>(this IEnumerable<T> collection)
        {
            ReadOnlyCollection<T> onlys = collection as ReadOnlyCollection<T>;
            if (onlys != null)
            {
                return onlys;
            }
            if (collection == null)
            {
                return EmptyReadOnlyCollection<T>.Empty;
            }
            return new List<T>(collection).AsReadOnly();
        }

        private class EmptyReadOnlyCollection<T>
        {
            internal static readonly ReadOnlyCollection<T> Empty;

            static EmptyReadOnlyCollection()
            {
                ReadOnlyExtensions.EmptyReadOnlyCollection<T>.Empty = new List<T>().AsReadOnly();
            }
        }
    }
}
