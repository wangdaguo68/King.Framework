namespace King.Framework.WorkflowEngineCore.ParticipantFunctions
{
    using King.Framework.EntityLibrary;
    using King.Framework.Interfaces;
    using King.Framework.Linq;
    using System;
    using System.Collections.Generic;

    internal class ParentDepartmentFunc : ParticiparntFuncBase
    {
        public ParentDepartmentFunc(DataContext ctx) : base(ctx)
        {
        }

        public IDepartment GetDepartment(SysProcessParticipant part, SysProcessInstance pi, SysActivityInstance ai)
        {
            IDepartment department = null;
            if (part.Param_Participant == null)
            {
                throw new ApplicationException("父部门参与人未指定基于参于人");
            }
            SysProcessParticipant participant = part.Param_Participant;
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
                department = this.GetDepartment(participant, pi, ai);
            }
            else
            {
                if (participant.FunctionType != 10)
                {
                    throw new ApplicationException("在ParentDepartmentFunc的GetDepartment中FunctionType有误！");
                }
                department = new SpecialLevelParentDeptFunc(base._context).GetDepartment(participant, pi, ai);
            }
            return base._orgProxy.GetDepartmentById(department.Parent_ID.Value);
        }

        public override IList<IUser> GetUsers(SysProcessParticipant part, SysProcessInstance pi, SysActivityInstance ai)
        {
            IDepartment department = this.GetDepartment(part, pi, ai);
            if (department == null)
            {
                throw new ApplicationException("在ParentDepartmentFunc的GetUsers中获取部门有误！");
            }
            return base._orgProxy.GetDepartmentUsers(department.Department_ID);
        }
    }
}

