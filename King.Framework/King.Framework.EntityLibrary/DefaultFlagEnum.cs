namespace King.Framework.EntityLibrary
{
    using System;
    using System.ComponentModel;

    [Serializable]
    public enum DefaultFlagEnum
    {
        [Description("否")]
        No = 0,
        [Description("是")]
        Yes = 1
    }
}

