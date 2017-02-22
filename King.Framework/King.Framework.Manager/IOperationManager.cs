namespace King.Framework.Manager
{
    using King.Framework.Common;
    using King.Framework.Interfaces;
    using System;
    using System.Collections.Generic;

    public interface IOperationManager
    {
        bool CheckHasSharedPrivilege(int objectId, string entityName, EntityOperationEnum operationEnum);
        List<int> GetSubDeptIds(int? deptId);
        int? GetUserDeptId(int? userId);
        List<int> GetUserIdByDeptId(params int[] ids);
        EntityPrivilegeEnum TryCanOperation(int userId, long entityId, EntityOperationEnum operationEnum);

        IUserIdentity CurrentUser { get; }
    }
}
