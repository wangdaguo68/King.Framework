namespace King.Framework.Mvc
{
    using King.Framework.Common;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;

    public class PagedList<T> : List<T>, IPagedList<T>, IEnumerable<T>, IPagedList, IEnumerable
    {
        public PagedList(IEnumerable<T> allItems, int pageIndex, int pageSize)
        {
            this.TotalItemCount = allItems.Count<T>();
            this.PageSize = pageSize.RangedTo(1, 0x7fffffff);
            this.TotalPageCount = PagedList<T>.GetPageCount(this.TotalItemCount, this.PageSize);
            this.CurrentPageIndex = pageIndex.RangedTo(1, this.TotalPageCount);
            base.AddRange(allItems.Skip<T>((this.StartRecordIndex - 1)).Take<T>(this.PageSize));
        }

        public PagedList(IQueryable<T> allItems, int pageIndex, int pageSize)
        {
            this.TotalItemCount = allItems.Count<T>();
            this.PageSize = pageSize.RangedTo(1, 0x7fffffff);
            this.TotalPageCount = PagedList<T>.GetPageCount(this.TotalItemCount, this.PageSize);
            this.CurrentPageIndex = pageIndex.RangedTo(1, this.TotalPageCount);
            base.AddRange(allItems.Skip<T>((this.StartRecordIndex - 1)).Take<T>(this.PageSize));
        }

        public PagedList(IEnumerable<T> currentPageItems, int pageIndex, int pageSize, int totalItemCount)
        {
            this.TotalItemCount = totalItemCount;
            this.PageSize = pageSize.RangedTo(1, 0x7fffffff);
            this.TotalPageCount = PagedList<T>.GetPageCount(this.TotalItemCount, this.PageSize);
            this.CurrentPageIndex = pageIndex.RangedTo(1, this.TotalPageCount);
            base.AddRange(currentPageItems);
        }

        public PagedList(IQueryable<T> currentPageItems, int pageIndex, int pageSize, int totalItemCount)
        {
            this.TotalItemCount = totalItemCount;
            this.PageSize = pageSize.RangedTo(1, 0x7fffffff);
            this.TotalPageCount = PagedList<T>.GetPageCount(this.TotalItemCount, this.PageSize);
            this.CurrentPageIndex = pageIndex.RangedTo(1, this.TotalPageCount);
            base.AddRange(currentPageItems);
        }

        private static int GetPageCount(int totalItemCount, int pageSize)
        {
            if (pageSize <= 0)
            {
                throw new ArgumentOutOfRangeException("pageSize == 0");
            }
            int num = totalItemCount / pageSize;
            if ((totalItemCount % pageSize) > 0)
            {
                num++;
            }
            if (num <= 0)
            {
                num = 1;
            }
            return num;
        }

        public int CurrentPageIndex { get; private set; }

        private int EndRecordIndex
        {
            get
            {
                return ((this.TotalItemCount > (this.CurrentPageIndex * this.PageSize)) ? (this.CurrentPageIndex * this.PageSize) : this.TotalItemCount);
            }
        }

        public string GridName { get; set; }

        public int PageSize { get; private set; }

        private int StartRecordIndex
        {
            get
            {
                return (((this.CurrentPageIndex - 1) * this.PageSize) + 1);
            }
        }

        public int TotalItemCount { get; private set; }

        public int TotalPageCount { get; private set; }
    }
}

