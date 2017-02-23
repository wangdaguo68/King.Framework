using System.Data.Common;

namespace King.Sys_Common.BLL
{
    using King.Framework.Common;
    using King.Framework.EntityLibrary;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Web;
    using System.Web.Caching;

    public static class Uitity
    {
        private static List<T_Configuration> configurationList;
        private static long dt = DateTime.Now.AddMinutes(10.0).Ticks;

        public static string FormatDateTime(DateTime dt)
        {
            return string.Format("{0:yyyy-MM-dd hh:mm}", dt);
        }

        private static T_Configuration GetConfiguration(string Key)
        {
            if ((configurationList == null) || (DateTime.Now.Ticks > dt))
            {
                using (BizDataContext context = new BizDataContext(true))
                {
                    Interlocked.Exchange<List<T_Configuration>>(ref configurationList, context.FetchAll<T_Configuration>());
                    Interlocked.Exchange(ref dt, DateTime.Now.AddSeconds(30.0).Ticks);
                }
            }
            T_Configuration configuration = configurationList.FirstOrDefault<T_Configuration>(p => p.Configuration_Key == Key);
            if (configuration == null)
            {
                throw new ApplicationException(string.Format("未找到{0}配置项", Key));
            }
            return configuration;
        }

        public static int GetMaxValue(string tableName, string fieldName, string sqlWhere, BizDataContext context)
        {
            int result = 1;
            string sql = string.Format("SELECT MAX({0}) maxValue FROM {1} WHERE 1=1 ", fieldName, tableName);
            if (!string.IsNullOrEmpty(sqlWhere))
            {
                sql = sql + sqlWhere;
            }
            object obj2 = context.ExecuteScalar(sql, new DbParameter[0]);
            if ((obj2 != null) && (obj2 != DBNull.Value))
            {
                if (!int.TryParse(obj2.ToString(), out result))
                {
                    return 1;
                }
                result++;
            }
            return result;
        }

        public static List<T> ICommonEntity<T>() where T: class, new()
        {
            List<T> list = null;
            using (BizDataContext context = new BizDataContext(true))
            {
                string key = typeof(T).ToString();
                if (HttpRuntime.Cache[key] == null)
                {
                    list = context.FetchAll<T>();
                    HttpRuntime.Cache.Add(key, list, null, DateTime.Now.AddSeconds(30.0), TimeSpan.Zero, CacheItemPriority.Normal, null);
                    return list;
                }
                return (List<T>) HttpRuntime.Cache[key];
            }
        }

        public static List<T> ToList<T>(this DataTable dt)
        {
            List<T> list = new List<T>();
            Type type = typeof(T);
            List<PropertyInfo> list2 = new List<PropertyInfo>(typeof(T).GetProperties());
            foreach (DataRow row in dt.Rows)
            {
                T local = Activator.CreateInstance<T>();
                Predicate<PropertyInfo> match = null;
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (match == null)
                    {
                        match = p => p.Name.ToLower() == dt.Columns[i].ColumnName.ToLower();
                    }
                    PropertyInfo info = list2.Find(match);
                    if ((info != null) && !Convert.IsDBNull(row[i]))
                    {
                        info.SetValue(local, row[i], null);
                    }
                }
                list.Add(local);
            }
            return list;
        }

        public static DataTable ToTable<T>(this List<T> list)
        {
            DataTable table = new DataTable();
            if (list.Count > 0)
            {
                T local = list[0];
                PropertyInfo[] properties = local.GetType().GetProperties();
                foreach (PropertyInfo info in properties)
                {
                    if (info.PropertyType.IsGenericType && (info.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                    {
                        Type type = info.PropertyType.GetGenericArguments()[0];
                        table.Columns.Add(info.Name, type);
                    }
                    else
                    {
                        table.Columns.Add(info.Name, info.PropertyType);
                    }
                }
                for (int i = 0; i < list.Count; i++)
                {
                    ArrayList list2 = new ArrayList();
                    foreach (PropertyInfo info in properties)
                    {
                        object obj2 = info.GetValue(list[i], null);
                        list2.Add(obj2);
                    }
                    object[] values = list2.ToArray();
                    table.LoadDataRow(values, true);
                }
            }
            return table;
        }

        public static string AddAddressCheck
        {
            get
            {
                return GetConfiguration("AddAddressCheck").Configuration_Value.ToString();
            }
        }

        public static string AllCaseList
        {
            get
            {
                return GetConfiguration("AllCaseList").Configuration_Value.ToString();
            }
        }

        public static string BaiDuAk
        {
            get
            {
                return GetConfiguration("BaiDuAk").Configuration_Value.ToString();
            }
        }

        public static string BaiduAPIAK
        {
            get
            {
                return GetConfiguration("BaiDuWebAK").Configuration_Value;
            }
        }

        public static string BaiduConvertToAddress
        {
            get
            {
                return GetConfiguration("BaiduConvertToAddress").Configuration_Value;
            }
        }

        public static string BaiduGPSReduceDeviationHTTPAddress
        {
            get
            {
                return GetConfiguration("BaiduGPSReduceDeviationHTTPAddress").Configuration_Value;
            }
        }

        public static string Captains
        {
            get
            {
                return GetConfiguration("Captains").Configuration_Value.ToString();
            }
        }

        public static int CornetType
        {
            get
            {
                return GetConfiguration("CornetType").Configuration_Value.ToInt();
            }
        }

        public static int DefaultPhoneType
        {
            get
            {
                return GetConfiguration("DefaultPhoneType").Configuration_Value.ToInt();
            }
        }

        public static string DefaultPwd
        {
            get
            {
                return GetConfiguration("DefaultPwd").Configuration_Value;
            }
        }

        public static int DepManager
        {
            get
            {
                return GetConfiguration("DepManager").Configuration_Value.ToInt();
            }
        }

        public static string DisCaseRole
        {
            get
            {
                return GetConfiguration("DisCaseRole").Configuration_Value.ToString();
            }
        }

        public static int EmailType
        {
            get
            {
                return GetConfiguration("EmailType").Configuration_Value.ToInt();
            }
        }

        public static int FaxType
        {
            get
            {
                return GetConfiguration("FaxType").Configuration_Value.ToInt();
            }
        }

        public static int HomePhoneType
        {
            get
            {
                return GetConfiguration("HomePhoneType").Configuration_Value.ToInt();
            }
        }

        public static int IsOpenLocation
        {
            get
            {
                return GetConfiguration("IsOpenLocation").Configuration_Value.ToInt();
            }
        }

        public static int LocationType
        {
            get
            {
                return GetConfiguration("LocationType").Configuration_Value.ToInt();
            }
        }

        public static int MapType
        {
            get
            {
                return GetConfiguration("MapType").Configuration_Value.ToInt();
            }
        }

        public static string MetrialType
        {
            get
            {
                return GetConfiguration("MetrialType").Configuration_Value.ToString();
            }
        }

        public static int MicrobloggingType
        {
            get
            {
                return GetConfiguration("MicrobloggingType").Configuration_Value.ToInt();
            }
        }

        public static int MicroChannelType
        {
            get
            {
                return GetConfiguration("MicroChannelType").Configuration_Value.ToInt();
            }
        }

        public static int MobileType
        {
            get
            {
                return GetConfiguration("MobileType").Configuration_Value.ToInt();
            }
        }

        public static int MonthCount
        {
            get
            {
                return GetConfiguration("MonthCount").Configuration_Value.ToInt();
            }
        }

        public static string NeedControl
        {
            get
            {
                return GetConfiguration("NeedControl").Configuration_Value.ToString();
            }
        }

        public static int NormalRole
        {
            get
            {
                return GetConfiguration("NormalRole").Configuration_Value.ToInt();
            }
        }

        public static int OfficePhoneType
        {
            get
            {
                return GetConfiguration("OfficePhoneType").Configuration_Value.ToInt();
            }
        }

        public static int ParAddressType
        {
            get
            {
                return GetConfiguration("ParAddressType").Configuration_Value.ToInt();
            }
        }

        public static string Patronn
        {
            get
            {
                return GetConfiguration("Patronn").Configuration_Value.ToString();
            }
        }

        public static int PostCodeType
        {
            get
            {
                return GetConfiguration("PostCodeType").Configuration_Value.ToInt();
            }
        }

        public static int QQType
        {
            get
            {
                return GetConfiguration("QQType").Configuration_Value.ToInt();
            }
        }

        public static string QueryAllProof
        {
            get
            {
                return GetConfiguration("QueryAllProof").Configuration_Value.ToString();
            }
        }

        public static string RecFile
        {
            get
            {
                return GetConfiguration("RecFile").Configuration_Value.ToString();
            }
        }

        public static string Register
        {
            get
            {
                return GetConfiguration("Register").Configuration_Value.ToString();
            }
        }

        public static int RoomBooking_Booking
        {
            get
            {
                return GetConfiguration("RoomBooking_Booking").Configuration_Value.ToInt();
            }
        }

        public static int RoomBooking_Check
        {
            get
            {
                return GetConfiguration("RoomBooking_Check").Configuration_Value.ToInt();
            }
        }

        public static string RoomBooking_ConnectionString
        {
            get
            {
                return GetConfiguration("RoomBooking_ConnectionString").Configuration_Value.ToString();
            }
        }

        public static string Sponsor
        {
            get
            {
                return GetConfiguration("Sponsor").Configuration_Value.ToString();
            }
        }

        public static int SystemManger
        {
            get
            {
                return GetConfiguration("SystemManger").Configuration_Value.ToInt();
            }
        }

        public static string UserPasswordKey
        {
            get
            {
                return GetConfiguration("UserPasswordKey").Configuration_Value.ToString();
            }
        }

        public static int WebSiteAddressType
        {
            get
            {
                return GetConfiguration("WebSiteAddressType").Configuration_Value.ToInt();
            }
        }
    }
}

