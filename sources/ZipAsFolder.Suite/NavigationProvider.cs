using ZipAsFolder.IO;
using ZipAsFolder.Suite.Internal;

namespace ZipAsFolder.Suite;

public sealed class NavigationProvider
{
    private readonly IFileSystemProvider _provider;
    private readonly IContext _context;
    private readonly ICmdlet _cmdlet;

    internal NavigationProvider(
        IFileSystemProvider provider,
        IContext context,
        ICmdlet cmdlet)
    {
        _provider = provider;
        _context = context;
        _cmdlet = cmdlet;
    }

    public IFileSystemInfo? GetItem(string path)
    {
        var walker = CreateWalker(path);
        return walker.IsCompleted && walker.IsValid ? walker.Current : null;
    }

    public bool ItemExists(string path) => GetItem(path) != null;

    public bool HasChildItems(string path) => GetItem(path).AsDirectory(out var dir) && !dir.IsEmpty(_context);

    public bool IsItemContainer(string path) => GetItem(path).IsDirectory();

    public IFileSystemInfo TryDowngradeToFile(IFileSystemInfo target)
    {
        IFileSystemInfo? file = null;

        if (target.AsDirectory(out var directory))
        {
            file = _provider.AsFile(directory);
        }

        return file ?? target;
    }

    public void WriteChildItems(
        string path,
        uint depth,
        bool nameOnly,
        FileSystemInfoAttributes attributesFilter,
        ISearchFilter? directoryFilter,
        ISearchFilter? fileFilter)
    {
        if (depth == 0 || !GetItem(path).AsDirectory(out var directory))
        {
            return;
        }

        if (depth == 1)
        {
            foreach (var item in _provider.GetChildItems(directory, true, directoryFilter, fileFilter, _context))
            {
                if (_context.Token.IsCancellationRequested)
                {
                    break;
                }

                if (attributesFilter.ContainsFlag(item.Attributes))
                {
                    _cmdlet.WriteItem(item, nameOnly);
                }
            }

            return;
        }

        var queue = new List<IDirectoryInfo> { directory };

        while (depth != 0 && queue.Count > 0 && !_context.Token.IsCancellationRequested)
        {
            depth--;
            var containersCount = queue.Count;

            for (var i = 0; i < containersCount && !_context.Token.IsCancellationRequested; i++)
            {
                var items = _provider.GetChildItems(queue[i], true, null, null, _context);
                foreach (var item in items)
                {
                    bool isMatch;
                    if (item.AsDirectory(out var childDirectory))
                    {
                        isMatch = directoryFilter.IsNullOrMatch(item.Name);
                        queue.Add(childDirectory);
                    }
                    else
                    {
                        isMatch = fileFilter.IsNullOrMatch(item.Name);
                    }

                    if (isMatch && attributesFilter.ContainsFlag(item.Attributes))
                    {
                        _cmdlet.WriteItem(item, nameOnly);
                    }
                }
            }

            queue.RemoveRange(0, containersCount);
        }
    }

    public IFileContentReader CreateTextReader(IFileSystemInfo target, bool raw, Encoding? encoding)
    {
        target.AssertIsNotNull(nameof(target));

        if (!target.AsFile(out var file))
        {
            file = _provider.AsFile((IDirectoryInfo)target);
        }

        PathAssert.PathIsFile(file != null, target.FullName);

        return new TextContentReader(file, raw, encoding);
    }

    public IFileContentWriter CreateTextWriter(IFileSystemInfo target, bool noNewLine, Encoding? encoding)
    {
        target.AssertIsNotNull(nameof(target));

        if (!target.AsFile(out var file))
        {
            file = _provider.AsFile((IDirectoryInfo)target);
        }

        PathAssert.PathIsFile(file != null, target.FullName);

        return new TextContentWriter(file, noNewLine, encoding);
    }

    public void ClearContent(IFileSystemInfo target)
    {
        target.AssertIsNotNull(nameof(target));

        target.ClearContent(_context);
    }

    public IFileSystemInfo CreateFile(string path, bool allowToCreatePath, bool allowToOverride)
    {
        var walker = CreateWalker(path);

        PathAssert.PathExists(walker.IsValid, path);

        if (walker.IsCompleted)
        {
            PathAssert.PathIsFile(walker.Current.AsFile(out var file), path);
            PathAssert.FileDoesNoExist(allowToOverride, path);

            ClearContent(file);
            return file;
        }

        PathAssert.PathExists(walker.NextIsLast || allowToCreatePath, path);

        while (!walker.NextIsLast)
        {
            var folder = _provider.CreateDirectory((IDirectoryInfo)walker.Current!, walker.NextName, _context);
            _cmdlet.PerformNewItem(folder);
            walker.SetNext(folder);
        }

        var result = _provider.CreateFile((IDirectoryInfo)walker.Current!, walker.NextName, _context);
        return result;
    }

    public IDirectoryInfo CreateDirectory(string path, bool ignoreExisting)
    {
        var walker = CreateWalker(path);

        PathAssert.PathExists(walker.IsValid, path);

        if (walker.IsCompleted)
        {
            PathAssert.PathIsDirectory(walker.Current.AsDirectory(out var directory), path);
            PathAssert.DirectoryDoesNoExist(ignoreExisting, path);

            return directory;
        }

        while (!string.IsNullOrEmpty(walker.NextName))
        {
            var folder = _provider.CreateDirectory((IDirectoryInfo)walker.Current!, walker.NextName, _context);
            _cmdlet.PerformNewItem(folder);
            walker.SetNext(folder);
        }

        return (IDirectoryInfo)walker.Current!;
    }

    public void CopyFile(IFileInfo source, string destinationPath)
    {
        var result = CopyOrMoveFile(source, true, destinationPath, false);
        _cmdlet.WriteItem(result);
    }

    public void CopyDirectory(
        IDirectoryInfo source,
        string destinationPath,
        bool force,
        bool recurse,
        ISearchFilter? filter)
    {
        var walker = CreateWalker(destinationPath);

        PathAssert.PathExists(walker.IsValid, destinationPath);

        IDirectoryInfo destination;

        if (walker.IsCompleted)
        {
            PathAssert.PathIsDirectory(walker.Current.AsDirectory(out var directory), destinationPath);

            destination = _provider.CreateDirectory(directory, source.Name, _context);
            _cmdlet.PerformNewItem(destination);
        }
        else
        {
            PathAssert.PathExists(walker.NextIsLast, destinationPath);

            destination = _provider.CreateDirectory((IDirectoryInfo)walker.Current!, walker.NextName, _context);
            _cmdlet.PerformNewItem(destination);
        }

        _cmdlet.WriteItem(destination);
        if (recurse)
        {
            CopyDirectoryRecurse(source, destination, filter);
        }
    }

    public void MoveFile(IFileInfo source, bool force, string destinationPath)
    {
        var result = CopyOrMoveFile(source, force, destinationPath, true);
        _cmdlet.WriteItem(result);
    }

    public void MoveDirectory(IDirectoryInfo source, string destinationPath, bool force, ISearchFilter? filter)
    {
        var walker = CreateWalker(destinationPath);

        PathAssert.PathExists(walker.IsValid, destinationPath);

        IDirectoryInfo destination;

        if (walker.IsCompleted)
        {
            PathAssert.PathIsDirectory(walker.Current.AsDirectory(out var directory), destinationPath);

            destination = _provider.CreateDirectory(directory, source.Name, _context);
            _cmdlet.PerformNewItem(destination);
        }
        else
        {
            PathAssert.PathExists(walker.NextIsLast, destinationPath);

            destination = _provider.CreateDirectory((IDirectoryInfo)walker.Current!, walker.NextName, _context);
            _cmdlet.PerformNewItem(destination);
        }

        _cmdlet.WriteItem(destination);
        MoveDirectoryRecurse(source, destination, force, filter);
    }

    private void CopyDirectoryRecurse(
        IDirectoryInfo source,
        IDirectoryInfo destination,
        ISearchFilter? filter)
    {
        var queue = new List<(IDirectoryInfo Source, IDirectoryInfo Destination)> { (source, destination) };
        while (queue.Count > 0 && !_context.Token.IsCancellationRequested)
        {
            var containersCount = queue.Count;

            for (var i = 0; i < containersCount && !_context.Token.IsCancellationRequested; i++)
            {
                var next = queue[i];
                var children = _provider.GetChildItems(next.Source, true, null, filter, _context);

                foreach (var child in children)
                {
                    var newItem = CopyItem(child, next.Destination);

                    if (newItem.AsDirectory(out var destinationDirectory))
                    {
                        queue.Add(((IDirectoryInfo)child, destinationDirectory));
                    }

                    _cmdlet.WriteItem(newItem);
                }
            }

            queue.RemoveRange(0, containersCount);
        }
    }

    private IFileSystemInfo CopyItem(IFileSystemInfo source, IDirectoryInfo destination)
    {
        var existing = _provider.GetChild(destination, source.Name, _context);

        if (source.AsFile(out var sourceFile))
        {
            PathAssert.PathIsFile(existing == null || existing.IsFile(), existing?.FullName!);

            var destinationFile = sourceFile.CopyTo(destination, (IFileInfo?)existing, source.Name, _context);
            return destinationFile;
        }

        PathAssert.PathIsDirectory(existing == null || existing.IsDirectory(), existing?.FullName!);

        if (!existing.AsDirectory(out var destinationDirectory))
        {
            destinationDirectory = _provider.CreateDirectory(destination, source.Name, _context);
            _cmdlet.PerformNewItem(destinationDirectory);
        }

        return destinationDirectory;
    }

    private void MoveDirectoryRecurse(
        IDirectoryInfo source,
        IDirectoryInfo destination,
        bool force,
        ISearchFilter? filter)
    {
        var queue = new List<(IDirectoryInfo Source, IDirectoryInfo Destination)> { (source, destination) };
        while (queue.Count > 0 && !_context.Token.IsCancellationRequested)
        {
            var containersCount = queue.Count;

            for (var i = 0; i < containersCount && !_context.Token.IsCancellationRequested; i++)
            {
                var next = queue[i];
                var sourceChildren = _provider.GetChildItems(next.Source, false, null, filter, _context);

                foreach (var child in sourceChildren)
                {
                    var newItem = MoveItem(child, next.Destination, force);

                    if (newItem.AsDirectory(out var destinationDirectory))
                    {
                        queue.Add(((IDirectoryInfo)child, destinationDirectory));
                    }

                    _cmdlet.WriteItem(newItem);
                }
            }

            queue.RemoveRange(0, containersCount);
        }

        if (!_context.Token.IsCancellationRequested)
        {
            source.Delete(_context);
        }
    }

    private IFileSystemInfo MoveItem(IFileSystemInfo sourceChild, IDirectoryInfo destination, bool force)
    {
        var candidate = _provider.GetChild(destination, sourceChild.Name, _context);

        if (sourceChild.IsDirectory())
        {
            PathAssert.PathIsFile(candidate == null || candidate.IsFile(), candidate?.FullName!);

            if (!candidate.AsDirectory(out var destinationDirectory))
            {
                destinationDirectory = _provider.CreateDirectory(destination, sourceChild.Name, _context);
                _cmdlet.PerformNewItem(destinationDirectory);
            }

            return destinationDirectory;
        }

        if (candidate != null)
        {
            PathAssert.PathIsFile(candidate.IsFile(), candidate.FullName);
            PathAssert.FileDoesNoExist(force, candidate.FullName, true);
        }

        return ((IFileInfo)sourceChild).MoveTo(destination, (IFileInfo?)candidate, sourceChild.Name, _context);
    }

    private IPathWalker CreateWalker(string path)
    {
        var result = new PathWalker(_provider, _context, path);
        result.Initialize();
        return result;
    }

    private IFileSystemInfo CopyOrMoveFile(IFileInfo source, bool force, string destinationPath, bool move)
    {
        var walker = CreateWalker(destinationPath);

        PathAssert.PathExists(walker.IsValid, destinationPath);

        IDirectoryInfo destinationParent;
        string name;
        IFileInfo? destination = null;

        if (walker.IsCompleted)
        {
            name = source.Name;
            if (walker.Current.AsDirectory(out var directory))
            {
                // 1.zip => 2.zip
                // ignore 1.zip => 2.zip/
                if (!walker.LastIsDirectory && _provider.AsDirectory(source) != null)
                {
                    destination = _provider.AsFile(directory);
                }

                destinationParent = destination == null ? directory : destination.Parent!;
            }
            else
            {
                PathAssert.FileDoesNoExist(force, destinationPath, true);

                destination = (IFileInfo)walker.Current!;
                destinationParent = destination.Parent!;
            }
        }
        else
        {
            PathAssert.PathExists(walker.NextIsLast && !walker.LastIsDirectory, destinationPath);

            destinationParent = (IDirectoryInfo)walker.Current!;
            name = walker.NextName;
        }

        if (destination == null)
        {
            var test = destinationParent.GetChild(name, _context);
            PathAssert.PathIsFile(test == null || test.IsFile(), destinationPath);

            destination = (IFileInfo?)test;
        }

        var result = move ? source.MoveTo(destinationParent, destination, name, _context) : source.CopyTo(destinationParent, destination, name, _context);

        var resultDirectory = _provider.AsDirectory(result);
        if (resultDirectory != null)
        {
            return resultDirectory;
        }

        return result;
    }
}