namespace King.Framework.AspNet.Identity
{
    using King.Framework.Common;
    using Microsoft.AspNet.Identity;
    using System;
    using System.Threading.Tasks;

    public class UserStore : IUserPasswordStore<SysUser>, IUserStore<SysUser>, IDisposable
    {
        public Task CreateAsync(SysUser user)
        {
            using (BizDataContext context = new BizDataContext(true))
            {
                user.User_ID = context.GetNextIdentity_Int(false);
                context.Insert(user);
            }
            return Task.FromResult<int>(0);
        }

        public Task DeleteAsync(SysUser user)
        {
            throw new NotImplementedException("不支持删除用户");
        }

        public void Dispose()
        {
        }

        public Task<SysUser> FindByIdAsync(long userId)
        {
            SysUser user;
            using (BizDataContext context = new BizDataContext(true))
            {
                user = context.FindById<SysUser>(new object[] { userId });
            }
            return Task.FromResult<SysUser>(user);
        }

        public Task<SysUser> FindByIdAsync(string userId)
        {
            return this.FindByIdAsync(long.Parse(userId));
        }

        public Task<SysUser> FindByNameAsync(string userName)
        {
            SysUser user;
            if (string.IsNullOrWhiteSpace(userName))
            {
                user = null;
            }
            else
            {
                using (BizDataContext context = new BizDataContext(true))
                {
                    user = context.FirstOrDefault<SysUser>(p => p.LoginName == userName);
                }
            }
            return Task.FromResult<SysUser>(user);
        }

        public Task<string> GetPasswordHashAsync(SysUser user)
        {
            return Task.FromResult<string>(user.User_Password);
        }

        public Task<bool> HasPasswordAsync(SysUser user)
        {
            return Task.FromResult<bool>(!string.IsNullOrWhiteSpace(user.User_Password));
        }

        public Task SetPasswordHashAsync(SysUser user, string passwordHash)
        {
            user.User_Password = passwordHash;
            return Task.FromResult<int>(0);
        }

        public Task UpdateAsync(SysUser user)
        {
            using (BizDataContext context = new BizDataContext(true))
            {
                context.Update(user);
            }
            return Task.FromResult<int>(0);
        }
    }
}

