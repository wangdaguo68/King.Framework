using System.Collections.Generic;

namespace King.Framework.Linq
{
    using System;
    using System.Collections;
    using System.Linq;

    public interface ISessionTable : IQueryable, IEnumerable
    {
        object GetById(object id);
        SubmitAction GetSubmitAction(object instance);
        void SetSubmitAction(object instance, SubmitAction action);

        IEntityTable ProviderTable { get; }

        IEntitySession Session { get; }
    }

    public interface ISessionTable<T> : IQueryable<T>, IEnumerable<T>, ISessionTable, IQueryable, IEnumerable
    {
        T GetById(object id);
        SubmitAction GetSubmitAction(T instance);
        void SetSubmitAction(T instance, SubmitAction action);

        IEntityTable<T> ProviderTable { get; }
    }
}
