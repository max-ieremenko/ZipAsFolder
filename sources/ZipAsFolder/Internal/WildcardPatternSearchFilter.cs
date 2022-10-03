using System.Management.Automation;
using ZipAsFolder.IO;

namespace ZipAsFolder.Internal;

internal sealed class WildcardPatternSearchFilter : ISearchFilter
{
    private readonly WildcardPattern _pattern;

    public WildcardPatternSearchFilter(string filter)
    {
        filter.AssertIsNotNull(nameof(filter));

        Filter = filter;
        _pattern = WildcardPattern.Get(filter, WildcardOptions.IgnoreCase | WildcardOptions.Compiled);
    }

    public string Filter { get; }

    public static WildcardPatternSearchFilter? FromPattern(string? pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern))
        {
            return null;
        }

        return new WildcardPatternSearchFilter(pattern);
    }

    public bool IsMatch(string input) => _pattern.IsMatch(input);
}