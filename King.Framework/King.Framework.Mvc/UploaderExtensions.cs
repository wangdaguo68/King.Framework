namespace King.Framework.Mvc
{
    using King.Framework.Common;
    using King.Framework.EntityLibrary;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;

    public static class UploaderExtensions
    {
        public static MvcHtmlString KingUploader(this HtmlHelper helper, string name, string fileType = null, string fileSize = null, int fileCount = 1, bool readOnly = false, string jsCallBack = null)
        {
            UploaderModel model = new UploaderModel {
                Name = name
            };
            helper.RenderPartial("~/Views/ControlScripts/_UploaderScripts.cshtml", model);
            List<string> keyList = new List<string>();
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("<div class=\"divUpload\">");
            if (!readOnly)
            {
                builder.AppendLine("<input type=\"file\" name=\"uploadify_" + name + "\" id=\"uploadify_" + name + "\" />");
            }
            builder.AppendLine("<input type=\"hidden\" class=\"currentFlag\" value=\"" + name + "\" />");
            builder.AppendLine("<input type=\"hidden\" id=\"" + name + "\" name=\"" + name + "\" />");
            builder.AppendLine("<input type=\"hidden\" id=\"hidCallBack_" + name + "\" name=\"hidCallBack_" + name + "\"  value=\"" + jsCallBack + "\" />");
            fileSize = string.IsNullOrEmpty(fileSize) ? string.Empty : fileSize;
            builder.AppendLine("<input type=\"hidden\" id=\"hidFileSize_" + name + "\" name=\"hidFileSize_" + name + "\" value=\"" + fileSize + "\" />");
            fileCount = (fileCount < 1) ? 1 : fileCount;
            builder.AppendLine(string.Concat(new object[] { "<input type=\"hidden\" id=\"hidFileCount_", name, "\" name=\"hidFileCount_", name, "\" value=\"", fileCount, "\" />" }));
            if (!string.IsNullOrEmpty(fileType))
            {
                StringBuilder builder2 = new StringBuilder();
                string[] strArray = fileType.Split(new char[] { ',', Convert.ToChar(0xff0c) }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string str in strArray)
                {
                    builder2.AppendFormat("*.{0};", str);
                }
                builder.AppendLine("<input type=\"hidden\" id=\"hidFileType_" + name + "\" name=\"hidFileType_" + name + "\" value=\"" + builder2.ToString() + "\" />");
            }
            builder.AppendLine("<div id=\"divAttachmentList_" + name + "\"></div>");
            builder.AppendLine("</div>");
            builder.AppendLine(CtrlScripts.RenderScript(helper, keyList));
            return MvcHtmlString.Create(builder.ToString());
        }

        public static MvcHtmlString KingUploaderFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, string name, string objectName = null, string fileType = null, string fileSize = null, int fileCount = 1, bool readOnly = false, string jsCallBack = null)
        {
            UploaderModel model = new UploaderModel {
                Name = name
            };
            helper.RenderPartial("~/Views/ControlScripts/_UploaderScripts.cshtml", model);
            List<string> keyList = new List<string>();
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression<TModel, TProperty>(expression, helper.ViewData);
            string s = (metadata.Model == null) ? "" : metadata.Model.ToString();
            StringBuilder builder = new StringBuilder();
            int objId = 0;
            using (BizDataContext context = new BizDataContext(true))
            {
                builder.AppendLine("<div class=\"divUpload\">");
                if (!readOnly)
                {
                    builder.AppendLine("<input type=\"file\" name=\"uploadify_" + name + "\" id=\"uploadify_" + name + "\" />");
                    builder.AppendLine("<input type=\"hidden\" class=\"currentFlag\" value=\"" + name + "\" />");
                }
                string str2 = string.Empty;
                StringBuilder builder2 = new StringBuilder();
                builder2.AppendLine("<div id=\"divAttachmentList_" + name + "\">");
                if (int.TryParse(s, out objId))
                {
                    List<T_Attachment> list2 = context.Where<T_Attachment>(a => ((a.State == 0) && (a.OwnerObjectId == objId)) && (string.IsNullOrWhiteSpace(objectName) || (a.OwnerEntityType == objectName)));
                    if ((list2 != null) && (list2.Count > 0))
                    {
                        foreach (T_Attachment attachment in list2)
                        {
                            str2 = str2 + attachment.Attachment_ID + ",";
                            builder2.AppendLine("<div class='CurrCount_" + name + "'>");
                            if (!readOnly)
                            {
                                builder2.AppendLine(string.Concat(new object[] { "<a href='javascript:void(0)' onclick='DeleteAttachment(this)' extAttachmentName='", name, "' extAttachmentId='", attachment.Attachment_ID, "'>[删除]</a>&nbsp;" }));
                            }
                            builder2.AppendLine(string.Concat(new object[] { "<a href='/Attachment/Download?attachmentId=", attachment.Attachment_ID, "'>", attachment.DisplayName, "</a></div>" }));
                        }
                    }
                }
                builder2.AppendLine("</div>");
                builder.AppendLine("<input type=\"hidden\" id=\"" + name + "\" name=\"" + name + "\" value=\"" + str2 + "\" />");
                builder.AppendLine("<input type=\"hidden\" id=\"hidCallBack_" + name + "\" name=\"hidCallBack_" + name + "\"  value=\"" + jsCallBack + "\" />");
                fileSize = string.IsNullOrEmpty(fileSize) ? string.Empty : fileSize;
                builder.AppendLine("<input type=\"hidden\" id=\"hidFileSize_" + name + "\" name=\"hidFileSize_" + name + "\" value=\"" + fileSize + "\" />");
                fileCount = (fileCount < 1) ? 1 : fileCount;
                builder.AppendLine(string.Concat(new object[] { "<input type=\"hidden\" id=\"hidFileCount_", name, "\" name=\"hidFileCount_", name, "\" value=\"", fileCount, "\" />" }));
                if (!string.IsNullOrEmpty(fileType))
                {
                    StringBuilder builder3 = new StringBuilder();
                    string[] strArray = fileType.Split(new char[] { ',', Convert.ToChar(0xff0c) }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string str3 in strArray)
                    {
                        builder3.AppendFormat("*.{0};", str3);
                    }
                    builder.AppendLine("<input type=\"hidden\" id=\"hidFileType_" + name + "\" name=\"hidFileType_" + name + "\" value=\"" + builder3.ToString() + "\" />");
                }
                builder.AppendLine(builder2.ToString());
                builder.AppendLine("</div>");
                builder.AppendLine(CtrlScripts.RenderScript(helper, keyList));
            }
            return MvcHtmlString.Create(builder.ToString());
        }
    }
}

