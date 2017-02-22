namespace King.Framework.Common
{
    using King.Framework.Linq;

    public interface IConnectionProvider
    {
        ConnectionInfo GetConnectionInfo();
    }
}
