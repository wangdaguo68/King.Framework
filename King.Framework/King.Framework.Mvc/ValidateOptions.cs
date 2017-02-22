namespace King.Framework.Mvc
{
    using System;
    using System.Runtime.CompilerServices;

    public class ValidateOptions
    {
        public bool Digit { get; set; }

        public bool Email { get; set; }

        public string EqualTo { get; set; }

        public string ErrorMsg { get; set; }

        public long Length { get; set; }

        public long Maxinum { get; set; }

        public long MaxLength { get; set; }

        public long Mininum { get; set; }

        public long MinLength { get; set; }

        public bool Mobile { get; set; }

        public string NotEqualTo { get; set; }

        public bool Number { get; set; }

        public string NumberBetween { get; set; }

        public string RangeLength { get; set; }

        public bool Required { get; set; }

        public bool StringOnly { get; set; }

        public bool StringOrNumber { get; set; }

        public bool Url { get; set; }
    }
}

