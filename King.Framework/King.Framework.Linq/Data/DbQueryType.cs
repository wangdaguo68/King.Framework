namespace King.Framework.Linq.Data
{
    using King.Framework.Linq.Data.Common;
    using System;
    using System.Data;

    public class DbQueryType : QueryType
    {
        private System.Data.SqlDbType dbType;
        private int length;
        private bool notNull;
        private short precision;
        private short scale;

        public DbQueryType(System.Data.SqlDbType dbType, bool notNull, int length, short precision, short scale)
        {
            this.dbType = dbType;
            this.notNull = notNull;
            this.length = length;
            this.precision = precision;
            this.scale = scale;
        }

        public System.Data.DbType DbType
        {
            get
            {
                return DbTypeSystem.GetDbType(this.dbType);
            }
        }

        public override int Length
        {
            get
            {
                return this.length;
            }
        }

        public override bool NotNull
        {
            get
            {
                return this.notNull;
            }
        }

        public override short Precision
        {
            get
            {
                return this.precision;
            }
        }

        public override short Scale
        {
            get
            {
                return this.scale;
            }
        }

        public System.Data.SqlDbType SqlDbType
        {
            get
            {
                return this.dbType;
            }
        }
    }
}
