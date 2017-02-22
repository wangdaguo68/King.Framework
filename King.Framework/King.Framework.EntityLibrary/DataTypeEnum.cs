namespace King.Framework.EntityLibrary
{
    using System;
    using System.ComponentModel;

    public enum DataTypeEnum
    {
        [Description("MultiRef")]
        MultiRef = 15,
        [Description("binary")]
        pbinary = 9,
        [Description("bool")]
        pbool = 11,
        [Description("datetime")]
        pdatetime = 4,
        [Description("decimal")]
        pdecimal = 5,
        [Description("enum")]
        penum = 13,
        [Description("file")]
        pfile = 10,
        [Description("float")]
        pfloat = 6,
        [Description("int")]
        pint = 2,
        [Description("key")]
        pkey = 1,
        [Description("long")]
        plong = 8,
        [Description("password")]
        ppassword = 14,
        [Description("ref")]
        pref = 12,
        [Description("string")]
        pstring = 3,
        [Description("text")]
        ptext = 7
    }
}

