namespace CascadeStudio
{
    using System.Globalization;
    using System.Text;

    public static class StringBuilderExt
    {
        public static StringBuilder AppendIfNotNull(this StringBuilder builder, double? value, string format)
        {
            if (value != null)
            {
                builder.AppendFormat(CultureInfo.InvariantCulture, format, value.Value);
            }

            return builder;
        }

        public static StringBuilder AppendIfNotNull(this StringBuilder builder, int? value, string format)
        {
            if (value != null)
            {
                builder.AppendFormat(CultureInfo.InvariantCulture, format, value.Value);
            }

            return builder;
        }
    }
}