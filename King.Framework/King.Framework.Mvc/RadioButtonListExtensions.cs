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
    using System.Web.UI.WebControls;

    public static class RadioButtonListExtensions
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

        public static MvcHtmlString KingRadioButtonList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> dataSource, ValidateOptions validateOption = null, object htmlAttribute = null, RepeatDirection repeatDirection = 0, bool IsDefaultStyle = true, bool readOnly = false)
        {
            return MvcHtmlString.Create(GenerateHtml(name, dataSource, validateOption, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttribute), repeatDirection, null, IsDefaultStyle, readOnly) + AddJs(htmlHelper));
        }

        public static MvcHtmlString KingRadioButtonList(this HtmlHelper htmlHelper, string name, Type enumType, string defaultValue = null, ValidateOptions validateOption = null, object htmlAttribute = null, RepeatDirection repeatDirection = 0, bool IsDefaultStyle = true, bool readOnly = false)
        {
            List<SelectListItem> enumToSelectList = MvcHtmlStringCommon.GetEnumToSelectList(enumType);
            return MvcHtmlString.Create(GenerateHtml(name, enumToSelectList, validateOption, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttribute), repeatDirection, defaultValue, IsDefaultStyle, readOnly) + AddJs(htmlHelper));
        }

        public static MvcHtmlString KingRadioButtonListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string name, ValidateOptions validateOption = null, RepeatDirection repeatDirection = 0, object htmlAttributes = null, bool IsDefaultStyle = true, bool readOnly = false)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression<TModel, TProperty>(expression, htmlHelper.ViewData);
            string expressionText = ExpressionHelper.GetExpressionText(expression);
            IDictionary<string, object> unobtrusiveValidationAttributes = htmlHelper.GetUnobtrusiveValidationAttributes(expressionText, metadata);
            string fullHtmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(expressionText);
            IEnumerable<SelectListItem> items = null;
            string defaultValue = "";
            Type modelType = ValidateCommon.GetModelType(metadata.ModelType);
            if (modelType.IsEnum)
            {
                items = new SelectList(MvcHtmlStringCommon.GetEnumToSelectList(modelType).AsEnumerable<SelectListItem>(), "Value", "Text");
            }
            else if (modelType == typeof(bool))
            {
                if (metadata.Model != null)
                {
                    defaultValue = ((bool) metadata.Model) ? "1" : "0";
                }
                else
                {
                    defaultValue = "0";
                }
                SelectListItem item = new SelectListItem {
                    Value = "1",
                    Text = "是"
                };
                SelectListItem item2 = new SelectListItem {
                    Value = "0",
                    Text = "否"
                };
                items = new List<SelectListItem> {
                    item,
                    item2
                };
            }
            return MvcHtmlString.Create(GenerateHtml(name, items, validateOption, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), repeatDirection, defaultValue, IsDefaultStyle, readOnly) + AddJs(htmlHelper));
        }

        public static MvcHtmlString KingRadioButtonListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string name, IEnumerable<SelectListItem> dataScource, ValidateOptions validateOption = null, RepeatDirection repeatDirection = 0, object htmlAttributes = null, bool IsDefaultStyle = true, bool readOnly = false)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression<TModel, TProperty>(expression, htmlHelper.ViewData);
            string expressionText = ExpressionHelper.GetExpressionText(expression);
            IDictionary<string, object> unobtrusiveValidationAttributes = htmlHelper.GetUnobtrusiveValidationAttributes(expressionText, metadata);
            string fullHtmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(expressionText);
            string defaultValue = "";
            if (ValidateCommon.GetModelType(metadata.ModelType) == typeof(bool))
            {
                if (metadata.Model != null)
                {
                    defaultValue = ((bool) metadata.Model) ? "1" : "0";
                }
                else
                {
                    defaultValue = "0";
                }
            }
            else
            {
                defaultValue = (metadata.Model == null) ? "" : metadata.Model.ToString();
            }
            return MvcHtmlString.Create(GenerateHtml(name, dataScource, validateOption, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), repeatDirection, defaultValue, IsDefaultStyle, readOnly) + AddJs(htmlHelper));
        }

        public static MvcHtmlString KingRadioButtonListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string name, Type enumType, string defaultValue = null, ValidateOptions validateOption = null, RepeatDirection repeatDirection = 0, object htmlAttribute = null, bool IsDefaultStyle = true, bool readOnly = false)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression<TModel, TProperty>(expression, htmlHelper.ViewData);
            string expressionText = ExpressionHelper.GetExpressionText(expression);
            IDictionary<string, object> unobtrusiveValidationAttributes = htmlHelper.GetUnobtrusiveValidationAttributes(expressionText, metadata);
            string fullHtmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(expressionText);
            string str3 = "";
            if (ValidateCommon.GetModelType(metadata.ModelType) == typeof(bool))
            {
                if (metadata.Model != null)
                {
                    str3 = ((bool) metadata.Model) ? "1" : "0";
                }
                else
                {
                    str3 = "0";
                }
            }
            else
            {
                str3 = (metadata.Model == null) ? "" : metadata.Model.ToString();
            }
            defaultValue = string.IsNullOrEmpty(str3) ? defaultValue : str3;
            List<SelectListItem> enumToSelectList = MvcHtmlStringCommon.GetEnumToSelectList(enumType);
            return MvcHtmlString.Create(GenerateHtml(name, enumToSelectList, validateOption, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttribute), repeatDirection, defaultValue, IsDefaultStyle, readOnly) + AddJs(htmlHelper));
        }

        public static string GenerateHtml(string name, IEnumerable<SelectListItem> items, ValidateOptions validateOptions, IDictionary<string, object> htmlAttributes, RepeatDirection repeatDirection = 0, object defaultValue = null, bool IsDefaultStyle = true, bool readOnly = false)
        {
            TagBuilder builder = new TagBuilder("div");
            builder.MergeAttribute("id", name);
            int num = 0;
            bool isChecked = false;
            foreach (SelectListItem item in items)
            {
                isChecked = (defaultValue == null) ? item.Selected : (defaultValue.ToString() == item.Value);
                num++;
                string id = string.Format("{0}_{1}", name, num);
                builder.InnerHtml = builder.InnerHtml + GenerateRadioHtml(name, id, item.Text, item.Value, isChecked, htmlAttributes, (num == items.Count<SelectListItem>()) ? validateOptions : null, readOnly);
            }
            if (!IsDefaultStyle)
            {
                builder.InnerHtml = builder.InnerHtml + "<script type='text/javascript'>";
                builder.InnerHtml = builder.InnerHtml + string.Format("$('#{0}').buttonset();", name);
                builder.InnerHtml = builder.InnerHtml + "</script>";
            }
            return builder.ToString();
        }

        public static string GenerateRadioHtml(string name, string id, string labelText, string value, bool isChecked, IDictionary<string, object> htmlAttributes, ValidateOptions validateOptions, bool readOnly)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("<div class='radio_label'>");
            TagBuilder builder2 = new TagBuilder("label");
            builder2.MergeAttribute("for", id);
            builder2.SetInnerText(labelText);
            TagBuilder builder3 = new TagBuilder("input");
            builder3.GenerateId(id);
            builder3.MergeAttribute("name", name);
            builder3.MergeAttribute("custom", "big");
            builder3.MergeAttribute("type", "radio");
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
    }
}

