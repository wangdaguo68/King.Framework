<<<<<<< HEAD
﻿namespace King.Framework.WebTools
=======
﻿namespace Drision.Framework.WebTools
>>>>>>> 2b573943baeab4164300c7a14b5c182ddccdbe59
{
    using System;
    using System.Runtime.CompilerServices;

    public class Resp<T> : Resp
    {
        public Resp()
        {
            base.State = true;
        }

        public Resp(T data)
        {
            base.State = true;
            this.Data = data;
        }

        public T Data { get; set; }
    }
}

