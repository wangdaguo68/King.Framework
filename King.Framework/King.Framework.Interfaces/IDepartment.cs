namespace King.Framework.Interfaces
{
    using System;

    public interface IDepartment
    {
        int Department_ID { get; set; }

        string Department_Name { get; set; }

        int? Manager_ID { get; set; }

        int? Parent_ID { get; set; }
    }
}

