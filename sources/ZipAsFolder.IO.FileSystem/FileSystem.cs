using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace ZipAsFolder.IO.FileSystem;

public sealed class FileSystem : IFileSystem
{
    public bool TryGetRoot(string name, [NotNullWhen(true)] out IDirectoryInfo? root)
    {
        if (Directory.Exists(name))
        {
            var info = new DirectoryInfo(name).Root;
            root = new FileSystemDirectoryInfo(null, info);
            return true;
        }

        root = null;
        return false;
    }

    public IFileInfo? AsFile(IDirectoryInfo directory) => null;

    public IDirectoryInfo? AsDirectory(IFileInfo file)
    {
        return null;
    }

    public bool CanFileNameBeDirectory(string name) => false;
}