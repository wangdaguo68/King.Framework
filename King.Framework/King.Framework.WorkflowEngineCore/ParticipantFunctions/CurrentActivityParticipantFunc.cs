using King.Framework.Interfaces;

namespace King.Framework.WorkflowEngineCore.ParticipantFunctions
{
    using King.Framework.EntityLibrary;
    using King.Framework.Linq;
    using King.Framework.WorkflowEngineCore.InternalCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class CurrentActivityParticipantFunc : ParticiparntFuncBase
    {
        public CurrentActivityParticipantFunc(DataContext ctx) : base(ctx)
        {
        }

        public override IList<IUser> GetUsers(SysProcessParticipant part, SysProcessInstance pi, SysActivityInstance ai)
        {
            List<IUser> source = new List<IUser>();
            using (IEnumerator<SysActivityParticipant> enumerator = ai.Activity.ActivityParticipants.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Func<SysProcessParticipant, bool> predicate = null;
                    SysActivityParticipant p = enumerator.Current;
                    if (predicate == null)
                    {
                        predicate = i => i.ParticipantId == p.ParticipantId;
                    }
                    SysProcessParticipant participant = pi.Process.ProcessParticipants.FirstOrDefault<SysProcessParticipant>(predicate);
                    if (participant != null)
                    {
                        int? wiOwnerId = null;
                        List<IUser> collection = ParticipantHelper.GetUsers(base._context, participant, pi, ai, wiOwnerId);
                        source.AddRange(collection);
                    }
                }
            }
            return source.Distinct<IUser>().ToList<IUser>();
        }
    }
}

