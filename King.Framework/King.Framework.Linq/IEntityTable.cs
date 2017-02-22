namespace King.Framework.Linq
{
    using System;
    using System.Collections;
    using System.Linq;

    public interface IEntityTable : IUpdatable, IQueryable, IEnumerable
    {
        int Delete(object instance);
        object GetById(object id);
        int Insert(object instance);
        int InsertOrUpdate(object instance);
        int Update(object instance);

        IEntityProvider Provider { get; }

        string TableId { get; }
    }
}
