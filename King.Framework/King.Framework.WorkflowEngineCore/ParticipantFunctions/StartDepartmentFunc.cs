namespace King.Framework.WorkflowEngineCore.ParticipantFunctions
{
    using King.Framework.EntityLibrary;
    using King.Framework.Interfaces;
    using King.Framework.Linq;
    using System;
    using System.Collections.Generic;

    internal class StartDepartmentFunc : ParticiparntFuncBase
    {
        public StartDepartmentFunc(DataContext ctx) : base(ctx)
        {
        }

        public IDepartment GetDepartment(SysProcessInstance pi)
        {
            if (pi == null)
            {
                throw new ArgumentNullException("pi");
            }
            if (!pi.StartUserId.HasValue)
            {
                throw new ApplicationException("启动用户为空");
            }
            int userId = pi.StartUserId.Value;
            IUser userById = base._orgProxy.GetUserById(userId);
            if (userById == null)
            {
                throw new ApplicationException("启动用户不存在");
            }
            if (!userById.Department_ID.HasValue)
            {
                throw new ApplicationException(string.Format("未设置用户{0}({1})的部门", userById.User_Name, userById.User_ID));
            }
            IDepartment departmentById = base._orgProxy.GetDepartmentById(userById.Department_ID.Value);
            if (departmentById == null)
            {
                throw new ApplicationException(string.Format("部门{0}不存在", userById.Department_ID.Value));
            }
            return departmentById;
        }

        public override IList<IUser> GetUsers(SysProcessParticipant part, SysProcessInstance pi, SysActivityInstance ai)
        {
            IDepartment department = this.GetDepartment(pi);
            if (department == null)
            {
                throw new ApplicationException("在StartDepartmentFunc的GetUsers中获取部门有误！");
            }
            return base._orgProxy.GetDepartmentUsers(department.Department_ID);
        }
    }
}

