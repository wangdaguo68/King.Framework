namespace King.Framework.Manager
{
    using King.Framework.Common;
    using King.Framework.EntityLibrary;
    using System;

    public interface ISharePrivilegeHandler
    {
        int Share(string entityName, int objectId, int ownerId, ShareType sharedType, int? sharedUserId, int? sharedRoleId, EntityOperationEnum privilege);
        void UnShare(int id);
    }
}
