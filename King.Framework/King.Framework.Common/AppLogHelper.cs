namespace King.Framework.Common
{
    using King.Framework.EntityLibrary;
    using King.Framework.Ioc;
    using System;
    using System.Configuration;
    using System.Runtime.InteropServices;
    using System.Text;

    public static class AppLogHelper
    {
        public static readonly string Config_AppId = "AppId";
        public static readonly string Config_ServerId = "ServerId";
        private static readonly ILogger logger;

        static AppLogHelper()
        {
            if (IocHelper.IsRegistered(typeof(ILogger)))
            {
                logger = IocHelper.GetInstance<ILogger>();
            }
            else
            {
                logger = new FileLogger();
            }
        }

        public static void Error(Exception ex)
        {
            try
            {
                InternalLog(LogTypeEnum.Error, 0, ex.ToString(), null, 0, 0, null, null);
            }
            catch
            {
            }
        }

        public static void Error(int operationUserId, Exception ex)
        {
            try
            {
                InternalLog(LogTypeEnum.Error, operationUserId, ex.ToString(), null, 0, 0, null, null);
            }
            catch
            {
            }
        }

        public static void Error(string data, Exception ex)
        {
            try
            {
                InternalLog(LogTypeEnum.Error, 0, ex.ToString(), data, 0, 0, null, null);
            }
            catch
            {
            }
        }

        public static void Error(Exception ex, string format, params object[] args)
        {
            try
            {
                InternalLog(LogTypeEnum.Error, 0, ex.ToString(), string.Format(format, args), 0, 0, null, null);
            }
            catch
            {
            }
        }

        public static void Error(int operationUserId, Exception ex, string operationData)
        {
            try
            {
                InternalLog(LogTypeEnum.Error, operationUserId, ex.ToString(), operationData, 0, 0, null, null);
            }
            catch
            {
            }
        }

        public static void Error(int operationUserId, string errorInfo, string operationData)
        {
            InternalLog(LogTypeEnum.Error, operationUserId, errorInfo, operationData, 0, 0, null, null);
        }

        public static void Error(int operationUserId, Exception ex, string format, params object[] args)
        {
            try
            {
                InternalLog(LogTypeEnum.Error, operationUserId, ex.ToString(), string.Format(format, args), 0, 0, null, null);
            }
            catch
            {
            }
        }

        public static void FailureAudit(string msg)
        {
            try
            {
                InternalLog(LogTypeEnum.FailureAudit, 0, msg, null, 0, 0, null, null);
            }
            catch
            {
            }
        }

        public static void FailureAudit(string format, params object[] args)
        {
            try
            {
                InternalLog(LogTypeEnum.FailureAudit, 0, string.Format(format, args), null, 0, 0, null, null);
            }
            catch
            {
            }
        }

        public static void FormatError(string format, params object[] args)
        {
            try
            {
                InternalLog(LogTypeEnum.Error, 0, string.Format(format, args), null, 0, 0, null, null);
            }
            catch
            {
            }
        }

        public static void FormatError(int operationUserId, string format, params object[] args)
        {
            try
            {
                InternalLog(LogTypeEnum.Error, operationUserId, string.Format(format, args), null, 0, 0, null, null);
            }
            catch (Exception exception)
            {
                EventLogHelper.Error2EventLog(string.Empty, exception);
            }
        }

        public static void FormatInformation(string format, params object[] args)
        {
            try
            {
                InternalLog(LogTypeEnum.Information, 0, string.Format(format, args), null, 0, 0, null, null);
            }
            catch
            {
            }
        }

        public static void FormatWarning(string format, params object[] args)
        {
            try
            {
                InternalLog(LogTypeEnum.Warning, 0, string.Format(format, args), null, 0, 0, null, null);
            }
            catch
            {
            }
        }

        private static bool GetAppSettingAsBool(string key)
        {
            string str = ConfigurationManager.AppSettings[key];
            if (!string.IsNullOrWhiteSpace(str))
            {
                if (str == "1")
                {
                    return true;
                }
                str = str.ToLower();
                return (((str == "true") || (str == "yes")) || (str == "t"));
            }
            return false;
        }

        private static int GetAppSettingAsInt(string key)
        {
            string str = ConfigurationManager.AppSettings[key];
            if (!string.IsNullOrWhiteSpace(str))
            {
                return Convert.ToInt32(str);
            }
            return 0;
        }

        private static string GetAppSettingAsString(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static void Information(string msg)
        {
            try
            {
                InternalLog(LogTypeEnum.Information, 0, msg, null, 0, 0, null, null);
            }
            catch
            {
            }
        }

        public static void Information(string format, params object[] args)
        {
            try
            {
                InternalLog(LogTypeEnum.Information, 0, string.Format(format, args), null, 0, 0, null, null);
            }
            catch
            {
            }
        }

        internal static void InternalLog(LogTypeEnum logType, int operationUserId, string errorMessage, string operationData, int ext1 = 0, int ext2 = 0, string ext3 = null, string ext4 = null)
        {
            try
            {
                SysLog log = new SysLog {
                    ServerId = GetAppSettingAsInt(Config_ServerId),
                    AppId = GetAppSettingAsInt(Config_AppId),
                    CreateTime = DateTime.Now,
                    OperationUserId = operationUserId,
                    Data = operationData,
                    Info = errorMessage,
                    LogType = logType,
                    Ext1 = ext1,
                    Ext2 = ext2,
                    Ext3 = ext3,
                    Ext4 = ext4
                };
                LogAsync(log);
            }
            catch (Exception exception)
            {
                StringBuilder builder = new StringBuilder(0x400);
                builder.Append("写日志失败:").Append("logType=").Append(logType).Append(";operationUserId=").Append(operationUserId).Append(";errorMessage=").Append(errorMessage).Append(";operationData=").Append(operationData).Append(";ext1=").Append(ext1).Append(";ext2=").Append(ext2).Append(";ext3=").Append(ext3).Append(";ext4=").Append(ext4);
                EventLogHelper.Error2EventLog(builder.ToString(), exception);
            }
        }

        public static void LogAsync(SysLog log)
        {
            try
            {
                logger.Log(log);
            }
            catch (Exception exception)
            {
                EventLogHelper.Error2EventLog(log.ToString("异步写日志错误,原日志信息为:"), exception);
            }
        }

        public static void SuccessAudit(string msg)
        {
            try
            {
                InternalLog(LogTypeEnum.SuccessAudit, 0, msg, null, 0, 0, null, null);
            }
            catch
            {
            }
        }

        public static void SuccessAudit(string format, params object[] args)
        {
            try
            {
                InternalLog(LogTypeEnum.SuccessAudit, 0, string.Format(format, args), null, 0, 0, null, null);
            }
            catch
            {
            }
        }

        public static void Warning(Exception ex)
        {
            try
            {
                InternalLog(LogTypeEnum.Warning, 0, ex.ToString(), null, 0, 0, null, null);
            }
            catch
            {
            }
        }

        public static void Warning(int operationUserId, Exception ex)
        {
            try
            {
                InternalLog(LogTypeEnum.Warning, operationUserId, ex.ToString(), null, 0, 0, null, null);
            }
            catch
            {
            }
        }

        public static void Warning(string msg, Exception ex)
        {
            try
            {
                InternalLog(LogTypeEnum.Warning, 0, ex.ToString(), msg, 0, 0, null, null);
            }
            catch
            {
            }
        }

        public static void Warning(string format, params object[] args)
        {
            try
            {
                InternalLog(LogTypeEnum.Warning, 0, null, string.Format(format, args), 0, 0, null, null);
            }
            catch
            {
            }
        }

        public static void Warning(Exception ex, string format, params object[] args)
        {
            try
            {
                InternalLog(LogTypeEnum.Warning, 0, ex.ToString(), string.Format(format, args), 0, 0, null, null);
            }
            catch
            {
            }
        }

        public static void Warning(int operationUserId, string errorMessage, string operationData)
        {
            try
            {
                InternalLog(LogTypeEnum.Warning, operationUserId, errorMessage, operationData, 0, 0, null, null);
            }
            catch
            {
            }
        }

        public static void Warning(int operationUserId, string format, params object[] args)
        {
            try
            {
                InternalLog(LogTypeEnum.Warning, operationUserId, null, string.Format(format, args), 0, 0, null, null);
            }
            catch
            {
            }
        }

        public static void Warning(int operationUserId, Exception ex, string format, params object[] args)
        {
            try
            {
                InternalLog(LogTypeEnum.Warning, operationUserId, ex.ToString(), string.Format(format, args), 0, 0, null, null);
            }
            catch
            {
            }
        }
    }
}
