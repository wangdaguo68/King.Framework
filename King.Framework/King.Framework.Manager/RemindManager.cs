namespace King.Framework.Manager
{
    using King.Framework.Common;
    using King.Framework.EntityLibrary;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Runtime.InteropServices;

    public class RemindManager
    {
        private BizDataContext context;

        public RemindManager(BizDataContext _ctx)
        {
            this.context = _ctx;
        }

        public void Delete(int id)
        {
            SysRemind remind = this.context.FindById<SysRemind>(new object[] { id });
            if (remind == null)
            {
                throw new Exception("传入的主键有误，查询不到提醒信息！");
            }
            this.context.Delete(remind);
        }

        public SysRemind Query(int id)
        {
            return this.Query((Expression<Func<SysRemind, bool>>) (p => (p.RemindId == id))).FirstOrDefault<SysRemind>();
        }

        public List<SysRemind> Query(Expression<Func<SysRemind, bool>> Express = null)
        {
            if (Express == null)
            {
                Express = p => true;
            }
            return this.context.Where<SysRemind>(Express).ToList<SysRemind>();
        }

        public int Save(SysRemind entity)
        {
            entity.RemindId = this.context.GetNextIdentity_Int(false);
            if (string.IsNullOrEmpty(entity.RemindName))
            {
                throw new Exception("提醒必须设置主题！");
            }
            entity.CreateTime = new DateTime?(DateTime.Now);
            if (!(entity.OwnerId.HasValue && (entity.OwnerId.Value > 0)))
            {
                throw new Exception("提醒必须设置OwnerId！");
            }
            if (!(entity.State.HasValue && (entity.State.Value >= 0)))
            {
                entity.State = 0;
            }
            this.context.Insert(entity);
            return entity.RemindId;
        }

        public void Update(SysRemind entity)
        {
            SysRemind remind = this.context.FindById<SysRemind>(new object[] { entity.RemindId });
            if (string.IsNullOrEmpty(entity.RemindName))
            {
                throw new Exception("提醒必须设置主题！");
            }
            remind.RemindName = entity.RemindName;
            remind.DeadLine = entity.DeadLine;
            remind.RemindURL = entity.RemindURL;
            remind.State = entity.State;
            this.context.Update(remind);
        }
    }
}
