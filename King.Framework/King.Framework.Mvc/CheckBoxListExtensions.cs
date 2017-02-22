namespace King.Framework.Mvc
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Web.Mvc;

    public static class CheckBoxListExtensions
    {
        public static readonly List<string> listJS = new List<string> { CtrlScripts.bValidate, CtrlScripts.JqueryBvalidatorCss };

        public static string AddJs(HtmlHelper html)
        {
            return "";
        }

        public static string AddJs<TModel, TValue>(HtmlHelper<TModel> html)
        {
            return "";
        }

        public static MvcHtmlString KingCheckBoxList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> list, object Value = null, ValidateOptions validateOptions = null, bool IsDefaultStyle = true, object htmlAttribute = null, bool readOnly = false)
        {
            return MvcHtmlString.Create(GenerateHtml(name, list, htmlAttribute, Value, validateOptions, IsDefaultStyle, readOnly) + AddJs(htmlHelper));
        }

        public static MvcHtmlString KingCheckBoxList(this HtmlHelper htmlHelper, string name, Type enumType, object Value = null, ValidateOptions validateOptions = null, bool IsDefaultStyle = true, object htmlAttribute = null, bool readOnly = false)
        {
            IEnumerable<SelectListItem> items = new SelectList(MvcHtmlStringCommon.GetEnumToSelectList(enumType).AsEnumerable<SelectListItem>(), "Value", "Text");
            return MvcHtmlString.Create(GenerateHtml(name, items, htmlAttribute, Value, validateOptions, IsDefaultStyle, readOnly) + AddJs(htmlHelper));
        }

        public static MvcHtmlString KingCheckBoxListFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, string name = null, object Value = null, ValidateOptions validateOptions = null, bool IsDefaultStyle = true, object htmlAttribute = null, bool readOnly = false)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression<TModel, TValue>(expression, htmlHelper.ViewData);
            if (string.IsNullOrEmpty(name))
            {
                name = ExpressionHelper.GetExpressionText(expression);
            }
            IEnumerable<SelectListItem> items = null;
            if (Value == null)
            {
                Value = metadata.Model;
            }
            Type modelType = ValidateCommon.GetModelType(metadata.ModelType);
            if (modelType.IsEnum)
            {
                items = new SelectList(MvcHtmlStringCommon.GetEnumToSelectList(modelType).AsEnumerable<SelectListItem>(), "Value", "Text");
            }
            else if (modelType == typeof(bool))
            {
                SelectListItem item = new SelectListItem {
                    Text = "",
                    Value = "1"
                };
                List<SelectListItem> list = new List<SelectListItem> {
                    item
                };
                if (metadata.Model != null)
                {
                    Value = ((bool) metadata.Model) ? "1" : "0";
                }
                items = new SelectList(list, "Value", "Text");
            }
            return MvcHtmlString.Create(GenerateHtml(name, items, htmlAttribute, Value, validateOptions, IsDefaultStyle, readOnly) + AddJs(htmlHelper));
        }

        public static MvcHtmlString KingCheckBoxListFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, IEnumerable<SelectListItem> list, string name = null, object Value = null, ValidateOptions validateOptions = null, bool IsDefaultStyle = true, object htmlAttribute = null, bool readOnly = false)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression<TModel, TValue>(expression, htmlHelper.ViewData);
            if (string.IsNullOrEmpty(name))
            {
                name = ExpressionHelper.GetExpressionText(expression);
            }
            if (Value == null)
            {
                Value = metadata.Model;
            }
            return MvcHtmlString.Create(GenerateHtml(name, list, htmlAttribute, Value, validateOptions, IsDefaultStyle, readOnly) + AddJs(htmlHelper));
        }

        public static MvcHtmlString KingCheckBoxListFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, Type enumType, string name = null, object Value = null, ValidateOptions validateOptions = null, bool IsDefaultStyle = true, object htmlAttribute = null, bool readOnly = false)
        {
            IEnumerable<SelectListItem> items = new SelectList(MvcHtmlStringCommon.GetEnumToSelectList(enumType).AsEnumerable<SelectListItem>(), "Value", "Text");
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression<TModel, TValue>(expression, htmlHelper.ViewData);
            if (string.IsNullOrEmpty(name))
            {
                name = ExpressionHelper.GetExpressionText(expression);
            }
            if (Value == null)
            {
                Value = metadata.Model;
            }
            return MvcHtmlString.Create(GenerateHtml(name, items, htmlAttribute, Value, validateOptions, IsDefaultStyle, readOnly) + AddJs(htmlHelper));
        }

        public static string GenerateCheckBoxHtml(string name, string id, string labelText, string value, bool isChecked, IDictionary<string, object> htmlAttributes, ValidateOptions validateOptions, bool readOnly)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("<div class='checkbox_label'>");
            TagBuilder builder2 = new TagBuilder("label");
            builder2.MergeAttribute("for", id);
            builder2.SetInnerText(labelText);
            TagBuilder builder3 = new TagBuilder("input");
            builder3.GenerateId(id);
            builder3.MergeAttribute("name", name);
            builder3.MergeAttribute("custom", "big");
            builder3.MergeAttribute("type", "checkbox");
            builder3.MergeAttribute("value", value);
            if (readOnly)
            {
                builder3.MergeAttribute("disabled", "disabled");
            }
            builder3.MergeAttributes<string, object>(htmlAttributes);
            if (readOnly)
            {
                builder3.MergeAttribute("onclick", "return false;");
            }
            if (isChecked)
            {
                builder3.MergeAttribute("checked", "checked");
            }
            if (validateOptions != null)
            {
                builder3.MergeAttribute("data-bvalidator", ValidateCommon.GetValidateAttr(validateOptions));
                if (!string.IsNullOrEmpty(validateOptions.ErrorMsg))
                {
                    builder3.MergeAttribute("data-bvalidator-msg", validateOptions.ErrorMsg);
                }
            }
            builder.AppendLine("<span>" + builder3.ToString() + "</span>");
            builder.AppendLine(builder2.ToString());
            builder.AppendLine("</div>");
            return builder.ToString();
        }

        public static string GenerateHtml(string name, IEnumerable<SelectListItem> items, object htmlAttribute, object Value, ValidateOptions validateOptions, bool IsDefaultStyle, bool readOnly)
        {
            TagBuilder builder = new TagBuilder("div");
            builder.MergeAttribute("id", name);
            int num = 0;
            bool isChecked = false;
            if (items == null)
            {
                return "";
            }
            foreach (SelectListItem item in items)
            {
                IEnumerable<string> source = Value as IEnumerable<string>;
                isChecked = (source != null) && source.Contains<string>(item.Value);
                if (!(isChecked || (Value == null)))
                {
                    isChecked = Value.ToString() == item.Value;
                }
                num++;
                string id = string.Format("{0}_{1}", name, num);
                builder.InnerHtml = builder.InnerHtml + GenerateCheckBoxHtml(name, id, item.Text, item.Value, isChecked, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttribute), (num == items.Count<SelectListItem>()) ? validateOptions : null, readOnly);
            }
            if (!IsDefaultStyle)
            {
                builder.InnerHtml = builder.InnerHtml + "<script type='text/javascript'>";
                builder.InnerHtml = builder.InnerHtml + string.Format("$('#{0}').buttonset();", name);
                builder.InnerHtml = builder.InnerHtml + "</script>";
            }
            return builder.ToString();
        }
    }
}

