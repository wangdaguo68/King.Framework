namespace King.Framework.EntityLibrary
{
    using System;
    using System.ComponentModel;

    public enum DisplayControlTypeEnum
    {
        [Description("复选框")]
        CheckBox = 2,
        [Description("复选框列表")]
        CheckBoxList = 0x13,
        [Description("日期控件")]
        DateTimeControl = 4,
        [Description("下拉")]
        DropDownControl = 6,
        [Description("图片")]
        Image = 7,
        [Description("整数")]
        IntControl = 8,
        [Description("标签")]
        Lable = 1,
        [Description("长整数")]
        LongControl = 9,
        [Description("默认多文件上传")]
        MultiUploader = 15,
        [Description("密码")]
        PasswordControl = 10,
        [Description("弹出")]
        PopUpControl = 11,
        [Description("单选框")]
        RadioButtonList = 3,
        [Description("单选框列表")]
        RadioList = 0x12,
        [Description("实数控件")]
        RealControl = 5,
        [Description("Silverlight图片上传")]
        SlUploader = 0x10,
        [Description("Silverlight图片预览")]
        SlViewer = 0x11,
        [Description("单行文本")]
        StringControl = 12,
        [Description("多行文本")]
        TextControl = 13,
        [Description("层级显示")]
        TreeLevel = 14
    }
}

