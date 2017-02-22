namespace King.Framework.WorkflowEngineCore.MessageHandler
{
    using King.Framework.EntityLibrary;
    using King.Framework.Linq;
    using King.Framework.WorkflowEngineCore.Cache;
    using King.Framework.WorkflowEngineCore.InternalCore;
    using System;
    using System.Collections.Generic;

    internal abstract class WorkflowMessage
    {
        private ProcessEngine _engine;

        public WorkflowMessage(ProcessEngine engine)
        {
            if (engine == null)
            {
                throw new ArgumentNullException("engine");
            }
            if (engine.Process == null)
            {
                throw new ApplicationException("engine.Process==null");
            }
            if (engine.PI == null)
            {
                throw new ApplicationException("engine.PI==null");
            }
            this._engine = engine;
        }

        public abstract void Execute(Queue<WorkflowMessage> queue);

        protected DataContext Context
        {
            get
            {
                return this._engine.Context;
            }
        }

        protected ProcessEngine Engine
        {
            get
            {
                return this._engine;
            }
        }

        protected DynamicManager Manager
        {
            get
            {
                return this._engine.Manager;
            }
        }

        protected ProcessCacheFactory PCacheFactory
        {
            get
            {
                return this._engine.ProcessCache;
            }
        }

        protected SysProcessInstance PI
        {
            get
            {
                return this._engine.PI;
            }
        }

        protected ProcessInstanceCacheFactory PICacheFactory
        {
            get
            {
                return this._engine.ProcessInstanceCache;
            }
        }

        protected SysProcess Process
        {
            get
            {
                return this._engine.Process;
            }
        }
    }
}

