namespace King.Framework.EntityLibrary
{
    using King.Framework.DAL;
    using System;
    using System.Runtime.CompilerServices;

    [Serializable, KingTable]
    public class T_Time_Plan
    {
        [KingColumn]
        public DateTime? CreateTime { get; set; }

        [KingColumn]
        public int? CreateUserId { get; set; }

        [KingColumn]
        public DateTime? CycleEndTime { get; set; }

        [KingColumn]
        public DateTime? CycleStartTime { get; set; }

        [KingColumn]
        public int? Day_Interval { get; set; }

        [KingColumn]
        public int? Day_Order { get; set; }

        [KingColumn]
        public int? DayCycleMode { get; set; }

        [KingColumn]
        public bool? Has_Friday { get; set; }

        [KingColumn]
        public bool? Has_Monday { get; set; }

        [KingColumn]
        public bool? Has_Saturday { get; set; }

        [KingColumn]
        public bool? Has_Sunday { get; set; }

        [KingColumn]
        public bool? Has_Thursday { get; set; }

        [KingColumn]
        public bool? Has_Tuesday { get; set; }

        [KingColumn]
        public bool? Has_Wednesday { get; set; }

        [KingColumn]
        public decimal? HoldTimeNum { get; set; }

        [KingColumn]
        public int? HoldTimeUnit { get; set; }

        [KingColumn]
        public decimal? IntervalTimeNum { get; set; }

        [KingColumn]
        public int? IntervalTimeUnit { get; set; }

        [KingColumn]
        public bool? IsCycleInDay { get; set; }

        [KingColumn]
        public bool? IsNeverOverTime { get; set; }

        [KingColumn]
        public int? Month_Interval { get; set; }

        [KingColumn]
        public int? Month_Order { get; set; }

        [KingColumn]
        public int? MonthCycleMode { get; set; }

        [KingColumn]
        public int? OwnerId { get; set; }

        [KingColumn]
        public int? PlanCycleMode { get; set; }

        [KingColumn]
        public int? PlanMode { get; set; }

        [KingColumn]
        public DateTime? RangeEndTime { get; set; }

        [KingColumn]
        public DateTime? RangeStartTime { get; set; }

        [KingColumn]
        public int? State { get; set; }

        [KingColumn]
        public int? StateDetail { get; set; }

        [KingColumn(IsPrimaryKey=true)]
        public int Time_Plan_Id { get; set; }

        [KingColumn(MaxLength=200)]
        public string Time_Plan_Name { get; set; }

        [KingColumn]
        public DateTime? UpdateTime { get; set; }

        [KingColumn]
        public int? UpdateUserId { get; set; }

        [KingColumn]
        public int? Week_Interval { get; set; }

        [KingColumn]
        public int? Week_Order { get; set; }

        [KingColumn]
        public int? WeekDay_Order { get; set; }

        [KingColumn]
        public int? YearCycleMode { get; set; }
    }
}

