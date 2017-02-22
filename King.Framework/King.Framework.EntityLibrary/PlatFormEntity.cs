namespace King.Framework.EntityLibrary
{
    using King.Framework.Interfaces;
    using King.Framework.Ioc;
    using System;
    using System.Linq;

    public static class PlatFormEntity
    {
        public const long PackageDenominator = 0x3b9aca00L;
        public static readonly string TableName_Department = "T_Department";
        public static readonly string TableName_SysApproveActivityData = "SysApproveActivityData";
        public static readonly string TableName_User = "T_User";

        public static long SysApproveActivityData
        {
            get
            {
                return IocHelper.Resolve<IEntityProvider>().FindByName(TableName_SysApproveActivityData).EntityId;
            }
        }

        public static long SysDepartment
        {
            get
            {
                return IocHelper.Resolve<IEntityProvider>().FindByName(TableName_Department).EntityId;
            }
        }

        public static long SysUser
        {
            get
            {
                return IocHelper.Resolve<IEntityProvider>().FindByName(TableName_User).EntityId;
            }
        }

        public static long SysUser_UserId
        {
            get
            {
                IMetaEntity schema = IocHelper.Resolve<IEntityProvider>().FindByName(TableName_User);
                return schema.GetFields().FirstOrDefault<IMetaField>(p => (p.FieldName == schema.KeyName)).FieldId;
            }
        }
    }
}

