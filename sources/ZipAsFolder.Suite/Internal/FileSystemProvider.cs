using ZipAsFolder.IO;

namespace ZipAsFolder.Suite.Internal;

internal sealed class FileSystemProvider : IFileSystemProvider
{
    private readonly IFileSystem[] _systems;

    public FileSystemProvider(IFileSystem[] systems)
    {
        _systems = systems;
    }

    public IDirectoryInfo? GetRoot(string name)
    {
        IDirectoryInfo? result = null;

        for (var i = 0; i < _systems.Length; i++)
        {
            if (!_systems[i].TryGetRoot(name, out var next))
            {
                continue;
            }

            if (result != null)
            {
                throw new NotSupportedException("2 roots found: {0} and {1}.".FormatWith(result.GetType().FullName, next.GetType().FullName));
            }

            result = next;
        }

        return result;
    }

    public IFileSystemInfo? GetChild(IDirectoryInfo parent, string childName, IContext context)
    {
        var result = parent.GetChild(childName, context);

        if (result.AsFile(out var file) && TryGetAsDirectory(file, out var container))
        {
            result = container;
        }

        return result;
    }

    public IDirectoryInfo? AsDirectory(IFileInfo file)
    {
        for (var i = 0; i < _systems.Length; i++)
        {
            var result = _systems[i].AsDirectory(file);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }

    public IFileInfo? AsFile(IDirectoryInfo directory)
    {
        for (var i = 0; i < _systems.Length; i++)
        {
            var result = _systems[i].AsFile(directory);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }

    public IEnumerable<IFileSystemInfo> GetChildItems(
        IDirectoryInfo directory,
        bool archiveAsDirectory,
        ISearchFilter? directoryFilter,
        ISearchFilter? fileFilter,
        IContext context)
    {
        foreach (var child in directory.EnumerateDirectories(directoryFilter, context))
        {
            if (context.Token.IsCancellationRequested)
            {
                yield break;
            }

            yield return child;
        }

        foreach (var child in directory.EnumerateFiles(null, context))
        {
            if (context.Token.IsCancellationRequested)
            {
                yield break;
            }

            if (archiveAsDirectory && TryGetAsDirectory(child, out var container))
            {
                if (directoryFilter.IsNullOrMatch(child.Name))
                {
                    yield return container;
                }
            }
            else if (fileFilter.IsNullOrMatch(child.Name))
            {
                yield return child;
            }
        }
    }

    public IFileSystemInfo CreateFile(IDirectoryInfo parent, string name, IContext context)
    {
        IFileSystem? directorySystem = null;
        for (var i = 0; i < _systems.Length; i++)
        {
            var system = _systems[i];
            if (system.CanFileNameBeDirectory(name))
            {
                directorySystem = system;
                break;
            }
        }

        var file = parent.CreateFile(name, context);
        if (directorySystem == null)
        {
            return file;
        }

        var directory = directorySystem.AsDirectory(file);
        if (directory == null)
        {
            throw new InvalidOperationException("'{0}'.AsDirectory('{1}') returns null.".FormatWith(
                directorySystem.GetType().FullName,
                name));
        }

        directory.ClearContent(context);
        return directory;
    }

    public IDirectoryInfo CreateDirectory(IDirectoryInfo parent, string name, IContext context)
    {
        return parent.CreateDirectory(name, context);
    }

    private bool TryGetAsDirectory(IFileInfo file, [NotNullWhen(true)] out IDirectoryInfo? directory)
    {
        directory = null;

        for (var i = 0; i < _systems.Length; i++)
        {
            var next = _systems[i].AsDirectory(file);
            if (next == null)
            {
                continue;
            }

            if (directory != null)
            {
                throw new NotSupportedException("2 directories for path childFullName found: {0} and {1}.".FormatWith(next.GetType().FullName, directory.GetType().FullName));
            }

            directory = next;
        }

        return directory != null;
    }
}