namespace King.Framework.Common
{
    using System;

    public static class ConstFieldNames
    {
        public const string CreateTime = "CreateTime";
        public const string CreateUserId = "CreateUserId";
        public static readonly string[] DefaultFieldNames = new string[] { "CreateTime", "CreateUserId", "UpdateTime", "UpdateUserId", "OwnerId", "State", "StateDetail" };
        public static readonly string[] DefaultLowerFieldNames = new string[] { "CreateTime".ToLower(), "CreateUserId".ToLower(), "UpdateTime".ToLower(), "UpdateUserId".ToLower(), "OwnerId".ToLower(), "State".ToLower(), "StateDetail".ToLower() };
        public const string Dept_Id = "Dept_Id";
        public const string OwnerId = "OwnerId";
        public const string State = "State";
        public const string StateDetail = "StateDetail";
        public const string SystemLevelCode = "SystemLevelCode";
        public const string UpdateTime = "UpdateTime";
        public const string UpdateUserId = "UpdateUserId";
    }
}
