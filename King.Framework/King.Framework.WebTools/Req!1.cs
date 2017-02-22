namespace Drision.Framework.WebTools
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

