using System.Collections.Generic;

namespace ZipAsFolder.IO;

public interface IDirectoryInfo : IFileSystemInfo
{
    bool IsEmpty(IContext context);

    IEnumerable<IDirectoryInfo> EnumerateDirectories(ISearchFilter? filter, IContext context);

    IEnumerable<IFileInfo> EnumerateFiles(ISearchFilter? filter, IContext context);

    IFileSystemInfo? GetChild(string childName, IContext context);

    IFileInfo CreateFile(string name, IContext context);

    IDirectoryInfo CreateDirectory(string name, IContext context);

    void Delete(IContext context);

    IDirectoryInfo Rename(string newName, IContext context);
}