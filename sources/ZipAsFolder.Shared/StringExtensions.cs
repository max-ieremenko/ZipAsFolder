using System;

namespace ZipAsFolder;

public static class StringExtensions
{
    public static string FormatWith(this string format, params object[] args)
    {
        return string.Format(format, args);
    }

    public static bool EqualsIgnoreCase(this string? text, string? other)
    {
        if (text == null)
        {
            return other == null;
        }

        return text.Equals(other, StringComparison.OrdinalIgnoreCase);
    }
}