using System.Collections.Generic;

namespace King.Framework.Linq
{
    using System.Collections;

    public interface IDeferredList : IList, ICollection, IEnumerable, IDeferLoadable
    {
    }
    public interface IDeferredList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IDeferredList, IList, ICollection, IEnumerable, IDeferLoadable
    {
    }
}
