namespace King.Framework.WorkflowEngineCore.ParticipantFunctions
{
    using King.Framework.EntityLibrary;
    using King.Framework.Interfaces;
    using King.Framework.Linq;
    using System;
    using System.Collections.Generic;

    internal class SpecDepartmentFunc : ParticiparntFuncBase
    {
        public SpecDepartmentFunc(DataContext ctx) : base(ctx)
        {
        }

        public IDepartment GetDepartment(SysProcessParticipant part, SysProcessInstance pi, SysActivityInstance ai)
        {
            if (part == null)
            {
                throw new ArgumentNullException("part");
            }
            if (!part.Param_DepartmentId.HasValue)
            {
                throw new ApplicationException("未指定参与人的部门参数");
            }
            return base._orgProxy.GetDepartmentById(part.Param_DepartmentId.Value);
        }

        public override IList<IUser> GetUsers(SysProcessParticipant part, SysProcessInstance pi, SysActivityInstance ai)
        {
            IDepartment department = this.GetDepartment(part, pi, ai);
            if (department == null)
            {
                throw new ApplicationException("在SpecDepartmentFunc的GetUsers中获取部门有误！");
            }
            return base._orgProxy.GetDepartmentUsers(department.Department_ID);
        }
    }
}

