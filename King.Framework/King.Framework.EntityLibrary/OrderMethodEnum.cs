namespace King.Framework.EntityLibrary
{
    using System;
    using System.ComponentModel;

    public enum OrderMethodEnum
    {
        [Description(" ASC ")]
        ASC = 1,
        [Description(" 拼音ASC ")]
        ASC_PY = 3,
        [Description(" DESC ")]
        DESC = 2,
        [Description(" 拼音DESC ")]
        DESC_PY = 4
    }
}

