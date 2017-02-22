namespace King.Framework.Linq.Data.ODP
{
    using King.Framework.Linq;
    using King.Framework.Linq.Data.Common;
    using King.Framework.Linq.Data.OracleCore;
    using System;

    public class ODPQueryProvider : OracleEntityProvider
    {
        public ODPQueryProvider(DataContext context, QueryMapping mapping, QueryPolicy policy) : base(context, mapping, policy)
        {
        }

        protected override QueryExecutor CreateExecutor()
        {
            return new ODPExecutor(this);
        }

        public static Type AdoConnectionType
        {
            get
            {
                return AdoOracleDataProvider.Default.DbConnectionType;
            }
        }

        public override King.Framework.Linq.Data.OracleCore.AdoProvider AdoProvider
        {
            get
            {
                return AdoOracleDataProvider.Default;
            }
        }
    }
}
