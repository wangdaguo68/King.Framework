namespace King.Framework.Linq.Data.Common
{
    using King.Framework.Linq;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class InsertCommand : CommandExpression
    {
        private ReadOnlyCollection<ColumnAssignment> assignments;
        private TableExpression table;

        public InsertCommand(TableExpression table, IEnumerable<ColumnAssignment> assignments) : base(DbExpressionType.Insert, typeof(int))
        {
            this.table = table;
            this.assignments = assignments.ToReadOnly<ColumnAssignment>();
        }

        public ReadOnlyCollection<ColumnAssignment> Assignments
        {
            get
            {
                return this.assignments;
            }
        }

        public TableExpression Table
        {
            get
            {
                return this.table;
            }
        }
    }
}
