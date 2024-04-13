using ZipAsFolder.Archive;

namespace ZipAsFolder.IO.Archive;

public sealed class ArchiveFileSystem : IFileSystem
{
    private readonly IArchiveContentProvider _archiveProvider;
    private readonly IList<string> _extensions;

    public ArchiveFileSystem(IArchiveContentProvider archiveProvider, IList<string> extensions)
    {
        archiveProvider.AssertIsNotNull(nameof(archiveProvider));
        extensions.AssertIsNotEmpty(nameof(extensions));

        _archiveProvider = archiveProvider;
        _extensions = extensions;
    }

    public bool TryGetRoot(string name, [NotNullWhen(true)] out IDirectoryInfo? root)
    {
        root = null;
        return false;
    }

    public IFileInfo? AsFile(IDirectoryInfo directory)
    {
        return (directory as ArchiveFile)?.AsFile();
    }

    public IDirectoryInfo? AsDirectory(IFileInfo file)
    {
        if (!DoesExtensionMatch(file.Name))
        {
            return null;
        }

        return new ArchiveFile(file, _archiveProvider);
    }

    public bool CanFileNameBeDirectory(string name) => DoesExtensionMatch(name);

    private bool DoesExtensionMatch(string fileName)
    {
        for (var i = 0; i < _extensions.Count; i++)
        {
            if (fileName.EndsWith(_extensions[i], StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}