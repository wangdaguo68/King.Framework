namespace King.Framework.Linq.Data.Common
{
    using System;

    public class QueryParameter
    {
        private string name;
        private King.Framework.Linq.Data.Common.QueryType queryType;
        private System.Type type;

        public QueryParameter(string name, System.Type type, King.Framework.Linq.Data.Common.QueryType queryType)
        {
            this.name = name;
            this.type = type;
            this.queryType = queryType;
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

        public System.Type Type
        {
            get
            {
                return this.type;
            }
        }
    }
}
