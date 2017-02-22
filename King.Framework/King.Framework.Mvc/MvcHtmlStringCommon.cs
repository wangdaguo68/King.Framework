namespace King.Framework.Mvc
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Web.Mvc;
    using System.Web.UI.WebControls;

    public class MvcHtmlStringCommon
    {
        public static MvcHtmlString GenerateHtml(string name, IEnumerable<SelectListItem> items, IDictionary<string, object> htmlAttributes, RepeatDirection repeatDirection = 0, string defaultValue = null)
        {
            return GenerateHtml(name, items, "radio", htmlAttributes, repeatDirection, defaultValue);
        }

        public static MvcHtmlString GenerateHtml(string name, IEnumerable<SelectListItem> items, string typeName, IDictionary<string, object> htmlAttributes, RepeatDirection repeatDirection = 0, object defaultValue = null)
        {
            TagBuilder builder2;
            IEnumerable<string> enumerable;
            string str;
            TagBuilder builder = new TagBuilder("table");
            int num = 0;
            bool isChecked = false;
            bool flag2 = typeName == "radio";
            if (repeatDirection == RepeatDirection.Horizontal)
            {
                builder2 = new TagBuilder("tr");
                foreach (SelectListItem item in items)
                {
                    if (flag2)
                    {
                        isChecked = (defaultValue == null) ? item.Selected : (defaultValue.ToString() == item.Value);
                    }
                    else
                    {
                        enumerable = defaultValue as IEnumerable<string>;
                        isChecked = (enumerable != null) && enumerable.Contains<string>(item.Value);
                    }
                    num++;
                    str = string.Format("{0}_{1}", name, num);
                    TagBuilder builder3 = new TagBuilder("td") {
                        InnerHtml = GenerateRadioHtml(name, str, item.Text, item.Value, isChecked, htmlAttributes, typeName)
                    };
                    builder2.InnerHtml = builder2.InnerHtml + builder3.ToString();
                }
                builder.InnerHtml = builder2.ToString();
            }
            else
            {
                foreach (SelectListItem item in items)
                {
                    builder2 = new TagBuilder("tr");
                    if (flag2)
                    {
                        isChecked = (defaultValue == null) ? item.Selected : (defaultValue.ToString() == item.Value);
                    }
                    else
                    {
                        enumerable = defaultValue as IEnumerable<string>;
                        isChecked = (enumerable != null) && enumerable.Contains<string>(item.Value);
                    }
                    num++;
                    str = string.Format("{0}_{1}", name, num);
                    builder2.InnerHtml = new TagBuilder("td") { InnerHtml = GenerateRadioHtml(name, str, item.Text, item.Value, isChecked, htmlAttributes, typeName) }.ToString();
                    builder.InnerHtml = builder.InnerHtml + builder2.ToString();
                }
            }
            return new MvcHtmlString(builder.ToString());
        }

        public static string GenerateRadioHtml(string name, string id, string labelText, string value, bool isChecked, IDictionary<string, object> htmlAttributes, string typeName)
        {
            StringBuilder builder = new StringBuilder();
            TagBuilder builder2 = new TagBuilder("label");
            builder2.MergeAttribute("for", id);
            builder2.SetInnerText(labelText);
            TagBuilder builder3 = new TagBuilder("input");
            builder3.GenerateId(id);
            builder3.MergeAttribute("name", name);
            builder3.MergeAttribute("type", typeName);
            builder3.MergeAttribute("value", value);
            builder3.MergeAttributes<string, object>(htmlAttributes);
            if (isChecked)
            {
                builder3.MergeAttribute("checked", "checked");
            }
            builder.AppendLine(builder3.ToString());
            builder.AppendLine(builder2.ToString());
            return builder.ToString();
        }

        public static string GetEnumDesc(Enum e)
        {
            if (e.Equals(0))
            {
                return "";
            }
            FieldInfo field = e.GetType().GetField(e.ToString());
            if (field != null)
            {
                DescriptionAttribute[] customAttributes = (DescriptionAttribute[]) field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (customAttributes.Length > 0)
                {
                    return customAttributes[0].Description;
                }
            }
            return e.ToString();
        }

        public static List<SelectListItem> GetEnumToSelectList(Type enumType)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            SelectListItem item = null;
            Array values = Enum.GetValues(enumType);
            foreach (object obj2 in values)
            {
                SelectListItem item2 = new SelectListItem {
                    Text = GetEnumDesc((Enum) obj2)
                };
                item2.Value = ((int) obj2).ToString();
                item = item2;
                list.Add(item);
            }
            return list;
        }
    }
}

