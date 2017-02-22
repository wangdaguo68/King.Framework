namespace King.Framework.WorkflowEngineCore.ParticipantFunctions
{
    using King.Framework.EntityLibrary;
    using King.Framework.Interfaces;
    using King.Framework.Linq;
    using System;
    using System.Collections.Generic;

    internal class DepartmentManagerFunc : ParticiparntFuncBase
    {
        public DepartmentManagerFunc(DataContext ctx) : base(ctx)
        {
        }

        public IDepartment GetDepartment(SysProcessParticipant part, SysProcessInstance pi, SysActivityInstance ai)
        {
            if (part == null)
            {
                throw new ArgumentNullException("part");
            }
            SysProcessParticipant participant = part.Param_Participant;
            if (participant == null)
            {
                throw new ApplicationException("部门经理参与人未指定基于参于人");
            }
            if (participant.FunctionType == 3)
            {
                SpecDepartmentFunc func = new SpecDepartmentFunc(base._context);
                return func.GetDepartment(participant, pi, ai);
            }
            if (participant.FunctionType == 2)
            {
                StartDepartmentFunc func2 = new StartDepartmentFunc(base._context);
                return func2.GetDepartment(pi);
            }
            if (participant.FunctionType == 5)
            {
                ParentDepartmentFunc func3 = new ParentDepartmentFunc(base._context);
                return func3.GetDepartment(participant, pi, ai);
            }
            if (participant.FunctionType != 10)
            {
                throw new ApplicationException("部门经理参与人获得部门失败，可能是参数类型错误");
            }
            SpecialLevelParentDeptFunc func4 = new SpecialLevelParentDeptFunc(base._context);
            return func4.GetDepartment(participant, pi, ai);
        }

        public override IList<IUser> GetUsers(SysProcessParticipant part, SysProcessInstance pi, SysActivityInstance ai)
        {
            IDepartment department = this.GetDepartment(part, pi, ai);
            if (department == null)
            {
                throw new ApplicationException("在参与人中未找到指定部门");
            }
            IUser departmentManager = base._orgProxy.GetDepartmentManager(department.Department_ID);
            if (departmentManager == null)
            {
                throw new ApplicationException("在指定部门中未找到指定部门经理");
            }
            return new List<IUser> { departmentManager };
        }
    }
}

