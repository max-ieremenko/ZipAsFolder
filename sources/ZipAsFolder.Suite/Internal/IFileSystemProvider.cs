using System.Collections.Generic;
using ZipAsFolder.IO;

namespace ZipAsFolder.Suite.Internal;

internal interface IFileSystemProvider
{
    IDirectoryInfo? GetRoot(string name);

    IFileSystemInfo? GetChild(IDirectoryInfo parent, string childName, IContext context);

    IDirectoryInfo? AsDirectory(IFileInfo file);

    IFileInfo? AsFile(IDirectoryInfo directory);

    IEnumerable<IFileSystemInfo> GetChildItems(
        IDirectoryInfo directory,
        bool archiveAsDirectory,
        ISearchFilter? directoryFilter,
        ISearchFilter? fileFilter,
        IContext context);

    IFileSystemInfo CreateFile(IDirectoryInfo parent, string name, IContext context);

    IDirectoryInfo CreateDirectory(IDirectoryInfo parent, string name, IContext context);
}