namespace King.Sys_Common.BLL
{
    using King.Framework.Common;
    using King.Framework.EntityLibrary;
    using King.Framework.Interfaces;
    using King.Framework.Ioc;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;

    public class WorkItemBLL
    {
        public bool ChangeWorkItemState(int id, T_WorkItemBaseStateEnum state, out string errorMsg)
        {
            bool flag;
            errorMsg = string.Empty;
            try
            {
                using (BizDataContext context = new BizDataContext(true))
                {
                    T_WorkItemBase base2 = context.FindById<T_WorkItemBase>(new object[] { id });
                    if ((base2 != null) && (base2.WorkItemBase_Id > 0))
                    {
                        base2.State = new int?((int) state);
                        context.UpdatePartial<T_WorkItemBase>(base2, p => new { State = p.State });
                        return true;
                    }
                    errorMsg = "未获取到T_WorkItemBase实体";
                    flag = false;
                }
            }
            catch (Exception exception)
            {
                errorMsg = exception.Message;
                flag = false;
            }
            return flag;
        }

        private List<AppFunc> GetAppList(BizDataContext context, IUserIdentity user)
        {
            int appUserState = 1;
            return (from s in context.Set<SysFunction>()
                join a in context.Set<T_Application_User>() on s.AppId equals (long?) a.Application_ID 
                where ((a.State == appUserState) && (a.OwnerId == user.User_ID)) && ((s.Permission_Type == null) || (s.Permission_Type == 0L))
                select new AppFunc { AppId = a.Application_ID, FuncId = s.Function_ID.ToInt(), FuncName = s.Permission_Name }).ToList<AppFunc>();
        }

        private List<ConfigRemindItem> GetRemindItem(BizDataContext context, IUserIdentity user, AppFunc appFunc)
        {
            List<ConfigRemindItem> list = new List<ConfigRemindItem>();
            IEnumerable<IConfigReminder> allInstances = IocHelper.GetAllInstances<IConfigReminder>();
            foreach (IConfigReminder reminder in allInstances)
            {
                list.AddRange(reminder.GetRemindItems(user, appFunc.AppId, appFunc.FuncId));
            }
            return list;
        }

        public List<WrokItemBase> GetWorkItem(IUserIdentity user)
        {
            using (BizDataContext context = new BizDataContext(true))
            {
                List<WrokItemBase> list = new List<WrokItemBase>();
                WrokItemBase item = null;
                if (!(from r in context.Where<T_User_Role>(r => r.User_Id == user.User_ID) select r.Role_Id).ToList<int>().Contains(Uitity.SystemManger))
                {
                    return null;
                }
                List<AppFunc> appList = this.GetAppList(context, user);
                if ((appList != null) && (appList.Count > 0))
                {
                    int workItemState = 0;
                    List<T_WorkItemBase> workItemList = context.Where<T_WorkItemBase>(w => w.State == workItemState);
                    foreach (AppFunc func in appList)
                    {
                        item = new WrokItemBase {
                            FuncName = func.FuncName,
                            workTaskList = this.GetWorkTask(context, user, func, workItemList),
                            remindItemList = this.GetRemindItem(context, user, func)
                        };
                        list.Add(item);
                    }
                }
                return list;
            }
        }

        public T_WorkItemBase GetWorkItemBase(int id)
        {
            using (BizDataContext context = new BizDataContext(true))
            {
                return context.FindById<T_WorkItemBase>(new object[] { id });
            }
        }

        private List<T_WorkItemBase> GetWorkTask(BizDataContext context, IUserIdentity user, AppFunc appFunc, List<T_WorkItemBase> workItemList)
        {
            return (from p in workItemList.Where<T_WorkItemBase>(delegate (T_WorkItemBase w) {
                int? classify = null;
                if (w.IsMust == true)
                {
                    classify = w.Classify;
                }
                return (((classify.GetValueOrDefault() == 0x3e9) && classify.HasValue) && w.ObjectId.HasValue) && (w.ObjectId.Value == appFunc.FuncId);
            })
                orderby p.WorkSort
                select p).ToList<T_WorkItemBase>();
        }
    }
}

