namespace King.Framework.Linq.Data.Common
{
    using System;
    using System.Linq.Expressions;

    public class DeleteCommand : CommandExpression
    {
        private TableExpression table;
        private Expression where;

        public DeleteCommand(TableExpression table, Expression where) : base(DbExpressionType.Delete, typeof(int))
        {
            this.table = table;
            this.where = where;
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
