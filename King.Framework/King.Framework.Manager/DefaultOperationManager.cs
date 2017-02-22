namespace King.Framework.Manager
{
    using King.Framework.Common;
    using King.Framework.EntityLibrary;
    using King.Framework.Interfaces;
    using King.Framework.OrgLibrary;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class DefaultOperationManager : IOperationManager
    {
        private BizDataContext context;
        private IUserIdentity currentUser;

        public DefaultOperationManager(BizDataContext context, IUserIdentity user)
        {
            this.context = context;
            this.currentUser = user;
        }

        public bool CheckHasSharedPrivilege(int objectId, string entityName, EntityOperationEnum operationEnum)
        {
            bool flag = false;
            if ((operationEnum != EntityOperationEnum.Add) && (operationEnum != EntityOperationEnum.None))
            {
                if (objectId == 0)
                {
                    return flag;
                }
                List<SysSharedPrivilege> list = this.context.Where<SysSharedPrivilege>(p => ((p.ObjectId == objectId) && (p.EntityName == entityName)) && (p.Privilege == ((int) operationEnum))).ToList<SysSharedPrivilege>();
                List<IRole> userRoles = OrgProxyFactory.GetProxy(this.context).GetUserRoles(this.CurrentUser.User_ID);
                using (List<SysSharedPrivilege>.Enumerator enumerator = list.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Func<IRole, bool> predicate = null;
                        SysSharedPrivilege share = enumerator.Current;
                        int? shareType = share.ShareType;
                        if ((shareType.GetValueOrDefault() == 0) && shareType.HasValue)
                        {
                            shareType = share.ShareUserId;
                            int num = this.CurrentUser.User_ID;
                            if ((shareType.GetValueOrDefault() == num) && shareType.HasValue)
                            {
                                return true;
                            }
                        }
                        else if (share.ShareType == 1)
                        {
                            if (predicate == null)
                            {
                                predicate = p => p.Role_ID == share.ShareRoleId;
                            }
                            if (userRoles.FirstOrDefault<IRole>(predicate) != null)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return flag;
        }

        public List<int> GetSubDeptIds(int? deptid)
        {
            if (deptid.HasValue)
            {
                return (from p in OrgProxyFactory.GetProxy(this.context).GetAllChildDepartment(deptid.Value) select p.Department_ID).ToList<int>();
            }
            return new List<int>();
        }

        public int? GetUserDeptId(int? userid)
        {
            int num = -2147483647;
            if (userid.HasValue)
            {
                IUser userById = OrgProxyFactory.GetProxy(null).GetUserById(userid.Value);
                if (userById.Department_ID.HasValue)
                {
                    num = userById.Department_ID.Value;
                }
            }
            return new int?(num);
        }

        public List<int> GetUserIdByDeptId(params int[] ids)
        {
            return (from x in this.context.FetchAll<T_User>()
                where ids.Contains<int>(x.Department_ID.Value)
                select x.User_ID).ToList<int>();
        }

        public EntityPrivilegeEnum TryCanOperation(int userId, long entityId, EntityOperationEnum operationEnum)
        {
            if (this.context.FindById<T_User>(new object[] { userId }) == null)
            {
                throw new Exception("当前用户不存在数据库中");
            }
            IOrgProxy proxy = OrgProxyFactory.GetProxy(this.context);
            IEnumerable<int> currentUserRoleIds = from p in proxy.GetUserRoles(userId) select p.Role_ID;
            SysDataPrivilege privilege = (from x in this.context.Where<SysDataPrivilege>(x => (x.EntityId == entityId) && (x.OperationId == ((int) operationEnum)))
                where currentUserRoleIds.Contains<int>(!x.RoleId.HasValue ? 0 : x.RoleId.ToInt())
                orderby x.PrivilegeLevel descending
                select x).FirstOrDefault<SysDataPrivilege>();
            if (null != privilege)
            {
                return (EntityPrivilegeEnum)privilege.PrivilegeLevel.Value;
            }
            return EntityPrivilegeEnum.NoPermission;
        }

        public IUserIdentity CurrentUser
        {
            get
            {
                return this.currentUser;
            }
        }
    }
}
