namespace ZipAsFolder.IO;

[DebuggerDisplay("{FullName}")]
public abstract class DirectoryInfoBase : IDirectoryInfo
{
    protected DirectoryInfoBase(
        IDirectoryInfo? parent,
        string name,
        string fullName,
        long? length,
        DateTime? lastWriteTime)
    {
        Parent = parent;
        Name = name;
        FullName = FileSystemPath.ToPsDrivePath(fullName);
        Length = length;
        LastWriteTime = lastWriteTime;
    }

    public IDirectoryInfo? Parent { get; }

    public string Name { get; }

    public string FullName { get; }

    public string DirectoryName => Parent?.FullName ?? string.Empty;

    public virtual FileSystemInfoAttributes Attributes => FileSystemInfoAttributes.Directory;

    public long? Length { get; }

    public DateTime? LastWriteTime { get; }

    public virtual void ClearContent(IContext context)
    {
        throw new NotSupportedException("Unable to clear content of '{0}' because it is a directory.".FormatWith(FullName));
    }

    public abstract bool IsEmpty(IContext context);

    public abstract IEnumerable<IDirectoryInfo> EnumerateDirectories(ISearchFilter? filter, IContext context);

    public abstract IEnumerable<IFileInfo> EnumerateFiles(ISearchFilter? filter, IContext context);

    public abstract IFileSystemInfo? GetChild(string childName, IContext context);

    public abstract IFileInfo CreateFile(string name, IContext context);

    public abstract IDirectoryInfo CreateDirectory(string name, IContext context);

    public abstract void Delete(IContext context);

    public abstract IDirectoryInfo Rename(string newName, IContext context);

    public override string ToString() => FullName;
}