namespace ZipAsFolder.IO.FileSystem;

internal sealed class FileSystemDirectoryInfo : DirectoryInfoBase
{
    public FileSystemDirectoryInfo(IDirectoryInfo? parent, DirectoryInfo info)
        : base(parent, info.Name, info.FullName, null, info.LastWriteTime)
    {
        NativeFullName = info.FullName;
    }

    internal string NativeFullName { get; }

    public override bool IsEmpty(IContext context)
    {
        return !Directory.EnumerateFileSystemEntries(NativeFullName).Any();
    }

    public override IEnumerable<IDirectoryInfo> EnumerateDirectories(ISearchFilter? filter, IContext context)
    {
        foreach (DirectoryInfo dir in Enumerate(true, filter))
        {
            yield return new FileSystemDirectoryInfo(this, dir);
        }
    }

    public override IEnumerable<IFileInfo> EnumerateFiles(ISearchFilter? filter, IContext context)
    {
        foreach (FileInfo file in Enumerate(false, filter))
        {
            yield return new FileSystemFileInfo(this, file);
        }
    }

    public override void Delete(IContext context)
    {
        Directory.Delete(NativeFullName, true);
    }

    public override IFileSystemInfo? GetChild(string childName, IContext context)
    {
        // ignore "*.txt"
        if (!context.Path.IsValidName(childName))
        {
            return null;
        }

        var directory = new DirectoryInfo(NativeFullName);
        var child = directory.EnumerateFileSystemInfos(childName, GetEnumerationOptions()).FirstOrDefault();

        if (child == null)
        {
            return null;
        }

        if (child is DirectoryInfo dir)
        {
            return new FileSystemDirectoryInfo(this, dir);
        }

        return new FileSystemFileInfo(this, (FileInfo)child);
    }

    public override IFileInfo CreateFile(string name, IContext context)
    {
        var fullName = context.Path.Combine(NativeFullName, name);
        new FileStream(fullName, FileMode.CreateNew, FileAccess.ReadWrite).Dispose();

        return new FileSystemFileInfo(this, new FileInfo(fullName));
    }

    public override IDirectoryInfo CreateDirectory(string name, IContext context)
    {
        var fullName = context.Path.Combine(NativeFullName, name);
        var info = Directory.CreateDirectory(fullName);

        return new FileSystemDirectoryInfo(this, info);
    }

    public override IDirectoryInfo Rename(string newName, IContext context)
    {
        var newLocation = context.Path.Combine(context.Path.GetParentPath(NativeFullName), newName);

        var directory = new DirectoryInfo(NativeFullName);
        directory.MoveTo(newLocation);

        return new FileSystemDirectoryInfo(Parent, directory);
    }

    private static EnumerationOptions GetEnumerationOptions()
    {
        return new EnumerationOptions
        {
            MatchType = MatchType.Win32,
            MatchCasing = MatchCasing.CaseInsensitive,
            AttributesToSkip = 0 // Default is to skip Hidden and System files, so we clear this to retain existing behavior
        };
    }

    private IEnumerable<FileSystemInfo> Enumerate(bool containers, ISearchFilter? filter)
    {
        var directory = new DirectoryInfo(NativeFullName);

        IEnumerable<FileSystemInfo> items;
        if (filter?.Filter == null)
        {
            items = containers ? directory.EnumerateDirectories() : directory.EnumerateFiles();
        }
        else
        {
            var options = GetEnumerationOptions();
            items = containers ? directory.EnumerateDirectories(filter.Filter, options) : directory.EnumerateFiles(filter.Filter, options);
        }

        return items;
    }
}