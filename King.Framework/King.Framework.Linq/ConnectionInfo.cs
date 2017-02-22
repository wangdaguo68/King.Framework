namespace King.Framework.Linq
{
    using System;
    using System.Configuration;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;

    [Serializable]
    public class ConnectionInfo : ISerializable
    {
        private readonly string _connectionString;
        private static readonly ConnectionInfo _default = FromConfig(DataContext.BizConnectionStringDefault, true);
        private readonly string _providerName;

        public ConnectionInfo(SerializationInfo info, StreamingContext context)
        {
            this._connectionString = info.GetString("connectionString");
            this._providerName = info.GetString("providerName");
        }

        public ConnectionInfo(string connectionString, string providerName)
        {
            this._connectionString = connectionString;
            this._providerName = providerName;
        }

        public static ConnectionInfo FromConfig(string connectionStringName, bool allowReturnNull = false)
        {
            if (string.IsNullOrEmpty(connectionStringName))
            {
                throw new ArgumentNullException("connectionStringName");
            }
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[connectionStringName];
            if (settings != null)
            {
                string providerName = settings.ProviderName;
                return new ConnectionInfo(settings.ConnectionString, settings.ProviderName);
            }
            if (!allowReturnNull)
            {
                throw new ConfigurationErrorsException(string.Format("connectionStringName[{0}]未配置", connectionStringName));
            }
            return null;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("connectionString", this._connectionString);
            info.AddValue("providerName", this._providerName);
        }

        public string ConnectionString
        {
            get
            {
                return this._connectionString;
            }
        }

        public static ConnectionInfo Default
        {
            get
            {
                if (_default == null)
                {
                    throw new ConfigurationErrorsException(string.Format("connectionStringName[{0}]未配置", DataContext.BizConnectionStringDefault));
                }
                return _default;
            }
        }

        public string ProviderName
        {
            get
            {
                return this._providerName;
            }
        }
    }
}
