namespace King.Framework.WorkflowEngineCore.ParticipantFunctions
{
    using King.Framework.EntityLibrary;
    using King.Framework.Interfaces;
    using King.Framework.Linq;
    using System;
    using System.Collections.Generic;

    internal class SpecialLevelParentDeptFunc : ParticiparntFuncBase
    {
        public SpecialLevelParentDeptFunc(DataContext ctx) : base(ctx)
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
                department = new ParentDepartmentFunc(base._context).GetDepartment(participant, pi, ai);
            }
            else
            {
                if (participant.FunctionType != 10)
                {
                    throw new ApplicationException("在SpecialLevelParentDeptFunc的GetDepartment中FunctionType有误！");
                }
                department = this.GetDepartment(participant, pi, ai);
            }
            string str = part.Param_Constant;
            if (!base._orgProxy.IsSupportDepartmentLevel())
            {
                throw new ApplicationException("OrgProxy未实现部门层级，请重载IsSupportDepartmentLevel方法");
            }
            IDepartment parentDepartment = department;
            if (parentDepartment == null)
            {
                throw new ApplicationException("在SpecialLevelParentDeptFunc的GetUsers中获取启动部门失败！");
            }
            for (string str2 = base._orgProxy.GetDepartmentLevelCode(parentDepartment.Department_ID).ToString(); (str != str2) && (parentDepartment != null); str2 = base._orgProxy.GetDepartmentLevelCode(parentDepartment.Department_ID).ToString())
            {
                parentDepartment = base._orgProxy.GetParentDepartment(parentDepartment.Department_ID);
            }
            if (parentDepartment == null)
            {
                throw new ApplicationException("在SpecialLevelParentDeptFunc中未能找到指定层级的上级部门！");
            }
            return parentDepartment;
        }

        public override IList<IUser> GetUsers(SysProcessParticipant part, SysProcessInstance pi, SysActivityInstance ai)
        {
            IDepartment department = this.GetDepartment(part, pi, ai);
            if (department == null)
            {
                throw new ApplicationException("在SpecialLevelParentDeptFunc中未能找到指定层级的上级部门！");
            }
            return base._orgProxy.GetDepartmentUsers(department.Department_ID);
        }
    }
}

