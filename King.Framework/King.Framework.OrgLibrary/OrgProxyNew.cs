using System.Data.Common;

namespace King.Framework.OrgLibrary
{
    using King.Framework.Common;
    using King.Framework.EntityLibrary;
    using King.Framework.Interfaces;
    using King.Framework.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class OrgProxyNew : IOrgProxy, IDisposable
    {
        private readonly DataContext _context;
        private readonly bool _isSelfCreated;

        public OrgProxyNew()
        {
            this._isSelfCreated = true;
        }

        public OrgProxyNew(DataContext ctx)
        {
            if (ctx == null)
            {
                this._isSelfCreated = true;
            }
            else
            {
                this._context = ctx;
                this._isSelfCreated = false;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if ((disposing && this._isSelfCreated) && (this._context != null))
            {
                this._context.Dispose();
            }
        }

        ~OrgProxyNew()
        {
            this.Dispose(false);
        }

        public virtual List<IDepartment> GetAllChildDepartment(int departmentId)
        {
            List<IDepartment> list2;
            DataContext ctx = this.GetContext();
            try
            {
                string condition = string.Format("SystemLevelCode like '{0}-%' and SystemLevelCode <> '{0}-'", departmentId);
                list2 = ctx.Where<T_Department>(condition, new DbParameter[0]).Cast<IDepartment>().ToList<IDepartment>();
            }
            catch (Exception exception)
            {
                AppLogHelper.Error(exception);
                throw exception;
            }
            finally
            {
                this.ReleaseContext(ctx);
            }
            return list2;
        }

        public virtual List<IDepartment> GetChildDepartments(int departmentId)
        {
            List<IDepartment> list2;
            DataContext ctx = this.GetContext();
            try
            {
                string condition = string.Format("Parent_ID = {0}", departmentId);
                list2 = ctx.Where<T_Department>(condition, new DbParameter[0]).Cast<IDepartment>().ToList<IDepartment>();
            }
            catch (Exception exception)
            {
                AppLogHelper.Error(exception);
                throw exception;
            }
            finally
            {
                this.ReleaseContext(ctx);
            }
            return list2;
        }

        private DataContext GetContext()
        {
            if (this._isSelfCreated)
            {
                return new BizDataContext(true);
            }
            return this._context;
        }

        public virtual IDepartment GetDepartmentById(int departmentId)
        {
            IDepartment dept;
            try
            {
                dept = this.GetDept(departmentId);
            }
            catch (Exception exception)
            {
                AppLogHelper.Error(exception);
                throw exception;
            }
            return dept;
        }

        public virtual int GetDepartmentLevelCode(int departmentId)
        {
            return 0;
        }

        public virtual IUser GetDepartmentManager(int departmentId)
        {
            IUser user2;
            try
            {
                T_Department dept = this.GetDept(departmentId);
                if ((dept != null) && dept.Manager_ID.HasValue)
                {
                    return this.GetUser(dept.Manager_ID.Value);
                }
                user2 = null;
            }
            catch (Exception exception)
            {
                AppLogHelper.Error(exception);
                throw exception;
            }
            return user2;
        }

        public virtual List<IUser> GetDepartmentRoleUsers(int departmentId, int roleId)
        {
            List<IUser> list;
            DataContext ctx = this.GetContext();
            try
            {
                string sql = string.Format("select a.* from T_User a left join T_User_Role b on a.User_ID = b.User_ID where b.Role_ID = {0} and a.Department_ID = {1}", roleId, departmentId);
                list = ctx.ExecuteObjectSet<T_User>(sql, new DbParameter[0]).Cast<IUser>().ToList<IUser>();
            }
            catch (Exception exception)
            {
                AppLogHelper.Error(exception);
                throw exception;
            }
            finally
            {
                this.ReleaseContext(ctx);
            }
            return list;
        }

        public virtual List<IDepartment> GetDepartments()
        {
            List<IDepartment> list2;
            DataContext ctx = this.GetContext();
            try
            {
                list2 = ctx.FetchAll<T_Department>().Cast<IDepartment>().ToList<IDepartment>();
            }
            catch (Exception exception)
            {
                AppLogHelper.Error(exception);
                throw exception;
            }
            finally
            {
                this.ReleaseContext(ctx);
            }
            return list2;
        }

        public virtual List<IUser> GetDepartmentUsers(int departmentId)
        {
            List<IUser> list2;
            DataContext ctx = this.GetContext();
            try
            {
                string condition = string.Format("Department_ID = {0}", departmentId);
                list2 = ctx.Where<T_User>(condition, new DbParameter[0]).Cast<IUser>().ToList<IUser>();
            }
            catch (Exception exception)
            {
                AppLogHelper.Error(exception);
                throw exception;
            }
            finally
            {
                this.ReleaseContext(ctx);
            }
            return list2;
        }

        private T_Department GetDept(int departmentId)
        {
            T_Department department;
            DataContext ctx = this.GetContext();
            try
            {
                department = ctx.FindById<T_Department>(new object[] { departmentId });
            }
            finally
            {
                this.ReleaseContext(ctx);
            }
            return department;
        }

        public virtual IDepartment GetParentDepartment(int departmentId)
        {
            IDepartment departmentById = this.GetDepartmentById(departmentId);
            if (departmentById.Parent_ID.HasValue)
            {
                return this.GetDepartmentById(departmentById.Parent_ID.Value);
            }
            return departmentById;
        }

        private T_Role GetRole(int roleId)
        {
            T_Role role;
            DataContext ctx = this.GetContext();
            try
            {
                role = this._context.FindById<T_Role>(new object[] { roleId });
            }
            finally
            {
                this.ReleaseContext(ctx);
            }
            return role;
        }

        public virtual List<IRole> GetRoles()
        {
            List<IRole> list2;
            DataContext ctx = this.GetContext();
            try
            {
                list2 = ctx.Set<T_Role>().Cast<IRole>().ToList<IRole>();
            }
            catch (Exception exception)
            {
                AppLogHelper.Error(exception);
                throw exception;
            }
            finally
            {
                this.ReleaseContext(ctx);
            }
            return list2;
        }

        public virtual List<IUser> GetRoleUsers(int roleId)
        {
            List<IUser> list;
            DataContext ctx = this.GetContext();
            try
            {
                string sql = string.Format("select a.* from T_User a left join T_User_Role b on a.User_ID = b.User_ID where b.Role_ID = {0}", roleId);
                list = ctx.ExecuteObjectSet<T_User>(sql, new DbParameter[0]).Cast<IUser>().ToList<IUser>();
            }
            catch (Exception exception)
            {
                AppLogHelper.Error(exception);
                throw exception;
            }
            finally
            {
                this.ReleaseContext(ctx);
            }
            return list;
        }

        private T_User GetUser(int userId)
        {
            T_User user;
            DataContext ctx = this.GetContext();
            try
            {
                user = ctx.FindById<T_User>(new object[] { userId });
            }
            finally
            {
                this.ReleaseContext(ctx);
            }
            return user;
        }

        public virtual IUser GetUserById(int userId)
        {
            IUser user;
            try
            {
                user = this.GetUser(userId);
            }
            catch (Exception exception)
            {
                AppLogHelper.Error(exception);
                throw exception;
            }
            return user;
        }

        public virtual List<IRole> GetUserRoles(int userId)
        {
            List<IRole> list;
            DataContext ctx = this.GetContext();
            try
            {
                string sql = string.Format("select a.* from T_Role a left join T_User_Role b on a.Role_ID = b.Role_ID where b.User_ID = {0}", userId);
                list = ctx.ExecuteObjectSet<T_Role>(sql, new DbParameter[0]).Cast<IRole>().ToList<IRole>();
            }
            catch (Exception exception)
            {
                AppLogHelper.Error(exception);
                throw exception;
            }
            finally
            {
                this.ReleaseContext(ctx);
            }
            return list;
        }

        public virtual List<IUser> GetUsers()
        {
            List<IUser> list2;
            DataContext ctx = this.GetContext();
            try
            {
                list2 = ctx.FetchAll<T_User>().Cast<IUser>().ToList<IUser>();
            }
            catch (Exception exception)
            {
                AppLogHelper.Error(exception);
                throw exception;
            }
            finally
            {
                this.ReleaseContext(ctx);
            }
            return list2;
        }

        public virtual bool IsRootDepartment(int departmentId)
        {
            bool flag2;
            try
            {
                bool flag = false;
                T_Department dept = this.GetDept(departmentId);
                if (!(dept.Parent_ID.HasValue && (dept.Parent_ID.Value != 0)))
                {
                    flag = true;
                }
                flag2 = flag;
            }
            catch (Exception exception)
            {
                AppLogHelper.Error(exception);
                throw exception;
            }
            return flag2;
        }

        public virtual bool IsSupportDepartmentLevel()
        {
            return false;
        }

        private void ReleaseContext(DataContext ctx)
        {
            if (this._isSelfCreated)
            {
                ctx.Dispose();
            }
        }
    }
}

