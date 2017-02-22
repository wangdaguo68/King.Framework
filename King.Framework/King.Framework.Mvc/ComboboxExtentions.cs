namespace King.Framework.Mvc
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;

    public static class ComboboxExtentions
    {
        public static readonly List<string> listJS = new List<string> { CtrlScripts.bValidate, CtrlScripts.JqueryBvalidatorCss, CtrlScripts.combobox, CtrlScripts.ComboboxCss };

        public static string AddJs(HtmlHelper html)
        {
            return "";
        }

        public static string AddJs<TModel, TValue>(HtmlHelper<TModel> html)
        {
            return "";
        }

        public static MvcHtmlString KingCombobox(this HtmlHelper html, string name, IEnumerable<SelectListItem> list, string Value = null, bool IsShowEmptyItem = true, ValidateOptions validateOptions = null, object htmlAttribute = null, string onselect = null)
        {
            return MvcHtmlString.Create(GetSelect(html, name, Value, IsShowEmptyItem, htmlAttribute, list, validateOptions, onselect) + AddJs(html));
        }

        public static MvcHtmlString KingCombobox(this HtmlHelper html, string name, Type enumType, string Value = null, bool IsShowEmptyItem = true, ValidateOptions validateOptions = null, object htmlAttribute = null, string onselect = null)
        {
            List<SelectListItem> enumToSelectList = MvcHtmlStringCommon.GetEnumToSelectList(enumType);
            return MvcHtmlString.Create(GetSelect(html, name, Value, IsShowEmptyItem, htmlAttribute, enumToSelectList, validateOptions, onselect) + AddJs(html));
        }

        public static MvcHtmlString KingComboboxFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string name = null, string Value = null, bool IsShowEmptyItem = true, ValidateOptions validateOptions = null, object htmlAttribute = null, string onselect = null)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression<TModel, TValue>(expression, html.ViewData);
            if (string.IsNullOrEmpty(name))
            {
                name = ExpressionHelper.GetExpressionText(expression);
            }
            if (string.IsNullOrEmpty(Value) && (metadata.Model != null))
            {
                Value = metadata.Model.ToString();
            }
            IEnumerable<SelectListItem> list = null;
            Type modelType = ValidateCommon.GetModelType(metadata.ModelType);
            if (modelType.IsEnum)
            {
                list = MvcHtmlStringCommon.GetEnumToSelectList(modelType);
            }
            else if (modelType == typeof(bool))
            {
                SelectListItem item = new SelectListItem {
                    Value = "1",
                    Text = "是"
                };
                SelectListItem item2 = new SelectListItem {
                    Value = "0",
                    Text = "否"
                };
                list = new List<SelectListItem> {
                    item,
                    item2
                };
            }
            return MvcHtmlString.Create(GetSelect(html, name, Value, IsShowEmptyItem, htmlAttribute, list, validateOptions, onselect) + AddJs<TModel, TValue>(html));
        }

        public static MvcHtmlString KingComboboxFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IEnumerable<SelectListItem> list, string name = null, string Value = null, bool IsShowEmptyItem = true, ValidateOptions validateOptions = null, object htmlAttribute = null, string onselect = null)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression<TModel, TValue>(expression, html.ViewData);
            if (string.IsNullOrEmpty(name))
            {
                name = ExpressionHelper.GetExpressionText(expression);
            }
            if (string.IsNullOrEmpty(Value) && (metadata.Model != null))
            {
                Value = metadata.Model.ToString();
            }
            return MvcHtmlString.Create(GetSelect(html, name, Value, IsShowEmptyItem, htmlAttribute, list, validateOptions, onselect) + AddJs<TModel, TValue>(html));
        }

        public static MvcHtmlString KingComboboxFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, Type enumType, string name = null, string Value = null, bool IsShowEmptyItem = true, ValidateOptions validateOptions = null, object htmlAttribute = null, string onselect = null)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression<TModel, TValue>(expression, html.ViewData);
            if (string.IsNullOrEmpty(name))
            {
                name = ExpressionHelper.GetExpressionText(expression);
            }
            if (string.IsNullOrEmpty(Value) && (metadata.Model != null))
            {
                Value = metadata.Model.ToString();
            }
            IEnumerable<SelectListItem> list = null;
            if (enumType != null)
            {
                list = MvcHtmlStringCommon.GetEnumToSelectList(enumType);
            }
            return MvcHtmlString.Create(GetSelect(html, name, Value, IsShowEmptyItem, htmlAttribute, list, validateOptions, onselect) + AddJs<TModel, TValue>(html));
        }

        private static string GetSelect(HtmlHelper html, string name, string Value, bool IsShowEmptyItem, object htmlAttribute, IEnumerable<SelectListItem> list, ValidateOptions validateOptions, string onselect)
        {
            TagBuilder builder2;
            ComboboxModel model = new ComboboxModel {
                Name = name.Replace(".", "_"),
                SelectedFunc = (onselect == null) ? "" : onselect
            };
            html.RenderPartial("~/Views/ControlScripts/_ComboboxScripts.cshtml", model);
            TagBuilder builder = new TagBuilder("select");
            builder.GenerateId(name);
            builder.MergeAttribute("name", name);
            builder.MergeAttributes<string, object>(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttribute));
            if (validateOptions != null)
            {
                builder.MergeAttribute("data-bvalidator", ValidateCommon.GetValidateAttr(validateOptions));
                if (!string.IsNullOrEmpty(validateOptions.ErrorMsg))
                {
                    builder.MergeAttribute("data-bvalidator-msg", validateOptions.ErrorMsg);
                }
            }
            if (IsShowEmptyItem)
            {
                builder2 = new TagBuilder("option");
                if (string.IsNullOrEmpty(Value))
                {
                    builder2.MergeAttribute("selected", "selected");
                }
                builder2.MergeAttribute("value", " ");
                builder2.SetInnerText("--请选择--");
                builder.InnerHtml = builder.InnerHtml + builder2.ToString();
            }
            foreach (SelectListItem item in list)
            {
                builder2 = new TagBuilder("option");
                builder2.MergeAttribute("value", item.Value);
                builder2.SetInnerText(item.Text);
                if (string.IsNullOrEmpty(Value) && item.Selected)
                {
                    Value = item.Value;
                }
                if (item.Value == Value)
                {
                    builder2.MergeAttribute("selected", "selected");
                }
                builder.InnerHtml = builder.InnerHtml + builder2.ToString();
            }
            bool flag = false;
            if ((validateOptions != null) && validateOptions.Required)
            {
                flag = true;
            }
            return ((flag ? "<span class='star'>*</span>" : "<span class='star'>&nbsp</span>") + builder.ToString());
        }
    }
}

