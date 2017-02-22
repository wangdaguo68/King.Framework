namespace King.Framework.Manager
{
    using King.Framework.Common;
    using King.Framework.EntityLibrary;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Runtime.InteropServices;
    using System.Web.UI.WebControls;

    public class ScheduleManager
    {
        private BizDataContext context;

        public ScheduleManager(BizDataContext ctx)
        {
            this.context = ctx;
        }


        public void Complete(int id)
        {
            throw this.ThrowException();
        }

        public void Delete(int id)
        {
            throw this.ThrowException();
        }

        public SysWorkItem Query(int id)
        {
            throw this.ThrowException();
        }

        public List<SysWorkItem> Query(Expression<Func<SysWorkItem, bool>> expr = null)
        {
            throw this.ThrowException();
        }

        private ApplicationException ThrowException()
        {
            return new ApplicationException("手动添加的工作项，将统一管理，本方法停止使用");
        }
    }
}
