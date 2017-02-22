namespace King.Framework.Mvc
{
    using System;
    using System.ComponentModel;
    using System.Text;

    public static class ValidateCommon
    {
        public static Type GetModelType(Type type)
        {
            Type underlyingType = type;
            if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                NullableConverter converter = new NullableConverter(type);
                underlyingType = converter.UnderlyingType;
            }
            return underlyingType;
        }

        public static string GetValidateAttr(ValidateOptions validateOptions)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(validateOptions.Required ? "required," : "");
            builder.Append(validateOptions.StringOnly ? "alpha," : "");
            builder.Append(validateOptions.StringOrNumber ? "alphanum," : "");
            builder.Append(validateOptions.Digit ? "digit," : "");
            builder.Append(validateOptions.Number ? "number," : "");
            builder.Append(validateOptions.Email ? "email," : "");
            builder.Append(validateOptions.Url ? "url," : "");
            builder.Append((validateOptions.Mininum > 0L) ? string.Format("min!{0}!,", validateOptions.Mininum) : "");
            builder.Append((validateOptions.Maxinum > 0L) ? string.Format("max!{0}!,", validateOptions.Maxinum) : "");
            builder.Append((validateOptions.MinLength > 0L) ? string.Format("minlength!{0}!,", validateOptions.MinLength) : "");
            builder.Append((validateOptions.MaxLength > 0L) ? string.Format("maxlength!{0}!,", validateOptions.MaxLength) : "");
            builder.Append(!string.IsNullOrEmpty(validateOptions.RangeLength) ? string.Format("rangelength!{0}!,", validateOptions.RangeLength) : "");
            builder.Append(!string.IsNullOrEmpty(validateOptions.NumberBetween) ? string.Format("between!{0}!,", validateOptions.NumberBetween) : "");
            builder.Append(!string.IsNullOrEmpty(validateOptions.EqualTo) ? string.Format("equalto!{0}!,", validateOptions.EqualTo) : "");
            builder.Append(!string.IsNullOrEmpty(validateOptions.NotEqualTo) ? string.Format("differs!{0}!,", validateOptions.NotEqualTo) : "");
            builder.Append((validateOptions.Length > 0L) ? string.Format("length!{0}!,", validateOptions.Length) : "");
            builder.Append(validateOptions.Mobile ? "mobile," : "");
            return builder.ToString().TrimEnd(new char[] { ',' });
        }
    }
}

