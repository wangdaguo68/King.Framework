namespace King.Framework.Manager.Exceptions
{
    using King.Framework.Repository.Schemas;
    using System;
    using System.Runtime.InteropServices;

    public class DeleteException : ApplicationException
    {
        public DeleteException() : base("删除对象失败")
        {
        }

        public DeleteException(string msg) : base(msg)
        {
        }

        public DeleteException(IEntitySchema es, object id, string msg = "没有自定义消息") : base(string.Format("删除对象:{0},Id:{1} 时失败 ,原因 : {2}", es.EntityName, id, msg))
        {
        }
    }
}
