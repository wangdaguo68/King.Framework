namespace King.Framework.LiteQueryDef.Internal
{
    using System;
    using System.Collections.Generic;

    public class LiteJoinTable
    {
        public LiteFilter filter;
        public readonly List<LiteJoinTable> joinTables;
        public readonly JoinTypeEnum JoinType;
        public readonly LiteQuery query;
        public readonly string tableAlias;
        public readonly string tableName;

        public LiteJoinTable(LiteQuery query, string _tableName, string _tableAlias, JoinTypeEnum joinType) : this(query, _tableName, _tableAlias, joinType, null)
        {
        }

        public LiteJoinTable(LiteQuery query, string _tableName, string _tableAlias, JoinTypeEnum joinType, LiteFilter _filter)
        {
            this.joinTables = new List<LiteJoinTable>();
            this.query = query;
            this.tableName = _tableName;
            this.tableAlias = _tableAlias;
            this.filter = _filter;
        }

        public LiteJoinTable ON(LiteFilter filter)
        {
            this.filter = filter;
            return this;
        }

        public LiteQuery Where(LiteFilter filter)
        {
            this.query.Filter = filter;
            return this.query;
        }
    }
}
