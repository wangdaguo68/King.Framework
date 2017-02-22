namespace King.Framework.Linq.Data.Common
{
    using System;

    public class ColumnExpression : DbExpression, IEquatable<ColumnExpression>
    {
        private TableAlias alias;
        private string name;
        private King.Framework.Linq.Data.Common.QueryType queryType;

        public ColumnExpression(Type type, King.Framework.Linq.Data.Common.QueryType queryType, TableAlias alias, string name) : base(DbExpressionType.Column, type)
        {
            if (queryType == null)
            {
                throw new ArgumentNullException("queryType");
            }
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            this.alias = alias;
            this.name = name;
            this.queryType = queryType;
        }

        public bool Equals(ColumnExpression other)
        {
            return (((other != null) && (this == other)) || ((this.alias == other.alias) && (this.name == other.Name)));
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as ColumnExpression);
        }

        public override int GetHashCode()
        {
            return (this.alias.GetHashCode() + this.name.GetHashCode());
        }

        public override string ToString()
        {
            return (this.Alias.ToString() + ".C(" + this.name + ")");
        }

        public TableAlias Alias
        {
            get
            {
                return this.alias;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public King.Framework.Linq.Data.Common.QueryType QueryType
        {
            get
            {
                return this.queryType;
            }
        }
    }
}
