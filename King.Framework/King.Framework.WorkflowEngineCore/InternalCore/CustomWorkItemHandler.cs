namespace King.Framework.WorkflowEngineCore.InternalCore
{
    using King.Framework.Common;
    using King.Framework.EntityLibrary;
    using King.Framework.Linq;
    using King.Framework.WorkflowEngineCore;
    using System;
    using System.Configuration;

    internal class CustomWorkItemHandler : IWorkItemHandler
    {
        private DataContext _context;
        private IWorkItemHandler handler;

        public CustomWorkItemHandler(DataContext _ctx)
        {
            this._context = _ctx;
        }

        public void OnWorkItemCompleted(int wiBaseId, int wiId, string Id)
        {
            if (this.HasHandler)
            {
                this.Handler.OnWorkItemCompleted(wiBaseId, wiId, Id);
            }
        }

        public string OnWorkItemCreating(T_WorkItemBase wiBase, SysWorkItem wi)
        {
            if (this.HasHandler)
            {
                return this.Handler.OnWorkItemCreating(wiBase, wi);
            }
            return null;
        }

        public IWorkItemHandler Handler
        {
            get
            {
                if (this.handler == null)
                {
                    string defaultWorkItemInterface = ConfigurationManager.AppSettings[MetaConfig.WorkItemInterfacePath];
                    if (string.IsNullOrWhiteSpace(defaultWorkItemInterface))
                    {
                        defaultWorkItemInterface = MetaConfig.DefaultWorkItemInterface;
                    }
                    this.handler = CustomHandlerLoader.GetHandlerWithConfiguration<IWorkItemHandler>(defaultWorkItemInterface, new object[] { this._context });
                }
                return this.handler;
            }
        }

        public bool HasHandler
        {
            get
            {
                return (this.Handler != null);
            }
        }
    }
}

