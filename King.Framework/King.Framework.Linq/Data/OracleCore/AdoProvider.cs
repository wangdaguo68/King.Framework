namespace King.Framework.Linq.Data.OracleCore
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Reflection;

    public class AdoProvider
    {
        protected System.Reflection.Assembly _assembly;
        protected Type _dbConnectionType;
        protected string _dbConnectionTypeName;
        protected Type _dbDataAdapterType;
        protected string _dbDataAdapterTypeName;
        protected Type _dbParameterType;
        protected string _dbParameterTypeName;
        protected PropertyInfo _dbTypeProperty;
        protected string _dbTypePropertyName;
        protected Type _dbTypeType;
        protected string _dbTypeTypeName;
        protected Dictionary<SqlDbType, int> _oracleTypeMapping = new Dictionary<SqlDbType, int>();

        public virtual DbDataAdapter CreateDataAdapter()
        {
            return (DbDataAdapter) Activator.CreateInstance(this.DbDataAdapterType);
        }

        public virtual DbParameter CreateParameter(string name, SqlDbType dbType, int size)
        {
            DbParameter parameter = (DbParameter) Activator.CreateInstance(this.DbParameterType);
            parameter.ParameterName = name;
            this.DbTypeProperty.SetValue(parameter, this.ToOracleType(dbType));
            parameter.Size = size;
            return parameter;
        }

        public virtual int GetOracleType(string name)
        {
            return Convert.ToInt32(this.DbTypeType.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static).GetValue(null));
        }

        public virtual int ToOracleType(SqlDbType dbType)
        {
            if (!this._oracleTypeMapping.ContainsKey(dbType))
            {
                throw new NotSupportedException(string.Format("The SQL type '{0}' is not supported", dbType));
            }
            return this._oracleTypeMapping[dbType];
        }

        public virtual System.Reflection.Assembly Assembly
        {
            get
            {
                return this._assembly;
            }
        }

        public virtual Type DbConnectionType
        {
            get
            {
                if (this._dbConnectionType == null)
                {
                    this._dbConnectionType = this.Assembly.GetType(this._dbConnectionTypeName);
                }
                return this._dbConnectionType;
            }
        }

        public virtual Type DbDataAdapterType
        {
            get
            {
                if (this._dbDataAdapterType == null)
                {
                    this._dbDataAdapterType = this.Assembly.GetType(this._dbDataAdapterTypeName);
                }
                return this._dbDataAdapterType;
            }
        }

        public virtual Type DbParameterType
        {
            get
            {
                if (this._dbParameterType == null)
                {
                    this._dbParameterType = this.Assembly.GetType(this._dbParameterTypeName);
                }
                return this._dbParameterType;
            }
        }

        public virtual PropertyInfo DbTypeProperty
        {
            get
            {
                if (this._dbTypeProperty == null)
                {
                    this._dbTypeProperty = this.DbParameterType.GetProperty(this._dbTypePropertyName);
                }
                return this._dbTypeProperty;
            }
        }

        public virtual Type DbTypeType
        {
            get
            {
                if (this._dbTypeType == null)
                {
                    this._dbTypeType = this.Assembly.GetType(this._dbTypeTypeName);
                }
                return this._dbTypeType;
            }
        }
    }
}
