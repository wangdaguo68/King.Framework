using System.Data.Common;
using King.Framework.EntityLibrary;

namespace King.Framework.Manager
{
    using King.Framework.Common;
    using King.Framework.Repository.Schemas;
    using Microsoft.CSharp.RuntimeBinder;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;

    public class OrgManager
    {
        private BizDataContext _context;

        public OrgManager(BizDataContext ctx)
        {
            this._context = ctx;
        }

        public List<T_User> GetDepartmentAllUsers(int departmentId)
        {
            string sql = string.Format("Select * \r\n                                                from T_User a,T_Department b\r\n                                                where a.Department_ID = b.Department_ID\r\n                                                  And b.SystemLevelCode like (Select SystemLevelCode from T_Department where Department_ID = {0})+'%'", departmentId);
            return this._context.ExecuteObjectSet<T_User>(sql, new DbParameter[0]);
        }

        public string GetDisplayValue(long entityId, int objectId)
        {
            try
            {
                IEntitySchema schema = IEntitySchemaHelper.Get(entityId);
                object entity = this.GetEntity(schema.EntityName, objectId);
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public string GetDisplayValue(string entityName, int id)
        {
            try
            {
                object entity = this.GetEntity(entityName, id);
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }


        public object GetEntity(string entityName, int id)
        {
            if (!(string.IsNullOrEmpty(entityName) || (id == 0)))
            {
                IEntitySchema schema = IEntitySchemaHelper.Get(entityName);
                return this._context.FindById(schema.EntityType, new object[] { id });
            }
            return null;
        }
    }
}
