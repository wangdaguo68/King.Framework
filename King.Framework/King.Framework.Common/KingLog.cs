namespace King.Framework.Common
{
    using System;

    [Obsolete("请使用AppLogHelper代替")]
    public static class KingLog
    {
        public static void Add(Exception e)
        {
            AppLogHelper.Error(e);
        }

        public static void WriteFile(string msg)
        {
            AppLogHelper.Information(msg);
        }
    }
}
