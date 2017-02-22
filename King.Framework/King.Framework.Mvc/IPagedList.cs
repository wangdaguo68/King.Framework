namespace King.Framework.Mvc
{
    using System;
    using System.Collections;

    public interface IPagedList : IEnumerable
    {
        int CurrentPageIndex { get; }

        string GridName { get; set; }

        int PageSize { get; }

        int TotalItemCount { get; }
    }
}

