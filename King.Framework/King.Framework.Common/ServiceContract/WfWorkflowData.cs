namespace King.Framework.Common.ServiceContract
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class WfWorkflowData
    {
        public IEnumerable<WfActivity> ActivityList { get; set; }

        public IEnumerable<WfActivityOperation> ActivityOperationList { get; set; }

        public IEnumerable<WfActivityParticipant> ActivityParticipantList { get; set; }

        public IEnumerable<WfActivityRemind> ActivityRemindList { get; set; }

        public IEnumerable<WfActivityRemindParticipant> ActivityRemindParticipantList { get; set; }

        public IEnumerable<WfActivityStep> ActivityStepList { get; set; }

        public IEnumerable<WfExpression> ExpressionList { get; set; }

        public IEnumerable<WfProcessParticipant> ParticipantList { get; set; }

        public WfProcess Process { get; set; }

        public IEnumerable<WfProcessRemind> ProcessRemindList { get; set; }

        public IEnumerable<WfProcessRemindParticipant> ProcessRemindParticipantList { get; set; }

        public IEnumerable<WfTransition> TransitionList { get; set; }
    }
}
