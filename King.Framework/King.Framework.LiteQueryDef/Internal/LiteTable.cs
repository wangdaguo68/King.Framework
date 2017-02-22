using System.Collections.Generic;

namespace King.Framework.LiteQueryDef.Internal
{
    using System;
    using System.ComponentModel;

    public class LiteTable
    {
        public readonly List<LiteJoinTable> JoinTables = new List<LiteJoinTable>();
        public readonly string TableAlias;
        public readonly string TableName;

        internal LiteTable(string _tableName, string _tableAlias)
        {
            this.TableName = _tableName;
            this.TableAlias = _tableAlias;
        }
    }
}
