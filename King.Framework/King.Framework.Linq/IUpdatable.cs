using System.Collections.Generic;

namespace King.Framework.Linq
{
    using System.Collections;
    using System.Linq;

    public interface IUpdatable : IQueryable, IEnumerable
    {
    }
    public interface IUpdatable<T> : IUpdatable, IQueryable<T>, IEnumerable<T>, IQueryable, IEnumerable
    {
    }
}
