namespace King.Framework.Mvc
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;

    public static class TabExtentions
    {
        private static int SelectedIndex = 0;

        public static MvcHtmlString KingBeginTabNode(this HtmlHelper html)
        {
            Stack<TabContext> stack = html.ViewContext.TempData[TabContext.Key] as Stack<TabContext>;
            if (stack == null)
            {
                throw new ApplicationException("未调用KingBeginTabs");
            }
            TabContext context = stack.Peek();
            string str = string.Format("<div id=\"{0}-{1}\">", context.Id, context.CurrentIndex);
            context.CurrentIndex++;
            return MvcHtmlString.Create(str);
        }

        public static MvcHtmlString KingBeginTabs(this HtmlHelper html, string id, string[] headers, int selectedIndex)
        {
            int length = headers.Length;
            if (selectedIndex < length)
            {
                SelectedIndex = selectedIndex;
            }
            StringBuilder builder = new StringBuilder(0x200);
            builder.Append("<div id=\"").Append(id).Append("\">").Append("<ul>");
            for (int i = 0; i < length; i++)
            {
                builder.Append("<li><a href=\"#").Append(id).Append("-").Append(i).Append("\">");
                builder.Append(HttpUtility.HtmlEncode(headers[i]));
                builder.Append("</a></li>");
            }
            builder.Append("</ul>");
            Stack<TabContext> stack = html.ViewContext.TempData[TabContext.Key] as Stack<TabContext>;
            if (stack == null)
            {
                stack = new Stack<TabContext>();
                html.ViewContext.TempData[TabContext.Key] = stack;
            }
            stack.Push(new TabContext(id, headers));
            return MvcHtmlString.Create(builder.ToString());
        }

        public static MvcHtmlString KingEndTabNode(this HtmlHelper html)
        {
            return MvcHtmlString.Create("</div>");
        }

        public static MvcHtmlString KingEndTabs(this HtmlHelper html)
        {
            Stack<TabContext> stack = html.ViewContext.TempData[TabContext.Key] as Stack<TabContext>;
            if (stack == null)
            {
                throw new ApplicationException("未调用KingBeginTabs");
            }
            TabContext context = stack.Peek();
            StringBuilder builder = new StringBuilder(0x100);
            builder.AppendLine("</div>");
            builder.AppendLine("<script type=\"text/javascript\">");
            builder.AppendLine(string.Concat(new object[] { "$('#", context.Id, "').tabs({active: ", SelectedIndex, "});" })).AppendLine();
            builder.AppendLine("</script>");
            stack.Pop();
            return MvcHtmlString.Create(builder.ToString());
        }

        private class TabContext
        {
            public int CurrentIndex;
            public readonly string[] Headers;
            public readonly string Id;
            public static readonly string Key = "__TabContext";

            public TabContext(string id, string[] headers)
            {
                this.Id = id;
                this.Headers = headers;
                this.CurrentIndex = 0;
            }
        }
    }
}

