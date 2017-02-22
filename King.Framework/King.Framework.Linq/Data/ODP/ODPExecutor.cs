namespace King.Framework.Linq.Data.ODP
{
    using King.Framework.Linq.Data.Common;
    using King.Framework.Linq.Data.OracleCore;
    using System;
    using System.Data.Common;
    using System.Reflection;

    public class ODPExecutor : OracleExecutor
    {
        private static PropertyInfo _bindByNameProperty;

        public ODPExecutor(OracleEntityProvider provider) : base(provider)
        {
        }

        protected override DbCommand GetCommand(QueryCommand query, object[] paramValues)
        {
            DbCommand command = base.GetCommand(query, paramValues);
            if (_bindByNameProperty == null)
            {
                _bindByNameProperty = command.GetType().GetProperty("BindByName");
            }
            _bindByNameProperty.SetValue(command, true);
            return command;
        }
    }
}
