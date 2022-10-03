using System;
using System.Runtime.CompilerServices;

namespace ZipAsFolder;

public static class EnumExtensions
{
    public static bool ContainsFlag<T>(this T value, T flag)
        where T : Enum
    {
        var v = Unsafe.As<T, int>(ref value);
        var f = Unsafe.As<T, int>(ref flag);

        return (v & f) != 0;
    }
}