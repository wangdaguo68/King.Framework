using System.Data.Common;
using King.Framework.EntityLibrary;

namespace King.Framework.Manager
{
    using King.Framework.Common;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class RoleFunctionManager
    {
        private BizDataContext context;
        private static readonly string DeleteFunctionsByRoleId = "delete from T_Role_Function where Role_ID = {0}";
        private static readonly string InsertFunctionsByRoleId = "insert into T_Role_Function (Role_Function_Id,Role_ID,Function_ID) values ({0},{1},{2})";
        private static readonly string QueryFunctionsByRoleId = "select * from SysFunction a left outer join T_Role_Function b on a.Function_ID = b.Function_ID where b.Role_ID = {0} and Is_Menu = 1";

        public RoleFunctionManager(BizDataContext ctx)
        {
            this.context = ctx;
        }

        public void AddFunctionsToRole(int roleID, List<int> functionList)
        {
            string sql = string.Format(DeleteFunctionsByRoleId, roleID);
            this.context.ExecuteNonQuery(sql, new DbParameter[0]);
            foreach (int num in functionList)
            {
                long nextIdentity = this.context.GetNextIdentity(false);
                string str2 = string.Format(InsertFunctionsByRoleId, nextIdentity, roleID, num);
                this.context.ExecuteNonQuery(str2, new DbParameter[0]);
            }
        }

        public List<SysFunction> GetFunctions(List<int> roleList)
        {
            IEnumerable<SysFunction> first = new List<SysFunction>();
            foreach (int num in roleList)
            {
                string sql = string.Format(QueryFunctionsByRoleId, num);
                List<SysFunction> second = this.context.ExecuteObjectSet<SysFunction>(sql, new DbParameter[0]);
                first = first.Union<SysFunction>(second);
            }
            return first.Distinct<SysFunction>().ToList<SysFunction>();
        }

        public List<SysFunction> GetFunctions(int roleID)
        {
            string sql = string.Format(QueryFunctionsByRoleId, roleID);
            return this.context.ExecuteObjectSet<SysFunction>(sql, new DbParameter[0]);
        }
    }
}
