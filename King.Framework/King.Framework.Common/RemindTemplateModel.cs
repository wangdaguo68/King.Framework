namespace King.Framework.Common
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class RemindTemplateModel
    {
        public Dictionary<string, object> AI_Data { get; set; }

        public string AprovePageInnerURL { get; set; }

        public string AprovePageOuterURL { get; set; }

        public string CurrentTime { get; set; }

        public Dictionary<string, object> PI_Data { get; set; }

        public string ProcessDetailInnerURL { get; set; }

        public string ProcessDetailOuterURL { get; set; }

        public string ReceiveUser { get; set; }
    }
}
