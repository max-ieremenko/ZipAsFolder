using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ZipAsFolder.IO;

public sealed class FileSystemPath : IPath
{
    public static readonly char[] Separators = { '/', '\\' };

    public FileSystemPath(string itemSeparator)
    {
        itemSeparator.AssertIsNotNull(nameof(itemSeparator));

        ItemSeparator = itemSeparator;
    }

    public FileSystemPath()
        : this(Path.DirectorySeparatorChar.ToString())
    {
    }

    public string ItemSeparator { get; }

    public static string ToPsDrivePath(string path)
    {
        if (path.StartsWith(Names.PsDrivePath, StringComparison.OrdinalIgnoreCase))
        {
            return path;
        }

        return Names.PsDrivePath + path;
    }

    public static string FromPsDrivePath(string path)
    {
        if (path.StartsWith(Names.PsDrivePath, StringComparison.OrdinalIgnoreCase))
        {
            return path.Substring(Names.PsDrivePath.Length);
        }

        return path;
    }

    public bool IsPathRooted(string path)
    {
        var normalized = FromPsDrivePath(NormalizePath(path));
        if (string.IsNullOrEmpty(normalized))
        {
            return false;
        }

        return Path.IsPathFullyQualified(normalized);
    }

    public string NormalizePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return path;
        }

        for (var i = 0; i < path.Length; i++)
        {
            if (IsSeparator(path[i]) && path[i] != ItemSeparator[0])
            {
                path = path.Replace(path[i], ItemSeparator[0]);
            }

            if (IsSeparator(path[i]) && i < path.Length - 1 && IsSeparator(path[i + 1]))
            {
                path = path.Remove(i + 1, 1);

                i--;
            }
        }

        return path;
    }

    public string Combine(string? normalizedPath1, string normalizedPath2)
    {
        if (string.IsNullOrEmpty(normalizedPath1))
        {
            return normalizedPath2;
        }

        if (string.IsNullOrEmpty(normalizedPath2))
        {
            return normalizedPath1;
        }

        var result = FromPsDrivePath(normalizedPath1);
        var path2 = FromPsDrivePath(normalizedPath2);

        if (result.EndsWith(ItemSeparator))
        {
            if (path2.StartsWith(ItemSeparator))
            {
                path2 = path2.Substring(ItemSeparator.Length);
            }

            result += path2;
        }
        else if (path2.StartsWith(ItemSeparator))
        {
            result += path2;
        }
        else
        {
            result += ItemSeparator + path2;
        }

        if (IsPsDrive(normalizedPath1) || IsPsDrive(normalizedPath2))
        {
            result = ToPsDrivePath(result);
        }

        return result;
    }

    public bool IsValidName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return false;
        }

        for (var i = 0; i < name.Length; i++)
        {
            if (IsInvalidNameChar(name[i]))
            {
                return false;
            }
        }

        return true;
    }

    public string GetParentPath(string path)
    {
        var normalizedPath = NormalizePath(path);

        var result = FromPsDrivePath(normalizedPath);
        if (string.IsNullOrEmpty(result))
        {
            return string.Empty;
        }

        var index = result.LastIndexOf(ItemSeparator, StringComparison.OrdinalIgnoreCase);
        if (index == result.Length - 1)
        {
            result = result.Substring(0, index);
        }

        index = result.LastIndexOf(ItemSeparator, StringComparison.OrdinalIgnoreCase);
        if (index == 0)
        {
            if (result.Length == ItemSeparator.Length)
            {
                result = string.Empty;
            }
            else
            {
                result = ItemSeparator;
            }
        }
        else if (index > 0)
        {
            result = result.Substring(0, index);
        }
        else
        {
            result = string.Empty;
        }

        if (result.Length > 0 && result.EndsWith(':'))
        {
            result += ItemSeparator;
        }

        if (result.Length > 0 && IsPsDrive(normalizedPath))
        {
            result = ToPsDrivePath(result);
        }

        return result;
    }

    // see FileSystemProvider:
    //  *.txt => *.txt
    //  11/*.txt => *.txt
    public string GetChildName(string normalizedPath)
    {
        if (string.IsNullOrEmpty(normalizedPath))
        {
            return string.Empty;
        }

        var result = FromPsDrivePath(normalizedPath);

        var index = result.LastIndexOf(ItemSeparator, StringComparison.OrdinalIgnoreCase);
        if (index < 0)
        {
            return result;
        }

        if (index == result.Length - 1)
        {
            return string.Empty;
        }

        return result.Substring(index + 1);
    }

    private static bool IsSeparator(char text)
    {
        return text == Separators[0] || text == Separators[1];
    }

    private static bool IsPsDrive(in ReadOnlySpan<char> path)
    {
        return path.StartsWith(Names.PsDrivePath, StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsInvalidNameChar(char c)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var chars = Path.GetInvalidFileNameChars();
            for (var i = 0; i < chars.Length; i++)
            {
                if (c == chars[i])
                {
                    return true;
                }
            }

            return false;
        }

        return c == '"' || c == '<' || c == '>' || c == '|' || c == '\u0000' || c == '\u0001' || c == '\u0002' || c == '\u0003' || c == '\u0004' || c == '\u0005' || c == '\u0006' || c == '\u0007' || c == '\u0008' || c == '\u0009' || c == '\u000A' || c == '\u000B' || c == '\u000C' || c == '\u000D' || c == '\u000E' || c == '\u000F' || c == '\u0010' || c == '\u0011' || c == '\u0012' || c == '\u0013' || c == '\u0014' || c == '\u0015' || c == '\u0016' || c == '\u0017' || c == '\u0018' || c == '\u0019' || c == '\u001A' || c == '\u001B' || c == '\u001C' || c == '\u001D' || c == '\u001E' || c == '\u001F' || c == ':' || c == '*' || c == '?' || c == '\\' || c == '/';
    }
}