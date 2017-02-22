namespace King.Framework.Interfaces
{
    using System;
    using System.Collections.Generic;

    public interface IOrgProxy
    {
        List<IDepartment> GetAllChildDepartment(int departmentId);
        List<IDepartment> GetChildDepartments(int departmentId);
        IDepartment GetDepartmentById(int departmentId);
        int GetDepartmentLevelCode(int departmentId);
        IUser GetDepartmentManager(int departmentId);
        List<IUser> GetDepartmentRoleUsers(int departmentId, int roleId);
        List<IDepartment> GetDepartments();
        List<IUser> GetDepartmentUsers(int departmentId);
        IDepartment GetParentDepartment(int departmentId);
        List<IRole> GetRoles();
        List<IUser> GetRoleUsers(int roleId);
        IUser GetUserById(int userId);
        List<IRole> GetUserRoles(int userId);
        List<IUser> GetUsers();
        bool IsRootDepartment(int departmentId);
        bool IsSupportDepartmentLevel();
    }
}

