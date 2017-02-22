namespace King.Framework.Linq
{
    using King.Framework.DAL.DataAccessLib;
    using King.Framework.LiteQueryDef;
    using King.Framework.LiteQueryDef.Internal;
    using King.Framework.LiteQueryDef.Visitors;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;

    public abstract class DatabaseFactory
    {
        private static readonly Dictionary<string, DatabaseTypeEnum> dbTypeDict;
        private static readonly Dictionary<DatabaseTypeEnum, Func<LiteQuery, ILiteVisitor>> liteVisitorDict;

        static DatabaseFactory()
        {
            Dictionary<string, DatabaseTypeEnum> dictionary = new Dictionary<string, DatabaseTypeEnum>();
            dictionary.Add(DataContext.System_Data_SqlClient_Provider, DatabaseTypeEnum.SqlServer);
            dictionary.Add(DataContext.Oracle_DataAccess_Client_Provider, DatabaseTypeEnum.Oracle);
            dictionary.Add(DataContext.Oracle_ManagedDataAccess_Client_Provider, DatabaseTypeEnum.Oracle);
            dbTypeDict = dictionary;
            Dictionary<DatabaseTypeEnum, Func<LiteQuery, ILiteVisitor>> dictionary2 = new Dictionary<DatabaseTypeEnum, Func<LiteQuery, ILiteVisitor>>();
            dictionary2.Add(DatabaseTypeEnum.SqlServer, p => new SqlVisitor(p));
            dictionary2.Add(DatabaseTypeEnum.Oracle, p => new OracleVisitor(p));
            liteVisitorDict = dictionary2;
        }

        protected DatabaseFactory()
        {
        }

        internal static BaseDataAccess CreateDataAccess(DatabaseTypeEnum databaseType)
        {
            switch (databaseType)
            {
                case DatabaseTypeEnum.SqlServer:
                    return new SqlDataAccess();

                case DatabaseTypeEnum.Oracle:
                    return new OracleDataAccess();
            }
            return new SqlDataAccess();
        }

        internal static DbProviderFactory CreateDbProviderFactory(string providerInvariantName)
        {
            return DbProviderFactories.GetFactory(providerInvariantName);
        }

        internal static DatabaseTypeEnum GetDatabaseType(string providerName)
        {
            if (!dbTypeDict.ContainsKey(providerName))
            {
                throw new ApplicationException(string.Format("未找到对应的提供程序名：{0}", providerName));
            }
            return dbTypeDict[providerName];
        }

        internal static ILiteVisitor GetLiteVisitor(DatabaseTypeEnum dbType, LiteQuery q)
        {
            return liteVisitorDict[dbType](q);
        }
    }
}
