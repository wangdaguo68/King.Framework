namespace King.Framework.Common
{
    using King.Framework.EntityLibrary;
    using King.Framework.Linq;
    using System;
    using System.Threading.Tasks;
    using System.Transactions;

    public class DbLogger : ILogger
    {
        public static readonly string ConnectionString_Log = "log";

        private void InsertLog(SysLog log)
        {
            TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress);
            try
            {
                using (DataContext context = new DataContext(ConnectionString_Log, true))
                {
                    context.Insert(log);
                }
            }
            catch (Exception exception)
            {
                EventLogHelper.Error2EventLog(log.ToString("异步写日志错误,原日志信息为:"), exception);
                EventLogHelper.Error2EventLog("插日志到数据库出错", exception);
            }
            finally
            {
                if (scope != null)
                {
                    scope.Dispose();
                }
            }
        }

        public void Log(SysLog log)
        {
            Task.Run((Action) (() => this.InsertLog(log)));
        }
    }
}
