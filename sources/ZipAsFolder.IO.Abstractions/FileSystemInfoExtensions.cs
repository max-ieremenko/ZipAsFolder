using System.Diagnostics.CodeAnalysis;

namespace ZipAsFolder.IO;

public static class FileSystemInfoExtensions
{
    public static bool IsDirectory([NotNullWhen(true)] this IFileSystemInfo? info) => info is IDirectoryInfo;

    public static bool IsFile([NotNullWhen(true)] this IFileSystemInfo? info) => info is IFileInfo;

    public static bool AsDirectory(this IFileSystemInfo? info, [NotNullWhen(true)] out IDirectoryInfo? directory)
    {
        return (directory = info as IDirectoryInfo) != null;
    }

    public static bool AsFile(this IFileSystemInfo? info, [NotNullWhen(true)] out IFileInfo? file)
    {
        return (file = info as IFileInfo) != null;
    }
}