namespace King.Framework.EntityLibrary
{
    using System;
    using System.ComponentModel;

    public enum ViewControlRelationType
    {
        [Description(" 多对多关系 ")]
        MM = 2,
        [Description(" 多引用关系 ")]
        MU = 3,
        [Description(" 非关系视图 ")]
        NO = 0,
        [Description(" 一对多关系 ")]
        OM = 1
    }
}

