namespace King.Framework.WorkflowEngineCore.ParticipantFunctions
{
    using King.Framework.EntityLibrary;
    using King.Framework.Interfaces;
    using King.Framework.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class PrevAllParticipantFunc : ParticiparntFuncBase
    {
        public PrevAllParticipantFunc(DataContext ctx) : base(ctx)
        {
        }

        private void GetUserIdList(SysActivityInstance ai, List<int> result)
        {
            SysTransitionInstance instance = ai.ToTransitionInstances.FirstOrDefault<SysTransitionInstance>();
            if (instance != null)
            {
                SysActivityInstance preActivityInstance = instance.PreActivityInstance;
                List<int> collection = (from i in preActivityInstance.WorkItems select i.OwnerId.Value).ToList<int>();
                result.AddRange(collection);
                this.GetUserIdList(preActivityInstance, result);
            }
        }

        public override IList<IUser> GetUsers(SysProcessParticipant part, SysProcessInstance pi, SysActivityInstance ai)
        {
            List<int> result = new List<int>();
            this.GetUserIdList(ai, result);
            List<IUser> source = new List<IUser>();
            foreach (int num in result.Distinct<int>())
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

