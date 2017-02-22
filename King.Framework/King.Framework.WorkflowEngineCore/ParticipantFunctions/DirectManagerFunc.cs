namespace King.Framework.WorkflowEngineCore.ParticipantFunctions
{
    using King.Framework.EntityLibrary;
    using King.Framework.Interfaces;
    using King.Framework.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class DirectManagerFunc : ParticiparntFuncBase
    {
        public DirectManagerFunc(DataContext ctx) : base(ctx)
        {
        }

        public override IList<IUser> GetUsers(SysProcessParticipant part, SysProcessInstance pi, SysActivityInstance ai)
        {
            List<IUser> list = new List<IUser>();
            IUser startUser = null;
            if (part == null)
            {
                throw new ArgumentNullException("part");
            }
            SysProcessParticipant participant = part.Param_Participant;
            if (participant == null)
            {
                throw new ApplicationException("主属主管参与人未指定基于参于人");
            }
            if (participant.FunctionType == 0)
            {
                startUser = new StartUserFunc(base._context).GetStartUser(pi);
            }
            else if (participant.FunctionType == 1)
            {
                SpecUserFunc func2 = new SpecUserFunc(base._context);
                startUser = func2.GetUsers(participant, pi, ai).FirstOrDefault<IUser>();
            }
            else
            {
                if (participant.FunctionType != 9)
                {
                    throw new ApplicationException("直属主管参与人获得人员失败，可能是参数类型错误");
                }
                startUser = this.GetUsers(participant, pi, ai).FirstOrDefault<IUser>();
            }
            if (startUser == null)
            {
                throw new ApplicationException("直属主管参与人获得人员失败，可能是参数类型错误");
            }
            if (!startUser.Department_ID.HasValue)
            {
                throw new ApplicationException("人员没有部门，无法计算直属主管");
            }
            IUser departmentManager = base._orgProxy.GetDepartmentManager(startUser.Department_ID.Value);
            if (departmentManager == null)
            {
                throw new ApplicationException(string.Format("部门ID:{0}没有设置经理", startUser.Department_ID));
            }
            if (startUser.User_ID != departmentManager.User_ID)
            {
                list.Add(departmentManager);
                return list;
            }
            IDepartment parentDepartment = base._orgProxy.GetParentDepartment(startUser.Department_ID.Value);
            if (parentDepartment != null)
            {
                if (parentDepartment.Department_ID != startUser.Department_ID)
                {
                    list.Add(base._orgProxy.GetDepartmentManager(parentDepartment.Department_ID));
                    return list;
                }
            }
            list.Add(departmentManager);
            return list;
        }
    }
}

