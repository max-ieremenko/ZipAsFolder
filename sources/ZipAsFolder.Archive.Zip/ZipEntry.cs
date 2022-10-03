using System;
using System.IO.Compression;
using System.Text;

namespace ZipAsFolder.Archive.Zip;

internal sealed class ZipEntry : IArchiveEntry
{
    private static readonly char[] Separator = { '/' };

    private readonly ZipArchiveEntry _entry;

    public ZipEntry(ZipArchiveEntry entry)
    {
        _entry = entry;
        Path = entry.FullName.Split(Separator, StringSplitOptions.RemoveEmptyEntries);

        // copy value when archive is open
        Length = entry.Length;
    }

    public string[] Path { get; }

    public DateTime? LastWriteTime => _entry.LastWriteTime.LocalDateTime;

    public bool IsFile => !_entry.FullName.EndsWith(Separator[0]);

    public long? Length { get; }

    public override string ToString() => _entry.FullName;

    internal static string CreateFullName(in Span<string> path, string? name, bool isDirectory)
    {
        var result = new StringBuilder();
        for (var i = 0; i < path.Length; i++)
        {
            if (i > 0)
            {
                result.Append(Separator[0]);
            }

            result.Append(path[i]);
        }

        if (name != null)
        {
            if (result.Length > 0)
            {
                result.Append(Separator[0]);
            }

            result.Append(name);
        }

        if (isDirectory)
        {
            result.Append(Separator[0]);
        }

        return result.ToString();
    }
}