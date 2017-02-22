namespace King.Framework.Mvc
{
    using King.Framework.Common;
    using System;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;

    public static class HtmlHelperKingExtensions
    {
        public static MvcHtmlString AttachmentEditorFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            string str2 = HttpUtility.UrlEncode(MemberVisitor.GetMember<TModel, TValue>(expression).Name);
            StringBuilder builder = new StringBuilder(0x200);
            builder.Append("<div class=\"input-attachment\">").Append("<input type=\"hidden\" ").Append("id =\"").Append(str2).Append("\" ").Append("name=\"").Append(str2).Append("\" />").Append("<iframe src=\"/Attachment/Upload").Append("?updateKey=").Append(str2).Append("\"").Append(" ></iframe></div>");
            return MvcHtmlString.Create(builder.ToString());
        }

        public static MvcHtmlString KingEditor(this HtmlHelper html, string expression)
        {
            return html.Editor(expression);
        }

        public static MvcHtmlString KingEditorFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            return html.EditorFor<TModel, TValue>(expression);
        }
    }
}

