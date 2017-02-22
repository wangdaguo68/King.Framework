namespace King.Framework.Linq.Data.Common
{
    using King.Framework.Linq;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class QueryCommand
    {
        private string commandText;
        private ReadOnlyCollection<QueryParameter> parameters;

        public QueryCommand(string commandText, IEnumerable<QueryParameter> parameters)
        {
            this.commandText = commandText;
            this.parameters = parameters.ToReadOnly<QueryParameter>();
        }

        public string CommandText
        {
            get
            {
                return this.commandText;
            }
        }

        public ReadOnlyCollection<QueryParameter> Parameters
        {
            get
            {
                return this.parameters;
            }
        }
    }
}
