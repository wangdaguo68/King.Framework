namespace King.Framework.Linq.Data.Common
{
    using King.Framework.Linq;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq.Expressions;

    public class UpdateCommand : CommandExpression
    {
        private ReadOnlyCollection<ColumnAssignment> assignments;
        private TableExpression table;
        private Expression where;

        public UpdateCommand(TableExpression table, Expression where, IEnumerable<ColumnAssignment> assignments) : base(DbExpressionType.Update, typeof(int))
        {
            this.table = table;
            this.where = where;
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

        public Expression Where
        {
            get
            {
                return this.where;
            }
        }
    }
}
