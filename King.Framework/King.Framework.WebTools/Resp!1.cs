
﻿namespace King.Framework.WebTools

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

