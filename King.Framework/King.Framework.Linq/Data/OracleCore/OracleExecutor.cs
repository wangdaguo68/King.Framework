namespace King.Framework.Linq.Data.OracleCore
{
    using King.Framework.Linq;
    using King.Framework.Linq.Data;
    using King.Framework.Linq.Data.Common;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;

    public class OracleExecutor : DbEntityProvider.Executor
    {
        private OracleEntityProvider provider;

        public OracleExecutor(OracleEntityProvider provider) : base(provider)
        {
            this.provider = provider;
        }

        protected override void AddParameter(DbCommand command, QueryParameter parameter, object value)
        {
            DbQueryType queryType = (DbQueryType) parameter.QueryType;
            if (queryType == null)
            {
                queryType = (DbQueryType) base.Provider.Language.TypeSystem.GetColumnType(parameter.Type);
            }
            int length = queryType.Length;
            if ((length == 0) && DbTypeSystem.IsVariableLength(queryType.SqlDbType))
            {
                length = 0x7fffffff;
            }
            DbParameter parameter2 = this.provider.AdoProvider.CreateParameter(":" + parameter.Name, queryType.SqlDbType, length);
            parameter2.Value = value ?? DBNull.Value;
            command.Parameters.Add(parameter2);
        }

        private IEnumerable<int> ExecuteBatch(QueryCommand query, IEnumerable<object[]> paramSets, int batchSize)
        {
            DbCommand command = this.GetCommand(query, null);
            DataTable dataTable = new DataTable();
            int num = 0;
            int count = query.Parameters.Count;
            while (num < count)
            {
                QueryParameter parameter = query.Parameters[num];
                command.Parameters[num].SourceColumn = parameter.Name;
                dataTable.Columns.Add(parameter.Name, King.Framework.Linq.TypeHelper.GetNonNullableType(parameter.Type));
                num++;
            }
            DbDataAdapter iteratorVariable2 = this.provider.AdoProvider.CreateDataAdapter();
            iteratorVariable2.InsertCommand = command;
            iteratorVariable2.InsertCommand.UpdatedRowSource = UpdateRowSource.None;
            iteratorVariable2.UpdateBatchSize = batchSize;
            this.LogMessage("-- Start SQL Batching --");
            this.LogMessage("");
            this.LogCommand(query, null);
            IEnumerator<object[]> enumerator = paramSets.GetEnumerator();
            using (enumerator)
            {
                bool iteratorVariable4 = true;
                while (iteratorVariable4)
                {
                    int iteratorVariable5 = 0;
                    while ((iteratorVariable5 < iteratorVariable2.UpdateBatchSize) && (iteratorVariable4 = enumerator.MoveNext()))
                    {
                        object[] values = enumerator.Current;
                        dataTable.Rows.Add(values);
                        this.LogParameters(query, values);
                        this.LogMessage("");
                        iteratorVariable5++;
                    }
                    if (iteratorVariable5 > 0)
                    {
                        int iteratorVariable6 = iteratorVariable2.Update(dataTable);
                        for (int i = 0; i < iteratorVariable5; i++)
                        {
                            yield return ((i < iteratorVariable6) ? 1 : 0);
                        }
                        dataTable.Rows.Clear();
                    }
                }
            }
            this.LogMessage(string.Format("-- End SQL Batching --", new object[0]));
            this.LogMessage("");
        }

        public override IEnumerable<int> ExecuteBatch(QueryCommand query, IEnumerable<object[]> paramSets, int batchSize, bool stream)
        {
            IEnumerable<int> enumerable2;
            base.StartUsingConnection();
            try
            {
                IEnumerable<int> source = this.ExecuteBatch(query, paramSets, batchSize);
                if (!(stream && !base.ActionOpenedConnection))
                {
                    return source.ToList<int>();
                }
                enumerable2 = new EnumerateOnce<int>(source);
            }
            finally
            {
                base.StopUsingConnection();
            }
            return enumerable2;
        }

        protected override bool BufferResultRows
        {
            get
            {
                return !this.provider.AllowsMultipleActiveResultSets;
            }
        }

      
    }
}
