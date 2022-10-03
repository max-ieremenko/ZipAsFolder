namespace ZipAsFolder.IO;

public static class SearchFilterExtensions
{
    public static bool IsNullOrMatch(this ISearchFilter? filter, string input)
    {
        return filter == null || filter.IsMatch(input);
    }
}