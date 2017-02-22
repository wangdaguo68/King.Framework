namespace King.Framework.Mvc
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;

    public static class PagerHelper
    {
        public static void KingRenderGridAndPager(this HtmlHelper html, string gridName, string queryActionName)
        {
            html.RenderAction(queryActionName, new { _gridName = gridName });
            PagedQueryInfo model = new PagedQueryInfo {
                GridName = gridName,
                QueryActionName = queryActionName
            };
            html.RenderPartial("~/Views/Shared/_PagerScript.cshtml", model);
            PagedQueryInfo info2 = new PagedQueryInfo {
                GridName = gridName,
                QueryActionName = queryActionName
            };
            html.RenderPartial("~/Views/Shared/_PagerPostScript.cshtml", info2);
        }

        public static void KingRenderPager(this HtmlHelper html, IPagedList model)
        {
            html.RenderPartial("~/Views/Shared/_Pager.cshtml", model);
        }

        public static PagedList<T> ToPagedList<T>(this IEnumerable<T> allItems, int pageIndex, int pageSize)
        {
            return allItems.AsQueryable<T>().ToPagedList<T>(pageIndex, pageSize);
        }

        public static PagedList<T> ToPagedList<T>(this IQueryable<T> allItems, int pageIndex, int pageSize)
        {
            if (pageIndex < 1)
            {
                pageIndex = 1;
            }
            int count = (pageIndex - 1) * pageSize;
            int totalItemCount = allItems.Count<T>();
            while ((totalItemCount <= count) && (pageIndex > 1))
            {
                count = (--pageIndex - 1) * pageSize;
            }
            return new PagedList<T>(allItems.Skip<T>(count).Take<T>(pageSize), pageIndex, pageSize, totalItemCount);
        }
    }
}

