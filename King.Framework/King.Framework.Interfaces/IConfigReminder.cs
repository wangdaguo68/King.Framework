namespace King.Framework.Interfaces
{
    using System;
    using System.Collections.Generic;

    public interface IConfigReminder
    {
        IList<ConfigRemindItem> GetRemindItems(IUserIdentity user, int appId, int functionId);
    }
}

