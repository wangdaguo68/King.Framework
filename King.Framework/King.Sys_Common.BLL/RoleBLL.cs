namespace King.Sys_Common.BLL
{
    using King.Framework.Common;
    using King.Framework.EntityLibrary;
    using King.Framework.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;

    public class RoleBLL : BaseBLL
    {
        public RoleBLL(IUserIdentity user) : base(user)
        {
        }

        public int AddRole(T_Role role)
        {
            using (BizDataContext context = new BizDataContext(true))
            {
                role.Role_ID = context.GetNextIdentity_Int(false);
                context.Insert(role);
            }
            return role.Role_ID;
        }

        public IQueryable<T_Role> Query(BizDataContext db)
        {
            return (from p in db.Set<T_Role>()
                where p.State != 0x270f
                select p);
        }

        public List<T_Role> QueryRoles(string roleName = null, int pageIndex = 1, int pageSize = 10)
        {
            using (BizDataContext context = new BizDataContext(true))
            {
                IQueryable<T_Role> source = context.Set<T_Role>();
                if (!string.IsNullOrWhiteSpace(roleName))
                {
                    source = from t in source
                        where t.Role_Name.Contains(roleName)
                        select t;
                }
                source = from t in source
                    orderby t.Role_Name
                    select t;
                if (pageIndex <= 1)
                {
                    pageIndex = 1;
                }
                return source.Skip<T_Role>(((pageIndex - 1) * pageSize)).Take<T_Role>(pageSize).ToList<T_Role>();
            }
        }
    }
}

