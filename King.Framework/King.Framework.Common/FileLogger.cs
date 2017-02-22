namespace King.Framework.Common
{
    using King.Framework.EntityLibrary;
    using System;
    using System.Configuration;
    using System.IO;
    using System.Text;

    public class FileLogger : ILogger
    {
        private DateTime LastDeleteTime = DateTime.Now.AddDays(-10.0);
        private static readonly string LogDir = GetLogDir();

        private void DeleteOldFiles()
        {
        }

        private static string GetLogDir()
        {
            string str = ConfigurationManager.AppSettings["logDir"];
            if (!string.IsNullOrWhiteSpace(str))
            {
                if (!Directory.Exists(str))
                {
                    Directory.CreateDirectory(str);
                }
                return str;
            }
            str = @"c:\logDir";
            if (!Directory.Exists(str))
            {
                Directory.CreateDirectory(str);
            }
            return str;
        }

        public void Log(SysLog log)
        {
            try
            {
                string str = DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                str = Path.Combine(LogDir, str);
                string contents = new StringBuilder(0x200).Append("\n").Append(DateTime.Now.ToString()).Append("   ").AppendLine(log.Info).ToString();
                File.AppendAllText(str, contents);
            }
            catch (Exception exception)
            {
                EventLogHelper.Error2EventLog(new StringBuilder(0x200).Append("插入日志出错:").AppendLine(log.ToString()).AppendLine("出错原因:").AppendLine(exception.ToString()).ToString(), new object[0]);
            }
        }
    }
}
