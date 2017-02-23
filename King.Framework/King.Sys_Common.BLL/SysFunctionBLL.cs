namespace King.Sys_Common.BLL
{
    using King.Framework.Common;
    using King.Framework.EntityLibrary;
    using System;
    using System.Collections.Generic;

    public class SysFunctionBLL
    {
        public bool DeleteRoleFunction(int roleId)
        {
            try
            {
                using (BizDataContext context = new BizDataContext(true))
                {
                    List<T_Role_Function> list = context.Where<T_Role_Function>(r => r.Role_ID == roleId);
                    if ((list != null) && (list.Count > 0))
                    {
                        foreach (T_Role_Function function in list)
                        {
                            context.Delete(function);
                        }
                    }
                }
                return true;
            }
            catch (Exception exception)
            {
                AppLogHelper.Error(exception);
                return false;
            }
        }

        public void InsertRoleFunction(int roleId, List<int> functionIdList)
        {
            using (BizDataContext context = new BizDataContext(true))
            {
                if (this.DeleteRoleFunction(roleId))
                {
                    T_Role_Function item = null;
                    List<T_Role_Function> objList = new List<T_Role_Function>();
                    foreach (int num in functionIdList)
                    {
                        item = new T_Role_Function {
                            Role_Function_ID = new long?((long) context.GetNextIdentity_Int(false)),
                            Role_ID = new long?((long) roleId),
                            Function_ID = new long?((long) num)
                        };
                        objList.Add(item);
                    }
                    context.BatchInsert<T_Role_Function>(objList);
                }
            }
        }
    }
}

