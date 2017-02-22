namespace King.Framework.Manager
{
    using King.Framework.Common;
    using King.Framework.EntityLibrary;
    using System;
    using System.Collections.Generic;

    public class FunctionManager
    {
        private BizDataContext context;

        public FunctionManager(BizDataContext ctx)
        {
            this.context = ctx;
        }

        public List<SysFunction> GetAllChildsAndSelfByParentId(long parentId)
        {
            List<SysFunction> list = new List<SysFunction>();
            SysFunction item = this.context.FindById<SysFunction>(new object[] { parentId });
            if (item != null)
            {
                list.Add(item);
            }
            list.AddRange(this.GetAllChildsByParentId(parentId));
            return list;
        }

        public List<SysFunction> GetAllChildsByParentId(long parentId)
        {
            List<SysFunction> res = new List<SysFunction>();
            List<SysFunction> collection = this.context.Where<SysFunction>(p => p.Permission_Type == parentId);
            res.AddRange(collection);
            collection.ForEach(delegate (SysFunction p) {
                res.AddRange(this.GetChilds(p, res, this.context));
            });
            return res;
        }

        public List<SysFunction> GetAllMenu()
        {
            return this.context.Where<SysFunction>(p => p.Is_Menu == 1);
        }

        public List<SysFunction> GetAllNotMenu()
        {
            return this.context.Where<SysFunction>(p => p.Is_Menu == 0);
        }

        private List<SysFunction> GetChilds(SysFunction parent, List<SysFunction> res, BizDataContext context)
        {
            List<SysFunction> list = context.Where<SysFunction>(p => p.Permission_Type == parent.Function_ID);
            list.ForEach(delegate (SysFunction p) {
                res.AddRange(this.GetChilds(p, res, context));
            });
            return list;
        }

        public List<SysFunction> GetRoots()
        {
            return this.context.Where<SysFunction>(p => (p.Permission_Type == null) || (p.Permission_Type == 0L));
        }
    }
}
