namespace King.Framework.Mvc
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;

    public static class DatePicketExtensions
    {
        public static MvcHtmlString KingDatePicker(this HtmlHelper helper, string name, DateEnum dateEnum, string value = null, ValidateOptions validateOptions = null)
        {
            return GetDatePickerHtml(helper, name, dateEnum, value, validateOptions);
        }

        public static MvcHtmlString KingDatePickerFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, DateEnum dateEnum, string value = null, ValidateOptions validateOptions = null)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression<TModel, TProperty>(expression, helper.ViewData);
            string expressionText = ExpressionHelper.GetExpressionText(expression);
            if (metadata.Model != null)
            {
                DateTime time = Convert.ToDateTime(metadata.Model);
                value = (dateEnum == DateEnum.DateTime) ? time.ToString("yyyy-MM-dd HH:mm:ss") : time.ToString("yyyy-MM-dd");
            }
            return GetDatePickerHtml(helper, expressionText, dateEnum, value, validateOptions);
        }

        private static MvcHtmlString GetDatePickerHtml(HtmlHelper html, string name, DateEnum dateEnum, string value, ValidateOptions validateOptions)
        {
            DatePicketModel model = new DatePicketModel {
                Name = name,
                DateType = dateEnum
            };
            html.RenderPartial("~/Views/ControlScripts/_DatePicketScripts.cshtml", model);
            List<string> keyList = new List<string>();
            StringBuilder builder = new StringBuilder();
            TagBuilder builder2 = new TagBuilder("input");
            builder2.GenerateId(name);
            builder2.MergeAttribute("type", "text");
            builder2.MergeAttribute("name", name);
            builder2.MergeAttribute("readonly", "readonly");
            if (!string.IsNullOrEmpty(value))
            {
                builder2.MergeAttribute("value", value);
            }
            bool flag = false;
            if (validateOptions != null)
            {
                builder2.MergeAttribute("data-bvalidator", ValidateCommon.GetValidateAttr(validateOptions));
                if (validateOptions.Required)
                {
                    flag = true;
                }
                if (!string.IsNullOrEmpty(validateOptions.ErrorMsg))
                {
                    builder2.MergeAttribute("data-bvalidator-msg", validateOptions.ErrorMsg);
                }
            }
            builder.AppendLine(builder2.ToString());
            builder.AppendLine(CtrlScripts.RenderScript(html, keyList));
            return MvcHtmlString.Create((flag ? "<span class='star'>*</span>" : "<span class='star'>&nbsp</span>") + builder.ToString());
        }
    }
}

