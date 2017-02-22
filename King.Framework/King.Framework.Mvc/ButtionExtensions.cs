namespace King.Framework.Mvc
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Web.Mvc;

    public static class ButtionExtensions
    {
        public static MvcHtmlString KingButtion(this HtmlHelper htmlHepler, string name = null, string id = null, string beginIcon = null, string text = null, string endIcon = null, Validator validator = null, object htmlAttribute = null)
        {
            return GenerateHtml(htmlHepler, name, id, beginIcon, text, endIcon, validator, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttribute));
        }

        public static MvcHtmlString GenerateHtml(HtmlHelper htmlHepler, string name = null, string id = null, string beginIcon = null, string text = null, string endIcon = null, Validator validator = null, IDictionary<string, object> htmlAttributes = null)
        {
            if (validator == null)
            {
                validator = new Validator();
            }
            TagBuilder builder = new TagBuilder("input");
            builder.MergeAttribute("value", text);
            object obj2 = string.Empty;
            if (!htmlAttributes.TryGetValue("type", out obj2))
            {
                obj2 = "button";
            }
            builder.MergeAttribute("type", obj2.ToString());
            builder.MergeAttributes<string, object>(htmlAttributes);
            StringBuilder builder2 = new StringBuilder(0x100);
            if (string.IsNullOrEmpty(id))
            {
                id = "btn" + new Random().Next(1, 0x1869f).ToString();
            }
            if (string.IsNullOrEmpty(name))
            {
                name = id.ToString();
            }
            builder.MergeAttribute("id", id);
            builder.MergeAttribute("name", name);
            if ((validator != null) && validator.isValidator)
            {
                if (!(!validator.isValidator || string.IsNullOrEmpty(validator.validateGroup)))
                {
                    builder.MergeAttribute("onclick", string.Format("return ValidateGroup(\"{0}\");", validator.validateGroup));
                }
                else if (validator.isValidator && string.IsNullOrEmpty(validator.validateGroup))
                {
                    builder.MergeAttribute("onclick", string.Format("return Validate();", validator.validateGroup));
                }
            }
            else
            {
                builder.MergeAttribute("onclick", "return DialogShow();");
            }
            return MvcHtmlString.Create(RegisteredResources(htmlHepler) + builder.ToString() + builder2.ToString());
        }

        public static string RegisteredResources(HtmlHelper html)
        {
            return CtrlScripts.RenderScript(html, new List<string>());
        }
    }
}

