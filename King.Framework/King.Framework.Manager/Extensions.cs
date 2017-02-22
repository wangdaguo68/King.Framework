namespace King.Framework.Manager
{
    using King.Framework.Common;
    using King.Framework.EntityLibrary;
    using King.Framework.Interfaces;
    using King.Framework.LiteQueryDef.Internal;
    using King.Framework.Repository.Schemas;
    using Microsoft.CSharp.RuntimeBinder;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;

    public static class Extensions
    {
        public static void AUD_OperationCheck(this IOperationManager opm, IEntitySchema es, object entity, EntityOperationEnum operationEnum)
        {
        }

        public static string CreateEntityKey(this IEntitySchema es, object id)
        {
            return string.Format("{0}_{1}", es.EntityName, id);
        }

        public static string GetCompareExpression(this QueryParma qp, Type ptype)
        {
            string str = "< ";
            switch (qp.CompareTypeEnum)
            {
                case CompareTypeEnum.LessThan:
                    str = " it.{0} < ";
                    break;

                case CompareTypeEnum.LessThanOrEqual:
                    str = " it.{0} <= ";
                    break;

                case CompareTypeEnum.Equal:
                    str = " it.{0} = ";
                    break;

                case CompareTypeEnum.GreaterThanOrEqual:
                    str = " it.{0} >= ";
                    break;

                case CompareTypeEnum.GreaterThan:
                    str = " it.{0} > ";
                    break;

                case CompareTypeEnum.EndWith:
                    str = " it.{0} LIKE \"%{1}\"";
                    break;

                case CompareTypeEnum.StartWith:
                    str = " it.{0} LIKE \"{1}%\"";
                    break;

                case CompareTypeEnum.Contains:
                    str = " it.{0} LIKE \"%{1}%\"";
                    break;

                case CompareTypeEnum.NotEqual:
                    str = " it.{0} != ";
                    break;
            }
            Type[] source = new Type[] { typeof(DateTime), typeof(DateTime?), typeof(string) };
            CompareTypeEnum[] enumArray = new CompareTypeEnum[] { CompareTypeEnum.StartWith, CompareTypeEnum.EndWith, CompareTypeEnum.Contains };
            if (!enumArray.Contains<CompareTypeEnum>(qp.CompareTypeEnum))
            {
                if (!source.Contains<Type>(ptype))
                {
                    str = str + "{1}";
                }
                else
                {
                    str = str + "\"{1}\"";
                }
            }
            Type[] typeArray2 = new Type[] { typeof(DateTime), typeof(DateTime?) };
            if (typeArray2.Contains<Type>(ptype))
            {
                str = str.Replace("\"", "").Replace("{1}", "cast('{1}' as System.DateTime)");
            }
            return str;
        }

        public static List<string> GetModifiedPropertys(this object modified, object persistenceObj, List<string> allPropNames)
        {
            Type type = modified.GetType();
            if (type != persistenceObj.GetType())
            {
                throw new ArgumentException(string.Format("modified type {0} , persistenceObj type {1} ,两者必须是同一种类型的对象", type, persistenceObj.GetType()));
            }
            List<string> list = new List<string>();
            foreach (string str in allPropNames)
            {
                object obj2 = modified.GetPropertyValue(str, type);
                object obj3 = persistenceObj.GetPropertyValue(str, type);
                if (((obj2 != null) && !obj2.Equals(obj3)) || ((obj3 != null) && !obj3.Equals(obj2)))
                {
                    list.Add(str);
                    persistenceObj.SetPropertyValue(str, obj2, null);
                }
            }
            return list;
        }

        public static Type GetPropertyType(this IEntitySchema es, string pname)
        {
            if (!es.PropertyTypes.ContainsKey(pname))
            {
                throw new Exception(string.Format("{0} 不是{1} 的有效属性", pname, es.EntityType));
            }
            return es.PropertyTypes[pname];
        }

        public static bool IsT_RoleTable(this IEntitySchema es)
        {
            return (es.EntityName == "T_Role");
        }



        public static King.Framework.EntityLibrary.PrivilegeModel PrivilegeModel(this IEntitySchema es)
        {
            return (King.Framework.EntityLibrary.PrivilegeModel) es.PrivilegeMode;
        }

        public static string Q_OperationCheck(this IOperationManager opm, IEntitySchema es, int objectId)
        {
            List<int> userIdByDeptId;
            string str2;
            IUserIdentity currentUser = opm.CurrentUser;
            if (currentUser == null)
            {
                throw new ApplicationException("登录用户为空，无法判断权限");
            }
            EntityPrivilegeEnum enum2 = opm.TryCanOperation(currentUser.User_ID, es.EntityId, EntityOperationEnum.Query);
            string str = "";
            if ((enum2 == EntityPrivilegeEnum.NoPermission) && !opm.CheckHasSharedPrivilege(objectId, es.EntityName, EntityOperationEnum.Query))
            {
                throw new ApplicationException(string.Format("当前登录的用户没有对 {0} 的查询权限", es.EntityName));
            }
            if (es.PrivilegeModel() == King.Framework.EntityLibrary.PrivilegeModel.Organization)
            {
                if ((enum2 != EntityPrivilegeEnum.AllRights) && !opm.CheckHasSharedPrivilege(objectId, es.EntityName, EntityOperationEnum.Query))
                {
                    throw new ApplicationException(string.Format("当前登录的用户没有对 {0} 的查询权限", es.EntityName));
                }
                return "";
            }
            if ((enum2 == EntityPrivilegeEnum.Personal) && !opm.CheckHasSharedPrivilege(objectId, es.EntityName, EntityOperationEnum.Query))
            {
                str = string.Format(" ( it.OwnerId = {0} )", currentUser.User_ID);
            }
            if ((enum2 == EntityPrivilegeEnum.Department) && !opm.CheckHasSharedPrivilege(objectId, es.EntityName, EntityOperationEnum.Query))
            {
                userIdByDeptId = opm.GetUserIdByDeptId(new int[] { currentUser.Department_ID.Value });
                if (userIdByDeptId.Count == 0)
                {
                    userIdByDeptId.Add(-2147483647);
                }
                str2 = string.Join<int>(" , ", userIdByDeptId);
                str2 = "{ " + str2 + " }";
                str = string.Format(" ( it.OwnerId in {0} )", str2);
            }
            if ((enum2 == EntityPrivilegeEnum.DepartmentAndSubSector) && !opm.CheckHasSharedPrivilege(objectId, es.EntityName, EntityOperationEnum.Query))
            {
                List<int> subDeptIds = opm.GetSubDeptIds(currentUser.Department_ID);
                subDeptIds.Add(currentUser.Department_ID.Value);
                userIdByDeptId = opm.GetUserIdByDeptId(subDeptIds.ToArray());
                if (userIdByDeptId.Count == 0)
                {
                    userIdByDeptId.Add(-2147483647);
                }
                str2 = string.Join<int>(" , ", userIdByDeptId);
                str2 = "{ " + str2 + " }";
                str = string.Format(" ( it.OwnerId in {0} )", str2);
            }
            return str;
        }

        public static RequireLevelEnum RequiredLevel(this IEntitySchema es)
        {
            return (RequireLevelEnum) es.RequiredLevel;
        }

        private static int TryGetEntityOwnerId(IEntitySchema es, object entity, string operationName)
        {
            object obj2 = entity.GetPropertyValue("OwnerId", null);
            if (obj2 == null)
            {
                throw new ApplicationException(string.Format("当前{1}的数据 {0} 的 OwnerId 不能为空", es.EntityName, operationName.Replace("权限", "")));
            }
            return Convert.ToInt32(obj2);
        }


    }
}
