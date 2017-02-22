namespace King.Framework.Mvc
{
    using System;
    using System.Collections.Generic;

    public interface ICustomSelectUser
    {
        List<NameValues> GetCustomData(object parameter);
    }
}

