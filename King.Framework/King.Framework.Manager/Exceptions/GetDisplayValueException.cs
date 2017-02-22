namespace King.Framework.Manager.Exceptions
{
    using King.Framework.Repository.Schemas;
    using System;
    using System.Runtime.InteropServices;

    public class GetDisplayValueException : Exception
    {
        public GetDisplayValueException() : base("根据对象ID查找对象的现实字段值失败")
        {
        }

        public GetDisplayValueException(IEntitySchema es, object id, string msg = "没有自定义消息") : base(string.Format("查询对象:{0},Id:{1} 的字段：{2} 值失败 : {3}", new object[] { es.EntityName, id, es.DisplayName, msg }))
        {
        }
    }
}
