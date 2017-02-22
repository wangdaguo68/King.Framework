namespace King.Framework.Linq.Data.OracleCore
{
    using King.Framework.Linq;
    using King.Framework.Linq.Data;
    using King.Framework.Linq.Data.Common;
    using System;

    public class OracleEntityProvider : DbEntityProvider
    {
        public OracleEntityProvider(DataContext context, QueryMapping mapping, QueryPolicy policy) : base(context, PLSqlLanguage.Default, mapping, policy)
        {
        }

        protected override QueryExecutor CreateExecutor()
        {
            return new OracleExecutor(this);
        }

        public virtual King.Framework.Linq.Data.OracleCore.AdoProvider AdoProvider
        {
            get
            {
                throw new NotImplementedException("AdoProvider need to be implemented.");
            }
        }

        public bool AllowsMultipleActiveResultSets
        {
            get
            {
                return false;
            }
        }
    }
}
