namespace King.Framework.Common
{
    using King.Framework.Linq;
    using System;

    public class DefaultConnectionProvider : IConnectionProvider
    {
        public ConnectionInfo GetConnectionInfo()
        {
            return ConnectionInfo.Default;
        }
    }
}
