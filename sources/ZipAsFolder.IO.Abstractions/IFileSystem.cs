namespace ZipAsFolder.IO;

public interface IFileSystem
{
    bool TryGetRoot(string name, [NotNullWhen(true)] out IDirectoryInfo? root);

    IFileInfo? AsFile(IDirectoryInfo directory);

    IDirectoryInfo? AsDirectory(IFileInfo file);

    bool CanFileNameBeDirectory(string name);
}