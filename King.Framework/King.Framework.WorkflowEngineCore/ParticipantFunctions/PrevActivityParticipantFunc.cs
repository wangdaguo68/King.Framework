namespace King.Framework.WorkflowEngineCore.ParticipantFunctions
{
    using King.Framework.EntityLibrary;
    using King.Framework.Interfaces;
    using King.Framework.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class PrevActivityParticipantFunc : ParticiparntFuncBase
    {
        public PrevActivityParticipantFunc(DataContext ctx) : base(ctx)
        {
        }

        public override IList<IUser> GetUsers(SysProcessParticipant part, SysProcessInstance pi, SysActivityInstance ai)
        {
            if (ai.ToTransitionInstances.Count != 1)
            {
                throw new ApplicationException(string.Format("连接线实例不正确,AI:{0}", ai.ActivityInstanceId));
            }
            SysTransitionInstance instance = ai.ToTransitionInstances.First<SysTransitionInstance>();
            SysActivityInstance preActivityInstance = instance.PreActivityInstance;
            if (preActivityInstance == null)
            {
                throw new ApplicationException(string.Format("获得上一个活动实例失败,ID:{0}", instance.PostActivityInstanceId));
            }
            if (preActivityInstance.Activity == null)
            {
                throw new ApplicationException(string.Format("获得上一个活动定义失败,ID:{0}", preActivityInstance.ActivityId));
            }
            List<int> list = (from p in preActivityInstance.WorkItems select p.OwnerId.Value).Distinct<int>().ToList<int>();
            List<IUser> source = new List<IUser>();
            foreach (int num in list)
            {
                IUser userById = base._orgProxy.GetUserById(num);
                if (userById != null)
                {
                    source.Add(userById);
                }
            }
            return source.ToList<IUser>();
        }
    }
}

