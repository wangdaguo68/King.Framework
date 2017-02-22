namespace King.Framework.EntityLibrary
{
    using System;
    using System.ComponentModel;

    public enum CascadeDeleteFlagEnum
    {
        [Description(" 级联删除 ")]
        CascadeDelete = 2,
        [Description(" 限制删除 ")]
        ConstraintDelete = 1,
        [Description(" 可选引用 ")]
        SetNull = 0
    }
}

