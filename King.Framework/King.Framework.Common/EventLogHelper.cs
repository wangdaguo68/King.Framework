namespace King.Framework.Common
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.Text;

    internal static class EventLogHelper
    {
        private static readonly string EventLog_Source = "Application";

        public static void Error2EventLog(string message, Exception ex)
        {
            try
            {
                StringBuilder builder = new StringBuilder(0x400);
                if (!string.IsNullOrEmpty(message))
                {
                    builder.Append(message);
                    builder.AppendLine(":");
                    builder.AppendLine();
                    builder.AppendLine();
                }
                while (ex != null)
                {
                    builder.AppendLine(ex.Message);
                    builder.AppendLine(ex.StackTrace);
                    builder.AppendLine();
                    ex = ex.InnerException;
                }
                EventLog.WriteEntry(EventLog_Source, builder.ToString(), EventLogEntryType.Error, 0, LogCategory);
            }
            catch
            {
            }
        }

        public static void Error2EventLog(string format, params object[] args)
        {
            try
            {
                EventLog.WriteEntry(EventLog_Source, string.Format(format, args), EventLogEntryType.Error, 0, LogCategory);
            }
            catch
            {
            }
        }

        public static void Error2EventLog(Exception ex, string format, params object[] args)
        {
            try
            {
                Error2EventLog(string.Format(format, args), ex);
            }
            catch
            {
            }
        }

        private static short LogCategory
        {
            get
            {
                short num;
                string str = ConfigurationManager.AppSettings["LogCategory"];
                if (!string.IsNullOrWhiteSpace(str) && short.TryParse(str, out num))
                {
                    return num;
                }
                return 0x3e7;
            }
        }
    }
}
