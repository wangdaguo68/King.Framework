namespace King.Framework.Manager
{
    using King.Framework.Common;
    using King.Framework.EntityLibrary;
    using System;

    public class SharePrivilegeManager : ISharePrivilegeHandler
    {
        private BizDataContext context;

        public SharePrivilegeManager(BizDataContext ctx)
        {
            this.context = ctx;
        }

        public int Share(string entityName, int objectId, int ownerId, ShareType sharedType, int? sharedUserId, int? sharedRoleId, EntityOperationEnum privilege)
        {
            SysSharedPrivilege privilege2 = new SysSharedPrivilege {
                CreateTime = new DateTime?(DateTime.Now),
                OwnerId = new int?(ownerId),
                EntityName = entityName,
                ObjectId = new int?(objectId),
                Privilege = new int?((int) privilege),
                ShareRoleId = sharedRoleId,
                ShareUserId = sharedUserId,
                ShareType = new int?((int) sharedType),
                Id = this.context.GetNextIdentity_Int(false)
            };
            this.context.Insert(privilege2);
            return privilege2.Id;
        }

        public void UnShare(int id)
        {
            SysSharedPrivilege privilege = this.context.FindById<SysSharedPrivilege>(new object[] { id });
            if (privilege != null)
            {
                this.context.Delete(privilege);
            }
        }
    }
}
