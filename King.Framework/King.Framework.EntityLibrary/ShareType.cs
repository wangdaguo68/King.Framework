namespace King.Framework.EntityLibrary
{
    using System;
    using System.ComponentModel;

    public enum ShareType
    {
        [Description("按角色")]
        ShareRole = 1,
        [Description("按人")]
        ShareUser = 0
    }
}

