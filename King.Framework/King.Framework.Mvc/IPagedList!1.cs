namespace King.Framework.Mvc
{
    using System.Collections;
    using System.Collections.Generic;

    public interface IPagedList<T> : IEnumerable<T>, IPagedList, IEnumerable
    {
    }
}

