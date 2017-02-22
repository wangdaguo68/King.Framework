namespace King.Framework.Manager
{
    using King.Framework.EntityLibrary;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public interface IAttachHandler
    {
        void DeleteAttachment(T_Attachment attach);
        T_Attachment GetAttachment(int id);
        List<T_Attachment> GetAttachment(Expression<Func<T_Attachment, bool>> predicate);
        int SaveAttachment(T_Attachment attach);
        void UpdateAttachment(T_Attachment attach);
        void UpdateAttachment(T_Attachment attach, Expression<Func<T_Attachment, object>> cols);
    }
}
