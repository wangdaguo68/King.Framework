namespace King.Framework.Linq
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public interface IEntityTable<T> : IEntityTable, IUpdatable<T>, IUpdatable, IQueryable<T>, IEnumerable<T>, IQueryable, IEnumerable
    {
        int Delete(T instance);
        T GetById(object id);
        int Insert(T instance);
        int InsertOrUpdate(T instance);
        int Update(T instance);
    }
}
