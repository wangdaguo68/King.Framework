namespace King.Framework.Linq.Data.Common
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq.Expressions;

    public sealed class ProjectedColumns
    {
        private ReadOnlyCollection<ColumnDeclaration> columns;
        private Expression projector;

        public ProjectedColumns(Expression projector, ReadOnlyCollection<ColumnDeclaration> columns)
        {
            this.projector = projector;
            this.columns = columns;
        }

        public ReadOnlyCollection<ColumnDeclaration> Columns
        {
            get
            {
                return this.columns;
            }
        }

        public Expression Projector
        {
            get
            {
                return this.projector;
            }
        }
    }
}
