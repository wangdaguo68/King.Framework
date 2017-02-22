namespace King.Framework.WorkflowEngineCore
{
    using King.Framework.Common;
    using King.Framework.EntityLibrary;
    using System;
    using System.Data.Common;

    public class WorkItemManager : IWorkItemHandler
    {
        public WorkItemManager(BizDataContext ctx)
        {
        }

        public virtual void OnWorkItemCompleted(int wiBaseId, int wiId, string Id)
        {
            if (WebConfigAppSettings.UseSSOModel)
            {
                using (BizDataContext context = new BizDataContext(WebConfigAppSettings.HomeBizDBConnString, true))
                {
                    int num;
                    if (int.TryParse(Id, out num))
                    {
                        T_WorkItemBase base2 = context.FindById<T_WorkItemBase>(new object[] { num });
                        if (base2 != null)
                        {
                            base2.State = 1;
                            context.Update(base2);
                        }
                    }
                }
            }
        }

        public virtual string OnWorkItemCreating(T_WorkItemBase wiBase, SysWorkItem wi)
        {
            if (WebConfigAppSettings.UseSSOModel)
            {
                using (BizDataContext context = new BizDataContext(WebConfigAppSettings.HomeBizDBConnString, true))
                {
                    wiBase.WorkItemBase_Id = context.GetNextIdentity_Int(false);
                    context.Insert(wiBase);
                    DbParameter parameter = context.CreateParameter();
                    parameter.ParameterName = context.AddPrefixToParameterName("OwnerBiz");
                    parameter.Value = WebConfigAppSettings.ApplicationName;
                    string sql = string.Format("update T_WorkItemBase set OwnerBiz = {0}", parameter.ParameterName);
                    context.ExecuteNonQuery(sql, new DbParameter[] { parameter });
                    return wiBase.WorkItemBase_Id.ToString();
                }
            }
            return null;
        }
    }
}

