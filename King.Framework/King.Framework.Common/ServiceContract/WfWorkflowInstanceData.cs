namespace King.Framework.Common.ServiceContract
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class WfWorkflowInstanceData
    {
        public IEnumerable<WfActivityInstance> ActivityInstanceList { get; set; }

        public IEnumerable<WfActivity> ActivityList { get; set; }

        public WfProcess Process { get; set; }

        public WfProcessInstance ProcessInstance { get; set; }

        public IEnumerable<WfTransitionInstance> TransitionInstanceList { get; set; }

        public IEnumerable<WfTransition> TransitionList { get; set; }
    }
}
