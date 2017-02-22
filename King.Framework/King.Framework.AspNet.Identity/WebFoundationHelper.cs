namespace King.Framework.AspNet.Identity
{
    using King.Framework.Common;
    using King.Framework.Linq;
    using System;
    using System.Linq;

    internal static class WebFoundationHelper
    {
        internal static readonly string WebFoundationConnectionStringName = "WebFoundation";

        internal static ConnectionInfo GetConnectionInfoFromWebFoundation(int companyId)
        {
            using (BizDataContext context = new BizDataContext("WebFoundation", true))
            {
                T_Company_AppDb db = (from p in context.Set<T_Company_AppDb>()
                    where p.CompanyId == companyId
                    select p).FirstOrDefault<T_Company_AppDb>();
                if (db == null)
                {
                    throw new ApplicationException("未指定企业的数据库连接:id=" + companyId.ToString());
                }
                return new ConnectionInfo(db.DbConnString, db.DbPrivider);
            }
        }
    }
}

