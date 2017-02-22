namespace King.Framework.Mvc
{
    using System;
    using System.Runtime.CompilerServices;

    public class Validator
    {
        public Validator()
        {
            this.isValidator = true;
            this.validateGroup = string.Empty;
        }

        public bool isValidator { get; set; }

        public string validateGroup { get; set; }
    }
}

