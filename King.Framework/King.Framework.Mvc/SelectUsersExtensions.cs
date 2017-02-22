namespace King.Framework.Mvc
{
    using King.Framework.Common;
    using King.Framework.EntityLibrary;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;
    using System.Web.Script.Serialization;

    public static class SelectUsersExtensions
    {
        public static void AddDepUser(SelectControlData item, List<SelectControlData> allDep, List<NameValues> userList)
        {
            IEnumerable<NameValues> collection = from p in userList
                where p.DepID == item.id
                select p;
            if (collection != null)
            {
                item.userlist.AddRange(collection);
            }
        }

        public static MvcHtmlString KingSelectUsers(this HtmlHelper helper, string name, string text = null, DataType DataType = DataType.All, string JSCallBackMethod = null, ButtonStyle buttonStyle = 0, SelectType selectType = SelectType.Single, bool Required = false, string value = null, int? currentUserID = new int?(), Type t = null, object parameter = null)
        {
            SelectUsersModel model = new SelectUsersModel {
                Name = name
            };
            helper.RenderPartial("~/Views/ControlScripts/_SelectUsersScripts.cshtml", model);
            return GetSelectUserHtml(helper, name, text, DataType, JSCallBackMethod, buttonStyle, selectType, Required, value, currentUserID, t, parameter);
        }

        private static string GetCompine(string name, string ext)
        {
            return string.Format("{0}_{1}", name, ext);
        }

        public static List<SelectControlData> GetData(DataType DataType, Type t, object parameter, int currentUseID)
        {
            List<SelectControlData> list = new List<SelectControlData>();
            List<NameValues> userList = new List<NameValues>();
            using (BizDataContext context = new BizDataContext(true))
            {
                if (DataType == DataType.All)
                {
                    List<T_User> list3 = context.Set<T_User>().ToList<T_User>();
                    userList = (from user in context.Set<T_User>()
                        join dep in context.Set<T_Department>() on user.Department_ID equals (int?) dep.Department_ID 
                        where (user.State == 0) && (dep.State == 0)
                        orderby user.User_Name
                        select new NameValues { Id = user.User_ID, Name = user.User_Name, DepID = user.Department_ID.ToInt(), DepName = dep.Department_Name, ParentDepID = dep.Parent_ID }).ToList<NameValues>();
                }
                if (DataType == DataType.CurrentDep)
                {
                    T_User currentUserModel = context.FindById<T_User>(new object[] { currentUseID });
                    if ((currentUserModel != null) && (currentUserModel.User_ID > 0))
                    {
                        userList = (from dep in context.Set<T_Department>()
                            join user in context.Set<T_User>() on dep.Department_ID equals user.Department_ID 
                            where (((dep.Department_ID == currentUserModel.Department_ID) && (user.State == 0)) && (dep.State == 0)) && (user.Department_ID != null)
                            orderby user.User_Name
                            select new NameValues { Id = user.User_ID, Name = user.User_Name, DepID = user.Department_ID.ToInt(), DepName = dep.Department_Name, ParentDepID = dep.Parent_ID }).ToList<NameValues>();
                    }
                }
                if (DataType == DataType.SubDep)
                {
                    T_User t_user = context.FindById<T_User>(new object[] { currentUseID });
                    if ((t_user != null) && (t_user.User_ID > 0))
                    {
                        T_Department currentDepModel = context.FindById<T_Department>(new object[] { t_user.Department_ID });
                        if ((currentDepModel != null) && (currentDepModel.Department_ID > 0))
                        {
                            userList = (from dep in context.Set<T_Department>()
                                join user in context.Set<T_User>() on dep.Department_ID equals user.Department_ID into user
                                where dep.SystemLevelCode.StartsWith(currentDepModel.SystemLevelCode)
                                select new NameValues { Id = t_user.User_ID, Name = t_user.User_Name, DepID = t_user.Department_ID.ToInt(), DepName = dep.Department_Name, ParentDepID = dep.Parent_ID }).ToList<NameValues>();
                        }
                    }
                }
                if (DataType == DataType.Custom)
                {
                    try
                    {
                        userList = ((ICustomSelectUser) Activator.CreateInstance(t)).GetCustomData(parameter);
                    }
                    catch (Exception exception)
                    {
                        AppLogHelper.Error(exception);
                    }
                }
            }
            List<SelectControlData> depList = new List<SelectControlData>();
            if ((userList != null) && (userList.Count > 0))
            {
                depList = (from p in userList select new SelectControlData { id = p.DepID, name = p.DepName, pId = p.ParentDepID, open = true, userlist = new List<NameValues>(), icon = "/Content/metro/images/selectuser_icon.png" }).ToList<SelectControlData>().Distinct<SelectControlData>(new List_DepId()).ToList<SelectControlData>();
                GetSelectControlData(userList, depList);
            }
            return depList;
        }

        public static void GetSelectControlData(List<NameValues> userList, List<SelectControlData> DepList)
        {
            List<SelectControlData> list = new List<SelectControlData>();
            if (DepList != null)
            {
                foreach (SelectControlData data in DepList)
                {
                    AddDepUser(data, DepList, userList);
                }
            }
        }

        private static MvcHtmlString GetSelectUserHtml(HtmlHelper html, string name, string text, DataType DataType, string JSCallBackMethod, ButtonStyle buttonStyle, SelectType selectType, bool Required, string value, int? currentUserID, Type t, object parameter)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            StringBuilder builder = new StringBuilder(800);
            TagBuilder builder2 = GetTagBuilder("div", name, "SelectUserDiv");
            TagBuilder builder3 = GetTagBuilder("div", name, "SelectUser");
            builder3.MergeAttribute("style", "display:none;");
            TagBuilder builder4 = GetTagBuilder("div", name, "SearchSelectDiv");
            string str = string.Format("<span class='dep' >部门</span>", new object[0]);
            builder4.InnerHtml = builder4.InnerHtml + str;
            TagBuilder builder5 = GetTagBuilder("select", name, "DepSelect");
            builder4.InnerHtml = builder4.InnerHtml + builder5;
            string str2 = string.Format("<span class='user' >用户</span>", new object[0]);
            builder4.InnerHtml = builder4.InnerHtml + str2;
            TagBuilder builder6 = GetTagBuilder("input", name, "UserSelect");
            builder4.InnerHtml = builder4.InnerHtml + builder6;
            TagBuilder builder7 = GetTagBuilder("input", name, "查询", "Search", "button");
            builder4.InnerHtml = builder4.InnerHtml + builder7;
            TagBuilder builder8 = GetTagBuilder("input", name, "重置", "Reset", "button");
            builder4.InnerHtml = builder4.InnerHtml + builder8;
            builder3.InnerHtml = builder3.InnerHtml + builder4;
            builder4 = GetTagBuilder("div", name, "SearchMoreDiv");
            TagBuilder builder9 = GetTagBuilder("input", name, "查询", "SearchMore", "button");
            builder4.InnerHtml = builder4.InnerHtml + builder9;
            builder3.InnerHtml = builder3.InnerHtml + builder4;
            string str3 = string.Format("<div class='WaitDepDiv'><span class='WaitDepSpan' >待选部门</span></div>", new object[0]);
            builder3.InnerHtml = builder3.InnerHtml + str3;
            string str4 = string.Format("<div class='WaitUserDiv'><span class='WaitUserSpan' >待选人员</span></div>", new object[0]);
            builder3.InnerHtml = builder3.InnerHtml + str4;
            string str5 = string.Format("<div class='SelectedUserDiv'><span class='SelectedUserSpan' >已选人员</span></div>", new object[0]);
            builder3.InnerHtml = builder3.InnerHtml + str5;
            builder4 = GetTagBuilder("div", name, "TreeDiv");
            TagBuilder builder10 = GetTagBuilder("ul", name, "ztree");
            builder4.InnerHtml = builder4.InnerHtml + builder10;
            builder3.InnerHtml = builder3.InnerHtml + builder4;
            builder4 = GetTagBuilder("div", name, "UserWaitSelectDiv");
            TagBuilder builder11 = GetTagBuilder("select", name, "UserWaitSelect");
            builder11.MergeAttribute("multiple", "multiple");
            builder4.InnerHtml = builder4.InnerHtml + builder11;
            builder3.InnerHtml = builder3.InnerHtml + builder4;
            builder4 = GetTagBuilder("div", name, "SelectButtonDiv");
            TagBuilder builder12 = GetTagBuilder("img", name, "SelectSingle");
            builder12.MergeAttribute("src", "/Content/metro/images/select_right2.png");
            builder4.InnerHtml = builder4.InnerHtml + builder12;
            if (selectType == SelectType.Multi)
            {
                builder12 = GetTagBuilder("img", name, "SelectAll");
                builder12.MergeAttribute("src", "/Content/metro/images/select_right.png");
                builder4.InnerHtml = builder4.InnerHtml + builder12;
            }
            builder12 = GetTagBuilder("img", name, "ReturnSingle");
            builder12.MergeAttribute("src", "/Content/metro/images/select_left2.png");
            builder4.InnerHtml = builder4.InnerHtml + builder12;
            if (selectType == SelectType.Multi)
            {
                builder12 = GetTagBuilder("img", name, "ReturnAll");
                builder12.MergeAttribute("src", "/Content/metro/images/select_left.png");
                builder4.InnerHtml = builder4.InnerHtml + builder12;
            }
            builder3.InnerHtml = builder3.InnerHtml + builder4;
            builder4 = GetTagBuilder("div", name, "UserSelectedDiv");
            TagBuilder builder13 = GetTagBuilder("select", name, "UserSelected");
            builder13.MergeAttribute("multiple", "multiple");
            builder4.InnerHtml = builder4.InnerHtml + builder13;
            builder3.InnerHtml = builder3.InnerHtml + builder4;
            TagBuilder builder14 = GetTagBuilder("div", name, "dataHF");
            builder14.MergeAttribute("style", "display:none;");
            List<SelectControlData> list = GetData(DataType, t, parameter, currentUserID.ToInt());
            builder14.InnerHtml = builder14.InnerHtml + serializer.Serialize(list);
            builder3.InnerHtml = builder3.InnerHtml + builder14;
            TagBuilder builder15 = GetTagBuilder("input", name, ((int) selectType).ToString(), "HidSelectType", "hidden");
            builder3.InnerHtml = builder3.InnerHtml + builder15;
            TagBuilder builder16 = GetTagBuilder("input", name, JSCallBackMethod, "HidCallBack", "hidden");
            builder3.InnerHtml = builder3.InnerHtml + builder16;
            TagBuilder hidUserId = GetTagBuilder("input", name, "HidUserId");
            hidUserId.MergeAttribute("type", "hidden");
            TagBuilder selectedIDHf = GetTagBuilder("input", name, "selectedIDHf");
            selectedIDHf.MergeAttribute("type", "hidden");
            TagBuilder userName = GetTagBuilder("input", name, "UserName");
            userName.MergeAttribute("type", "text");
            if (Required)
            {
                userName.MergeAttribute("data-bvalidator", "required");
                userName.MergeAttribute("data-bvalidator-msg", "请至少选人一个用户!");
            }
            userName.MergeAttribute("readonly", "true");
            SetDefaultValue(userName, hidUserId, selectedIDHf, value);
            builder3.InnerHtml = builder3.InnerHtml + hidUserId;
            builder3.InnerHtml = builder3.InnerHtml + selectedIDHf;
            string str6 = (buttonStyle == ButtonStyle.Normal) ? "" : "style='display:none'";
            string str7 = string.Empty;
            string str8 = string.Empty;
            if (buttonStyle == ButtonStyle.Normal)
            {
                userName.MergeAttribute("style", "float: left;");
                str7 = string.Format("<span class='col-9' {1} >{0}</span>", userName, str6);
                str8 = string.Format("<span class='col-3'><input class='selectButton' id='{0}_selectButton' \r\n                                            treename='{0}' type='button' value='{1}'></span>", name, text ?? "选择");
            }
            else
            {
                userName.MergeAttribute("style", "float: left;display:none");
                str7 = userName.ToString();
                str8 = string.Format("<input class='selectButton' id='{0}_selectButton' \r\n                                            treename='{0}' type='button' value='{1}'>", name, text ?? "选择");
            }
            builder2.InnerHtml = builder2.InnerHtml + str7;
            builder2.InnerHtml = builder2.InnerHtml + str8;
            builder2.InnerHtml = builder2.InnerHtml + builder3;
            builder.AppendLine(builder2.ToString());
            return MvcHtmlString.Create(builder.ToString());
        }

        private static TagBuilder GetTagBuilder(string tag, string name, string cssClass)
        {
            return GetTagBuilder(tag, name, "", cssClass, "");
        }

        private static TagBuilder GetTagBuilder(string tag, string name, string value, string cssClass)
        {
            return GetTagBuilder(tag, name, value, cssClass, "");
        }

        private static TagBuilder GetTagBuilder(string tag, string name, string value, string cssClass, string type)
        {
            TagBuilder builder = new TagBuilder(tag);
            builder.GenerateId(GetCompine(name, cssClass));
            if (!string.IsNullOrEmpty(value))
            {
                builder.MergeAttribute("value", value);
            }
            if (!string.IsNullOrEmpty(type))
            {
                builder.MergeAttribute("type", type);
            }
            builder.AddCssClass(cssClass);
            return builder;
        }

        public static List<NameValues> GetTest(int depId)
        {
            List<NameValues> list3 = new List<NameValues>();
            NameValues item = new NameValues {
                Id = 0x3e9,
                Name = "AAAA",
                DepID = 1,
                DepName = "开发部",
                ParentDepID = 0
            };
            list3.Add(item);
            NameValues values2 = new NameValues {
                Id = 0x3ea,
                Name = "BBBB",
                DepID = 1,
                DepName = "开发部",
                ParentDepID = 0
            };
            list3.Add(values2);
            NameValues values3 = new NameValues {
                Id = 0x3ee,
                Name = "BBAA",
                DepID = 1,
                DepName = "开发部",
                ParentDepID = 0
            };
            list3.Add(values3);
            NameValues values4 = new NameValues {
                Id = 0x3eb,
                Name = "CCCC",
                DepID = 2,
                DepName = "开发1部",
                ParentDepID = 1
            };
            list3.Add(values4);
            NameValues values5 = new NameValues {
                Id = 0x3ec,
                Name = "DDDD",
                DepID = 2,
                DepName = "开发1部",
                ParentDepID = 1
            };
            list3.Add(values5);
            NameValues values6 = new NameValues {
                Id = 0x3ed,
                Name = "EEEE",
                DepID = 2,
                DepName = "开发1部",
                ParentDepID = 1
            };
            list3.Add(values6);
            List<NameValues> list = list3;
            return (from n in list
                where n.DepID == depId
                select n).ToList<NameValues>();
        }

        public static List<SelectControlData> GetTestData()
        {
            List<SelectControlData> list = new List<SelectControlData>();
            SelectControlData item = new SelectControlData {
                id = 1,
                pId = 0,
                name = "开发部",
                userlist = GetTest(1)
            };
            list.Add(item);
            SelectControlData data2 = new SelectControlData {
                id = 2,
                pId = 1,
                name = "开发1部",
                userlist = GetTest(2)
            };
            list.Add(data2);
            SelectControlData data3 = new SelectControlData {
                id = 3,
                pId = 0,
                name = "人事部",
                userlist = GetTest(3)
            };
            list.Add(data3);
            return list;
        }

        public static void SetDefaultValue(TagBuilder UserName, TagBuilder HidUserId, TagBuilder selectedIDHf, string selectValue)
        {
            string str = string.Empty;
            StringBuilder builder = new StringBuilder(100);
            if (!string.IsNullOrEmpty(selectValue))
            {
                string[] strArray = selectValue.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                using (BizDataContext context = new BizDataContext(true))
                {
                    var source = (from u in context.FetchAll<T_User>()
                        join suid in strArray on u.User_ID.ToString() equals suid
                        select new { User_ID = u.User_ID, User_Name = u.User_Name }).ToList();
                    List<string> values = new List<string>();
                    if ((source != null) && (source.Count > 0))
                    {
                        foreach (string str2 in strArray)
                        {
                            int id = str2.ToInt();
                            string item = source.FirstOrDefault(a => (a.User_ID == id)).User_Name;
                            values.Add(item);
                            builder.Append(str2 + "," + item + ";");
                        }
                        str = string.Join(",", values);
                    }
                }
                UserName.MergeAttribute("value", str);
                HidUserId.MergeAttribute("value", selectValue);
                selectedIDHf.MergeAttribute("value", builder.ToString().TrimEnd(new char[] { ';' }));
            }
        }
    }
}

