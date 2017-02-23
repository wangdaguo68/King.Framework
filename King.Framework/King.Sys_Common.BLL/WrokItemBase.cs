using King.Framework.EntityLibrary;
using King.Framework.Interfaces;

namespace King.Sys_Common.BLL
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class WrokItemBase
    {
        public string FuncName { get; set; }

        public List<ConfigRemindItem> remindItemList { get; set; }

        public List<T_WorkItemBase> workTaskList { get; set; }
    }
}

