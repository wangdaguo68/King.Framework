namespace King.Framework.Mvc
{
    using System;
    using System.Collections.Generic;

    public class List_DepId : IEqualityComparer<SelectControlData>
    {
        public bool Equals(SelectControlData x, SelectControlData y)
        {
            return (x.id == y.id);
        }

        public int GetHashCode(SelectControlData obj)
        {
            return 0;
        }
    }
}

