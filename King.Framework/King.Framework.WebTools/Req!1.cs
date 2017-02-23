<<<<<<< HEAD
﻿namespace King.Framework.WebTools
=======
﻿namespace Drision.Framework.WebTools
>>>>>>> 2b573943baeab4164300c7a14b5c182ddccdbe59
{
    using System;
    using System.Runtime.CompilerServices;

    public class Req<T> : Req
    {
        public Req()
        {
        }

        public Req(T input)
        {
            this.Input = input;
        }

        public T Input { get; set; }
    }
}

