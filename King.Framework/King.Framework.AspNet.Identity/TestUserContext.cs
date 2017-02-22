namespace King.Framework.AspNet.Identity
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal static class TestUserContext
    {
        private static readonly List<SysUser> store = new List<SysUser>();

        public static void Create(SysUser user)
        {
            store.Add(user);
        }

        public static SysUser FindById(int userId)
        {
            return store.FirstOrDefault<SysUser>(p => (p.User_ID == userId));
        }

        public static SysUser FindByName(string userName)
        {
            return store.FirstOrDefault<SysUser>(p => (p.LoginName == userName));
        }
    }
}

