namespace King.Framework.Linq
{
    using System;

    public interface IEntitySession
    {
        ISessionTable<T> GetTable<T>(string tableId);
        ISessionTable GetTable(Type elementType, string tableId);
        void SubmitChanges();

        IEntityProvider Provider { get; }
    }
}
