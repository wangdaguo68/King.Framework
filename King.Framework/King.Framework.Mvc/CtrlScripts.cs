namespace King.Framework.Mvc
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Web.Mvc;

    public static class CtrlScripts
    {
        public static readonly string bValidate = "<script src=\"/Scripts/jquery.bvalidator.js\"></script>";
        public static readonly string combobox = "<script src=\"/Scripts/combobox.js\"></script>";
        public static readonly string ComboboxCss = "<link href=\"/Content/combobox.css\" rel=\"stylesheet\" />";
        public static readonly string datePicteterZH = "<script src=\"/Scripts/jquery-ui-1.11.1/datepicker-zh-TW.js\"></script>";
        public static readonly string JqueryBvalidatorCss = "<link href=\"/Content/bvalidator.css\" rel=\"stylesheet\" />";
        public static readonly string JQueryUI = "<script src=\"/Scripts/jquery-ui-1.11.1/jquery-ui.min.js\"></script>";
        public static readonly string JqueryUiCss = "<link href=\"/Content/jquery-ui-1.11.1/jquery-ui.min.css\" rel=\"stylesheet\" />";
        public static readonly string timePicteter = "<script src=\"/Scripts/jquery-ui-1.11.1/jquery-ui-timepicker-addon.js\"></script>";
        public static readonly string timePicteterCss = "<link href=\"/Content/jquery-ui-1.11.1/jquery-ui-timepicker-addon.css\" rel=\"stylesheet\" />";
        public static readonly string timePicteterZH = "<script src=\"/Scripts/jquery-ui-1.11.1/jquery-ui-timepicker-zh-CN.js\" charset=\"gb2312\"></script>";
        public static readonly string TreeCommonScript = "<script src=\"/Scripts/TreeCustomScript.js\"></script>";
        public static readonly string TreeCss = "<link href=\"/Content/zTree/zTreeStyle.css\" rel=\"stylesheet\" />";
        public static readonly string TreeCustomScript = "<script src=\"/Scripts/jquery.ztree.all-3.5.js\"></script>";
        public static readonly string uploadify = "<script src=\"/Scripts/uploadify-V3.2.1/jquery.uploadify.min.js\"></script>";
        public static readonly string uploadifyAttachment = "<script src=\"/Scripts/uploadify-V3.2.1/uploadAttachment.js\"></script>";
        public static readonly string uploadifyCss = "<link href=\"/Scripts/uploadify-V3.2.1/uploadify.css\" rel=\"stylesheet\" />";
        public static readonly string validateCommon = "<script src=\"/Scripts/Common.js\"></script>";

        public static string RenderScript(HtmlHelper html, List<string> keyList)
        {
            StringBuilder builder = new StringBuilder();
            List<string> list = html.ViewContext.TempData["__scripts"] as List<string>;
            if (list == null)
            {
                list = new List<string>();
                html.ViewContext.TempData["__scripts"] = list;
            }
            if ((keyList != null) && (keyList.Count > 0))
            {
                foreach (string str in keyList)
                {
                    if (!list.Contains(str))
                    {
                        list.Add(str);
                        builder.AppendLine(str);
                    }
                }
            }
            return builder.ToString();
        }

        public static string RenderScript<TModel, TValue>(HtmlHelper<TModel> html, List<string> keyList)
        {
            StringBuilder builder = new StringBuilder();
            List<string> list = html.ViewContext.TempData["__scripts"] as List<string>;
            if (list == null)
            {
                list = new List<string>();
                html.ViewContext.TempData["__scripts"] = list;
            }
            if ((keyList != null) && (keyList.Count > 0))
            {
                foreach (string str in keyList)
                {
                    if (!list.Contains(str))
                    {
                        list.Add(str);
                        builder.AppendLine(str);
                    }
                }
            }
            return builder.ToString();
        }
    }
}

