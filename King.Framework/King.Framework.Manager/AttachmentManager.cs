namespace King.Framework.Manager
{
    using King.Framework.Common;
    using King.Framework.EntityLibrary;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Web;
    using System.Web.Script.Serialization;
    using System.Web.UI.WebControls;

    public class AttachmentManager : IAttachHandler
    {
        private BizDataContext _context;

        public AttachmentManager(BizDataContext _ctx)
        {
            this._context = _ctx;
        }

        public static bool CheckFilePath(string filePath)
        {
            return File.Exists(filePath);
        }

        private static T_Attachment CreateAttach(string name, string filePath, int size, string contentType)
        {
            if (!CheckFilePath(filePath))
            {
                throw new Exception("文件不存在！");
            }
            T_Attachment attachment = new T_Attachment {
                FileName = Path.GetFileName(filePath),
                DisplayName = name,
                FileSize = new int?(size),
                FilePath = filePath,
                ContentType = contentType,
                FileExtension = Path.GetExtension(name),
                FileData = GetFileData(filePath)
            };
            if (attachment.FileSize == -1)
            {
                attachment.FileSize = new int?(attachment.FileData.Length);
            }
            int? fileSize = attachment.FileSize;
            int length = attachment.FileData.Length;
            if ((fileSize.GetValueOrDefault() != length) || !fileSize.HasValue)
            {
                throw new Exception("文件长度异常，文件可能已损坏！");
            }
            return attachment;
        }

        public virtual void DeleteAttachment(T_Attachment att)
        {
            this._context.Delete(att);
        }


        public static T_Attachment GetAttachFromContext_Silverlight(HttpContext context)
        {
            HttpRequest request = context.Request;
            string path = HttpUtility.UrlDecode(request.QueryString["name"]);
            byte[] buffer = new byte[request.InputStream.Length];
            request.InputStream.Read(buffer, 0, buffer.Length);
            using (new BizDataContext(true))
            {
                return new T_Attachment { DisplayName = path, FileSize = new int?(buffer.Length), FileData = buffer, FileName = path, FileExtension = Path.GetExtension(path) };
            }
        }

        public virtual T_Attachment GetAttachment(int id)
        {
            return this.GetFileFromDB(id);
        }

        public virtual List<T_Attachment> GetAttachment(Expression<Func<T_Attachment, bool>> predicate)
        {
            return this._context.Where<T_Attachment>(predicate);
        }



        public IQueryable<T_Attachment> GetByIds(List<int> attachmentIdList)
        {
            return null;
        }

        public static byte[] GetFileData(string filePath)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            {
                byte[] buffer = new byte[stream.Length];
                if (stream.CanRead)
                {
                    stream.Seek(0L, SeekOrigin.Begin);
                    int count = 0x1000;
                    int num2 = ((int) (stream.Length / ((long) count))) + 1;
                    for (int i = 0; i < num2; i++)
                    {
                        if (i == (num2 - 1))
                        {
                            if ((stream.Length % ((long) count)) > 0L)
                            {
                                stream.Read(buffer, i * count, (int) (stream.Length % ((long) count)));
                            }
                        }
                        else
                        {
                            stream.Read(buffer, i * count, count);
                        }
                    }
                }
                stream.Close();
                return buffer;
            }
        }

        private T_Attachment GetFileFromDB(int id)
        {
            T_Attachment attachment = this._context.FindById<T_Attachment>(new object[] { id });
            if (attachment != null)
            {
                return attachment;
            }
            return null;
        }

        public static string GetJsonFromAttachment(T_Attachment atm)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(new { id = Guid.NewGuid().ToString(), name = atm.DisplayName, size = atm.FileSize, loaded = atm.FileData.Length, percent = GetPercent(atm), status = 5 });
        }

        public static string GetJsonFromAttachment(List<T_Attachment> atmList)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(from p in atmList select new { id = Guid.NewGuid().ToString(), name = p.DisplayName, size = p.FileSize, loaded = p.FileData.Length, percent = GetPercent(p), status = 5, attachId = p.Attachment_ID });
        }

        public static int GetPercent(T_Attachment p)
        {
            int? nullable;
            if (!(p.FileSize.HasValue && !(((nullable = p.FileSize).GetValueOrDefault() == p.FileData.Length) && nullable.HasValue)))
            {
                return 100;
            }
            double a = (p.FileData.Length * 100.0) / ((double) p.FileSize.Value);
            return (int) Math.Ceiling(a);
        }

        private int Save(T_Attachment attachment)
        {
            int num;
            try
            {
                attachment.Attachment_ID = this._context.GetNextIdentity_Int(false);
                this._context.Insert(attachment);
                num = attachment.Attachment_ID;
            }
            catch (ApplicationException exception)
            {
                throw exception;
            }
            return num;
        }

        public virtual int SaveAttachment(T_Attachment attach)
        {
            if (File.Exists(attach.FilePath))
            {
                File.Delete(attach.FilePath);
            }
            return this.Save(attach);
        }

        public virtual void UpdateAttachment(T_Attachment att)
        {
            this._context.Update(att);
        }

        public virtual void UpdateAttachment(T_Attachment att, Expression<Func<T_Attachment, object>> cols)
        {
            this._context.UpdatePartial<T_Attachment>(att, cols);
        }
    }
}
