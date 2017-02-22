namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [Serializable, KingTable]
    public class T_Entity_Trace_Plan
    {
        [KingColumn]
        public bool? AllowBeforehandExecute { get; set; }

        [KingColumn]
        public bool? AllowDelayedExecute { get; set; }

        [KingColumn]
        public decimal? BeforehandCreateTimeNum { get; set; }

        [KingColumn]
        public int? BeforehandCreateTimeUnit { get; set; }

        [KingColumn]
        public int? BeforehandExecuteTimeNum { get; set; }

        [KingColumn]
        public int? BeforehandExecuteTimeUnit { get; set; }

        [KingColumn]
        public DateTime? CreateTime { get; set; }

        public virtual T_User CreateUser { get; set; }

        [KingColumn]
        public int? CreateUserId { get; set; }

        public virtual T_Entity_Trace_Plan DdlEntityCondition { get; set; }

        [KingColumn]
        public int? DdlEntityConditionID { get; set; }

        public virtual T_Entity_Trace_Plan DdlEntityName { get; set; }

        [KingColumn]
        public int? DdlEntityNameID { get; set; }

        public virtual T_Entity_Trace_Plan DdlMoreRelation { get; set; }

        [KingColumn]
        public int? DdlMoreRelationID { get; set; }

        public virtual T_Entity_Trace_Plan DdlRelation { get; set; }

        [KingColumn]
        public int? DdlRelationID { get; set; }

        public virtual T_User DefaultOwner { get; set; }

        [KingColumn]
        public int? DefaultOwnerID { get; set; }

        public virtual T_User DefaultWorkItemOwner { get; set; }

        [KingColumn]
        public int? DefaultWorkItemOwnerID { get; set; }

        [KingColumn]
        public int? DelayedExecuteTimeNum { get; set; }

        [KingColumn]
        public int? DelayedExecuteTimeUnit { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public int Entity_Trace_Plan_Id { get; set; }

        [KingColumn(MaxLength=200)]
        public string Entity_Trace_Plan_Name { get; set; }

        [KingColumn]
        public long? EntityConditionId { get; set; }

        [KingColumn]
        public int? EntityFilterType { get; set; }

        [KingColumn]
        public int? EntityId { get; set; }

        [KingColumn(MaxLength=200)]
        public string EntityType { get; set; }

        [KingColumn]
        public int? ExecuteType { get; set; }

        [KingColumn(MaxLength=200)]
        public string FilterObjectId { get; set; }

        [KingColumn]
        public long? MoreRelationId { get; set; }

        public virtual T_User Owner { get; set; }

        [KingColumn]
        public int? OwnerId { get; set; }

        [KingColumn(MaxLength=200)]
        public string PluginAssemblyName { get; set; }

        [KingColumn(MaxLength=200)]
        public string PluginClassPath { get; set; }

        [KingColumn(MaxLength=200)]
        public string RegisteredServiceKey { get; set; }

        [KingColumn]
        public long? RelationId { get; set; }

        [KingColumn]
        public int? State { get; set; }

        [KingColumn]
        public int? StateDetail { get; set; }

        public virtual T_Time_Plan TimePlan { get; set; }

        [KingColumn]
        public int? TimePlanID { get; set; }

        [KingColumn(MaxLength=200)]
        public string Title { get; set; }

        [KingColumn]
        public DateTime? UpdateTime { get; set; }

        [KingColumn]
        public int? UpdateUserId { get; set; }

        public virtual T_WorkItemType WorkItemType { get; set; }

        [KingColumn]
        public int? WorkItemTypeID { get; set; }
    }
}

