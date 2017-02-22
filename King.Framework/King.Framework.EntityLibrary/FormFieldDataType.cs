namespace King.Framework.EntityLibrary
{
    using System;
    using System.ComponentModel;

    public enum FormFieldDataType
    {
        [Description("时间")]
        DateTime = 4,
        [Description("附件")]
        File = 10,
        [Description("整数")]
        Int = 2,
        [Description("小数")]
        Number = 5,
        [Description("文本")]
        String = 3,
        [Description("长文本")]
        Text = 7
    }
}

