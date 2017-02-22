namespace King.Framework.Mvc
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Web.Mvc;

    public static class TextBoxExtensions
    {
        public static string AddJs(HtmlHelper html)
        {
            return CtrlScripts.RenderScript(html, new List<string>());
        }

        public static string AddJs<TModel, TValue>(HtmlHelper<TModel> html)
        {
            return CtrlScripts.RenderScript(html, new List<string> { CtrlScripts.JqueryUiCss, CtrlScripts.JqueryBvalidatorCss, CtrlScripts.JQueryUI, CtrlScripts.bValidate });
        }

        public static MvcHtmlString KingTextBox(this HtmlHelper helper, string name, string value = null, object htmlAttribute = null, ValidateOptions validateOptions = null, bool readOnly = false)
        {
            return MvcHtmlString.Create(GetTextBoxMvcHtml(name, value, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttribute), validateOptions, readOnly) + AddJs(helper));
        }

        public static MvcHtmlString KingTextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, object htmlAttribute = null, ValidateOptions validateOptions = null, bool readOnly = false)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression<TModel, TProperty>(expression, helper.ViewData);
            string expressionText = ExpressionHelper.GetExpressionText(expression);
            string str2 = (metadata.Model == null) ? "" : metadata.Model.ToString();
            return MvcHtmlString.Create(GetTextBoxMvcHtml(expressionText, str2, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttribute), validateOptions, readOnly) + AddJs(helper));
        }

        private static string GetTextBoxMvcHtml(string name, string value, IDictionary<string, object> htmlAttribute, ValidateOptions validateOptions, bool readOnly)
        {
            TagBuilder builder = new TagBuilder("input");
            builder.GenerateId(name);
            builder.MergeAttribute("name", name);
            builder.MergeAttribute("type", "text");
            if (!string.IsNullOrEmpty(value))
            {
                builder.MergeAttribute("value", value);
            }
            if (readOnly)
            {
                builder.MergeAttribute("readonly", "readonly");
                string str = string.Empty;
                if ((htmlAttribute != null) && (htmlAttribute.Count > 0))
                {
                    if (htmlAttribute.ContainsKey("style"))
                    {
                        object obj2 = null;
                        if (htmlAttribute.TryGetValue("style", out obj2))
                        {
                            str = obj2.ToString().TrimEnd(new char[] { ';' });
                        }
                        str = str + ";background-color: #DFDFDF;";
                        htmlAttribute.Remove("style");
                        htmlAttribute.Add("style", str);
                    }
                    else
                    {
                        htmlAttribute.Add("style", "background-color: #DFDFDF");
                    }
                }
                else
                {
                    htmlAttribute.Add("style", "background-color: #DFDFDF");
                }
            }
            builder.MergeAttributes<string, object>(htmlAttribute);
            bool flag = false;
            if (validateOptions != null)
            {
                builder.MergeAttribute("data-bvalidator", ValidateCommon.GetValidateAttr(validateOptions));
                if (validateOptions.Required)
                {
                    flag = true;
                }
                if (!string.IsNullOrEmpty(validateOptions.ErrorMsg))
                {
                    builder.MergeAttribute("data-bvalidator-msg", validateOptions.ErrorMsg);
                }
            }
            return ((flag ? "<span class='star'>*</span>" : "<span class='star'>&nbsp</span>") + builder.ToString());
        }
    }
}

