namespace King.Framework.Common.ServiceContract
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class WfActivityEntityInstance
    {
        public Dictionary<string, WfActivityFieldData> ActivityEntityInstanceData { get; set; }

        public int ActivityEntityInstanceId { get; set; }
    }
}
