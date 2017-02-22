namespace King.Framework.UrlAuthorization.Owin
{
    using King.Framework.Common;
    using King.Framework.EntityLibrary;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class UserLoader
    {
        private static IList<T_Role> GetRoleList(BizDataContext ctx, int userId)
        {
            List<T_Role> list = new List<T_Role>();
            List<int> list2 = (from t in ctx.Where<T_User_Role>(t => t.User_Id == userId) select t.Role_Id).ToList<int>();
            foreach (int num in list2)
            {
                T_Role item = new T_Role {
                    Role_ID = num
                };
                list.Add(item);
            }
            return list;
        }

        public static T_User GetUser(string loginName)
        {
            T_User user = null;
            if (!string.IsNullOrWhiteSpace(loginName))
            {
                using (BizDataContext context = new BizDataContext(true))
                {
                    user = context.FirstOrDefault<T_User>(t => t.LoginName == loginName);
                    if (user != null)
                    {
                        user.T_Roles = GetRoleList(context, user.User_ID);
                    }
                }
            }
            return user;
        }
    }
}

