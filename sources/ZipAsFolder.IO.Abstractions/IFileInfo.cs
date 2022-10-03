using System.IO;

namespace ZipAsFolder.IO;

public interface IFileInfo : IFileSystemInfo
{
    string Extension { get; }

    Stream OpenRead();

    Stream OpenWrite();

    void Delete();

    IFileInfo CopyTo(IDirectoryInfo destinationParent, IFileInfo? destination, string name, IContext context);

    IFileInfo MoveTo(IDirectoryInfo destinationParent, IFileInfo? destination, string name, IContext context);

    IFileInfo Rename(string newName, IContext context);
}