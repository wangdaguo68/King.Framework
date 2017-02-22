namespace King.Framework.WorkflowEngineCore.ParticipantFunctions
{
    using King.Framework.EntityLibrary;
    using King.Framework.Interfaces;
    using King.Framework.Linq;
    using System;
    using System.Collections.Generic;

    internal class DepartmentRoleFunc : ParticiparntFuncBase
    {
        public DepartmentRoleFunc(DataContext ctx) : base(ctx)
        {
        }

        public override IList<IUser> GetUsers(SysProcessParticipant part, SysProcessInstance pi, SysActivityInstance ai)
        {
            if (part == null)
            {
                throw new ArgumentNullException("part");
            }
            if (part.Param_Participant == null)
            {
                throw new ApplicationException("部门角色参与人未指定基于参与人");
            }
            if (!part.Param_RoleId.HasValue)
            {
                throw new ApplicationException("未指定参与人的角色");
            }
            SysProcessParticipant participant = part.Param_Participant;
            IDepartment department = null;
            if (participant.FunctionType == 3)
            {
                department = new SpecDepartmentFunc(base._context).GetDepartment(participant, pi, ai);
            }
            else if (participant.FunctionType == 2)
            {
                department = new StartDepartmentFunc(base._context).GetDepartment(pi);
            }
            else if (participant.FunctionType == 5)
            {
                department = new ParentDepartmentFunc(base._context).GetDepartment(participant, pi, ai);
            }
            else
            {
                if (participant.FunctionType != 10)
                {
                    throw new ApplicationException("在DepartmentRoleFunc的GetUsers中FunctionType有误！");
                }
                department = new SpecialLevelParentDeptFunc(base._context).GetDepartment(participant, pi, ai);
            }
            return base._orgProxy.GetDepartmentRoleUsers(department.Department_ID, part.Param_RoleId.Value);
        }
    }
}

