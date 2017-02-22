namespace King.Framework.Mvc
{
    using King.Framework.Common;
    using King.Framework.EntityLibrary;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using System.Web.Mvc;
    using System.Web.Script.Serialization;

    public static class SelectUserExtensions
    {
        private static List<ITreeData> DealWithNoteOpenState(List<ITreeData> data, OpenNoteEnum openNote)
        {
            switch (openNote)
            {
                case OpenNoteEnum.Root:
                    foreach (ITreeData data2 in data)
                    {
                        data2.open = data2.pId <= 0;
                    }
                    return data;

                case OpenNoteEnum.None:
                    data.ForEach(delegate (ITreeData a) {
                        a.open = false;
                    });
                    return data;

                case OpenNoteEnum.All:
                    data.ForEach(delegate (ITreeData a) {
                        a.open = true;
                    });
                    return data;
            }
            return data;
        }

        private static string GetCompine(string name, string ext)
        {
            return string.Format("{0}_{1}", name, ext);
        }

        private static MvcHtmlString GetSelectUserHtml(HtmlHelper html, string name, List<ITreeData> data, string GetLeafURL, string CallBackMethod, UserStyle userStyle, OpenNoteEnum openNote, ValidateOptions validateOptions, string value)
        {
            BizDataContext context;
            List<string> keyList = new List<string>();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            StringBuilder builder = new StringBuilder();
            TagBuilder builder2 = new TagBuilder("div");
            builder2.GenerateId(name);
            builder2.AddCssClass("treeDiv");
            TagBuilder builder3 = GetTagBuilder("ul", name, "ztree");
            builder3.MergeAttribute("style", "display : none;");
            builder2.InnerHtml = builder2.InnerHtml + builder3;
            TagBuilder builder4 = GetTagBuilder("div", name, "dataHF");
            builder4.MergeAttribute("style", "display:none;");
            if ((data != null) && (data.Count > 0))
            {
                data = DealWithNoteOpenState(data, openNote);
                builder4.InnerHtml = builder4.InnerHtml + serializer.Serialize(data);
            }
            else
            {
                using (context = new BizDataContext(true))
                {
                    ParameterExpression expression;
                   // List<ITreeData> list2 = DealWithNoteOpenState(context.Set<T_Department>().
                    //    Select<T_Department, TreeData>(Expression.Lambda<Func<T_Department, TreeData>>(Expression.MemberInit(Expression.New((ConstructorInfo) methodof(TreeData..ctor), new Expression[0]), new MemberBinding[] { Expression.Bind((MethodInfo) methodof(TreeData.set_id), Expression.Property(expression = Expression.Parameter(typeof(T_Department), "u"), (MethodInfo) methodof(T_Department.get_Department_ID))), Expression.Bind((MethodInfo) methodof(TreeData.set_name), Expression.Property(expression, (MethodInfo) methodof(T_Department.get_Department_Name))), Expression.Bind((MethodInfo) methodof(TreeData.set_open), Expression.Constant(false, typeof(bool))), Expression.Bind((MethodInfo) methodof(TreeData.set_pId), Expression.Coalesce(Expression.Property(expression, (MethodInfo) methodof(T_Department.get_Parent_ID)), Expression.Constant(-1, typeof(int)))) }), new ParameterExpression[] { expression })).ToList<ITreeData>(), openNote);
                    //builder4.InnerHtml = builder4.InnerHtml + serializer.Serialize(list2);
                }
            }
            builder2.InnerHtml = builder2.InnerHtml + builder4;
            TagBuilder builder5 = GetTagBuilder("input", name, "resultInput");
            if (userStyle == UserStyle.Normal)
            {
                builder5.MergeAttribute("style", "float: left;");
            }
            else
            {
                builder5.MergeAttribute("style", "float: left;display:none");
            }
            builder5.MergeAttribute("type", "text");
            if (validateOptions != null)
            {
                builder5.MergeAttribute("data-bvalidator", ValidateCommon.GetValidateAttr(validateOptions));
                if (!string.IsNullOrEmpty(validateOptions.ErrorMsg))
                {
                    builder5.MergeAttribute("data-bvalidator-msg", validateOptions.ErrorMsg);
                }
            }
            builder5.MergeAttribute("readonly", "true");
            TagBuilder builder6 = GetTagBuilder("input", name, "resultHf");
            builder6.MergeAttribute("type", "hidden");
            builder2.InnerHtml = builder2.InnerHtml + builder5;
            builder2.InnerHtml = builder2.InnerHtml + builder6;
            TagBuilder builder7 = GetTagBuilder("input", name, "treeButton");
            builder7.MergeAttribute("type", "button");
            builder7.MergeAttribute("value", "选择");
            builder7.MergeAttribute("treeName", name);
            builder2.InnerHtml = builder2.InnerHtml + builder7;
            TagBuilder builder8 = GetTagBuilder("input", name, "selectedIDHf");
            builder8.MergeAttribute("type", "hidden");
            string str = string.Empty;
            if (!string.IsNullOrEmpty(value))
            {
                string[] strArray = value.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                using (context = new BizDataContext(true))
                {
                    IEnumerable<string> values = from u in context.FetchAll<T_User>()
                        join suid in strArray on u.User_ID.ToString() equals suid
                        select u.User_Name;
                    str = string.Join(",", values);
                }
                builder8.MergeAttribute("value", value);
            }
            builder2.InnerHtml = builder2.InnerHtml + builder8;
            TagBuilder builder9 = GetTagBuilder("input", name, "selectedNameHF");
            builder9.MergeAttribute("type", "hidden");
            builder9.MergeAttribute("value", str);
            builder2.InnerHtml = builder2.InnerHtml + builder9;
            TagBuilder builder10 = GetTagBuilder("input", name, "configDataHF");
            builder10.MergeAttribute("type", "hidden");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("GetLeafURL", GetLeafURL);
            dictionary.Add("CallBackMethod", CallBackMethod);
            builder10.MergeAttribute("value", serializer.Serialize(dictionary));
            builder2.InnerHtml = builder2.InnerHtml + builder10;
            builder.AppendLine(builder2.ToString());
            builder.AppendLine(CtrlScripts.RenderScript(html, keyList));
            return MvcHtmlString.Create(builder.ToString());
        }

        private static TagBuilder GetTagBuilder(string tag, string name, string ext)
        {
            TagBuilder builder = new TagBuilder(tag);
            builder.GenerateId(GetCompine(name, ext));
            builder.AddCssClass(ext);
            return builder;
        }
    }
}

