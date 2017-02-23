namespace King.Sys_Common.BLL
{
    using King.Framework.Common;
    using King.Framework.EntityLibrary;
    using King.Framework.Interfaces;
    using King.Sys_Common.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class MenuBLL : BaseBLL
    {
        public MenuBLL()
        {
        }

        public MenuBLL(IUserIdentity user) : base(user)
        {
        }

        public string AddModuleAndFunctionId(SysFunction p, string url)
        {
            if (url.IndexOf('?') == -1)
            {
                url = string.Format("{0}", url, p.Permission_Type, p.Function_ID);
                return url;
            }
            url = string.Format("{0}", url, p.Permission_Type, p.Function_ID);
            return url;
        }
        #region 根据用户id获取Menu
        //private List<MenuModel> GetFunctionListByUser(int userId, string ssotoken)
        //{
        //    List<T_User_Role> list2;
        //    Func<SysFunction, MenuModel> selector = null;
        //    List<T_Role_Function> roleFuncList;
        //    List<MenuModel> funcList;
        //    int appUserState = 1;
        //    using (BizDataContext context = new BizDataContext(true))
        //    {
        //        List<T_Role> list = context.FetchAll<T_Role>();
        //        roleFuncList = context.FetchAll<T_Role_Function>();
        //        list2 = context.FetchAll<T_User_Role>();
        //        if (selector == null)
        //        {
        //            selector = p => new MenuModel(p, ssotoken);
        //        }
        //        funcList = context.Where<SysFunction>(p => p.FunctionType == 0).Select<SysFunction, MenuModel>(selector).ToList<MenuModel>();
        //        if (BizDataContext.UseConnectionProvider)
        //        {
        //            List<int> appIdList = (from a in context.Where<T_Application_User>(a => (a.OwnerId == userId) && (a.State == appUserState)) select a.Application_ID).ToList<int>();
        //            List<SysFunction> list3 = context.Where<SysFunction>(s => ((s.FunctionType == 0) && ((s.Permission_Type == null) || (s.Permission_Type == 0L))) && ((s.IsRelateApp != null) && (s.IsRelateApp == 1)));
        //            List<long> deleteIdlist = (from s in list3
        //                where s.AppId.HasValue && !appIdList.Contains(s.AppId.Value.ToInt())
        //                select s.Function_ID).ToList<long>();
        //            funcList.RemoveAll(p => deleteIdlist.Contains(p.Function_ID));
        //        }
        //    }
        //    Dictionary<long, MenuModel> dictionary = funcList.ToDictionary<MenuModel, long, MenuModel>(p => p.Function_ID, p => p);
        //    List<MenuModel> collection = (from <>h__TransparentIdentifier1 in (from ur in list2
        //        from rf in roleFuncList
        //        from f in funcList
        //        select new { <>h__TransparentIdentifier0 = <>h__TransparentIdentifier0, f = f }).Where(delegate (<>f__AnonymousType1<<>f__AnonymousType0<T_User_Role, T_Role_Function>, MenuModel> <>h__TransparentIdentifier1) {
        //        long? nullable;
        //        long num;
        //        if (<>h__TransparentIdentifier1.<>h__TransparentIdentifier0.ur.User_Id == userId)
        //        {
        //            nullable = <>h__TransparentIdentifier1.<>h__TransparentIdentifier0.rf.Role_ID;
        //            num = <>h__TransparentIdentifier1.<>h__TransparentIdentifier0.ur.Role_Id;
        //        }
        //        return ((nullable.GetValueOrDefault() == num) && nullable.HasValue) && (((num = <>h__TransparentIdentifier1.f.Function_ID) == (nullable = <>h__TransparentIdentifier1.<>h__TransparentIdentifier0.rf.Function_ID).GetValueOrDefault()) && nullable.HasValue);
        //    }) select <>h__TransparentIdentifier1.f).Distinct<MenuModel>().ToList<MenuModel>();
        //    Stack<MenuModel> stack = new Stack<MenuModel>(collection);
        //    HashSet<MenuModel> source = new HashSet<MenuModel>(collection);
        //    while (stack.Count > 0)
        //    {
        //        MenuModel item = stack.Pop();
        //        if ((item.Permission_Type.HasValue && (item.Permission_Type.Value > 0L)) && dictionary.ContainsKey(item.Permission_Type.Value))
        //        {
        //            MenuModel model2 = dictionary[item.Permission_Type.Value];
        //            model2.Children.Add(item);
        //            item.Parent = model2;
        //            if (!source.Contains(model2))
        //            {
        //                source.Add(model2);
        //                stack.Push(model2);
        //            }
        //        }
        //    }
        //    foreach (MenuModel model3 in source)
        //    {
        //        if (model3.Children.Count > 1)
        //        {
        //            model3.Children.Sort();
        //        }
        //    }
        //    return (from p in source.Where<MenuModel>(delegate (MenuModel p) {
        //        long? nullable;
        //        return !p.Permission_Type.HasValue || (((nullable = p.Permission_Type).GetValueOrDefault() == 0L) && nullable.HasValue);
        //    })
        //        orderby p.OrderIndex
        //        select p).ToList<MenuModel>();
        //}
#endregion

        public string GetPageUrl(MenuModel p)
        {
            return p.URL;
        }

        public string GetPageUrl(BizDataContext db, SysEntity entity, SysPage page)
        {
            string str = string.Empty;
            if ((page != null) && page.ModuleId.HasValue)
            {
                page.OwnerModule = db.FindById<SysModule>(new object[] { page.ModuleId });
                if (page.OwnerModule != null)
                {
                    page.OwnerModule.EntityCategory = db.FindById<SysEntityCategory>(new object[] { page.OwnerModule.CategoryId });
                    if (page.OwnerModule.EntityCategory != null)
                    {
                        str = string.Format("~/{0}_{1}/{2}", page.OwnerModule.EntityCategory.CategoryName, page.OwnerModule.ModuleName, page.PageName);
                    }
                }
                return str;
            }
            if (entity != null)
            {
                entity.OwnerModule = db.FindById<SysModule>(new object[] { entity.ModuleId });
                if (entity.OwnerModule == null)
                {
                    return str;
                }
                entity.OwnerModule.EntityCategory = db.FindById<SysEntityCategory>(new object[] { entity.OwnerModule.CategoryId });
                if (entity.OwnerModule.EntityCategory != null)
                {
                    str = string.Format("~/{0}_{1}/{2}", entity.OwnerModule.EntityCategory.CategoryName, entity.OwnerModule.ModuleName, page.PageName);
                }
            }
            return str;
        }

        public List<MenuModel> GetRootMenus()
        {
            return null;
            //return this.GetFunctionListByUser(base.CurrentUser.User_ID, base.CurrentUser.SsoToken);
        }

        private class MenuModelComparer : IComparer<MenuModel>
        {
            public int Compare(MenuModel x, MenuModel y)
            {
                int? orderIndex = y.OrderIndex;
                orderIndex = x.OrderIndex;
                return ((orderIndex.HasValue ? orderIndex.GetValueOrDefault() : 0) - (orderIndex.HasValue ? orderIndex.GetValueOrDefault() : 0));
            }
        }
    }
}

