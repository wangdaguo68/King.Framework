namespace King.Framework.Linq.Data.ODP
{
    using King.Framework.Linq.Data.OracleCore;
    using Oracle.ManagedDataAccess.Client;
    using System;
    using System.Data;
    using System.Reflection;
    using System.Threading;

    public class AdoOracleDataProvider : AdoProvider
    {
        private static AdoOracleDataProvider _default;

        public AdoOracleDataProvider()
        {
            base._dbConnectionTypeName = "Oracle.ManagedDataAccess.Client.OracleConnection";
            base._dbTypeTypeName = "Oracle.ManagedDataAccess.Client.OracleDbType";
            base._dbParameterTypeName = "Oracle.ManagedDataAccess.Client.OracleParameter";
            base._dbDataAdapterTypeName = "Oracle.ManagedDataAccess.Client.OracleDataAdapter";
            base._dbTypePropertyName = "OracleDbType";
            base._oracleTypeMapping.Add(SqlDbType.BigInt, this.GetOracleType("Int64"));
            base._oracleTypeMapping.Add(SqlDbType.Binary, this.GetOracleType("Blob"));
            base._oracleTypeMapping.Add(SqlDbType.Bit, this.GetOracleType("Byte"));
            base._oracleTypeMapping.Add(SqlDbType.NChar, this.GetOracleType("NChar"));
            base._oracleTypeMapping.Add(SqlDbType.Char, this.GetOracleType("Char"));
            base._oracleTypeMapping.Add(SqlDbType.Date, this.GetOracleType("Date"));
            base._oracleTypeMapping.Add(SqlDbType.DateTime, this.GetOracleType("Date"));
            base._oracleTypeMapping.Add(SqlDbType.SmallDateTime, this.GetOracleType("Date"));
            base._oracleTypeMapping.Add(SqlDbType.Decimal, this.GetOracleType("Decimal"));
            base._oracleTypeMapping.Add(SqlDbType.Float, this.GetOracleType("Single"));
            base._oracleTypeMapping.Add(SqlDbType.Image, this.GetOracleType("Blob"));
            base._oracleTypeMapping.Add(SqlDbType.Int, this.GetOracleType("Int32"));
            base._oracleTypeMapping.Add(SqlDbType.Money, this.GetOracleType("Decimal"));
            base._oracleTypeMapping.Add(SqlDbType.SmallMoney, this.GetOracleType("Decimal"));
            base._oracleTypeMapping.Add(SqlDbType.NVarChar, this.GetOracleType("NVarchar2"));
            base._oracleTypeMapping.Add(SqlDbType.VarChar, this.GetOracleType("Varchar2"));
            base._oracleTypeMapping.Add(SqlDbType.SmallInt, this.GetOracleType("Int16"));
            base._oracleTypeMapping.Add(SqlDbType.NText, this.GetOracleType("Clob"));
            base._oracleTypeMapping.Add(SqlDbType.Text, this.GetOracleType("Clob"));
            base._oracleTypeMapping.Add(SqlDbType.Time, this.GetOracleType("TimeStamp"));
            base._oracleTypeMapping.Add(SqlDbType.Timestamp, this.GetOracleType("TimeStamp"));
            base._oracleTypeMapping.Add(SqlDbType.TinyInt, this.GetOracleType("Byte"));
            base._oracleTypeMapping.Add(SqlDbType.UniqueIdentifier, this.GetOracleType("Varchar2"));
            base._oracleTypeMapping.Add(SqlDbType.VarBinary, this.GetOracleType("Blob"));
            base._oracleTypeMapping.Add(SqlDbType.Xml, this.GetOracleType("XmlType"));
        }

        public override System.Reflection.Assembly Assembly
        {
            get
            {
                if (base._assembly == null)
                {
                    base._assembly = typeof(OracleConnection).Assembly;
                }
                return base._assembly;
            }
        }

        public static AdoOracleDataProvider Default
        {
            get
            {
                if (_default == null)
                {
                    Interlocked.CompareExchange<AdoOracleDataProvider>(ref _default, new AdoOracleDataProvider(), null);
                }
                return _default;
            }
        }
    }
}
