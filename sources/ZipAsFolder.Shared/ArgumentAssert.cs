namespace ZipAsFolder;

public static class ArgumentAssert
{
    public static void AssertIsNotNull<T>([NotNull] this T? value, string name)
        where T : class
    {
        if (value == null)
        {
            throw new ArgumentNullException(name);
        }
    }

    public static void AssertIsNotNull([NotNull] this string? value, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentNullException(name);
        }
    }

    public static void AssertIsNotEmpty<T>([NotNull] this ICollection<T> collection, string name)
        where T : class
    {
        if (collection == null)
        {
            throw new ArgumentNullException(name);
        }

        if (collection.Count == 0)
        {
            throw new ArgumentOutOfRangeException(name, "Name cannot be empty.".FormatWith(name));
        }
    }
}