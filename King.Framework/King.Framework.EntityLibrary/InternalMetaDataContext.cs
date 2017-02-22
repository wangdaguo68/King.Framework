using System.Configuration;

namespace King.Framework.EntityLibrary
{
    using King.Framework.Linq;
    using System;
    using System.Runtime.InteropServices;

    internal class InternalMetaDataContext : DataContext
    {
        public static readonly string PackageId = ConfigurationManager.AppSettings["Package"];

        public InternalMetaDataContext(bool openConnection = true) : base("MetaEntities", openConnection)
        {
        }

        public InternalMetaDataContext(string connectionStringName, bool openConnection = true) : base(connectionStringName, openConnection)
        {
        }

        public InternalMetaDataContext(string connectionStringName, string providerName, bool openConnection = true) : base(connectionStringName, providerName, openConnection)
        {
        }

        public override long GetNextIdentity(bool rollBack = false)
        {
            long num3;
            try
            {
                if (string.IsNullOrEmpty(PackageId))
                {
                    throw new ApplicationException("请配置当前的Package！");
                }
                long num = PackageId.ToLong();
                int num2 = this.GetNextIdentity_Int(rollBack);
                num3 = (num * 0x3b9aca00L) + num2;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return num3;
        }
    }
}

