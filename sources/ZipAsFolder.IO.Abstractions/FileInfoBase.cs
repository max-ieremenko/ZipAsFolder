namespace ZipAsFolder.IO;

[DebuggerDisplay("{FullName}")]
public abstract class FileInfoBase : IFileInfo
{
    protected FileInfoBase(
        IDirectoryInfo parent,
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

    public IDirectoryInfo Parent { get; }

    public string Name { get; }

    public string FullName { get; }

    public string Extension => Path.GetExtension(Name);

    public FileSystemInfoAttributes Attributes => FileSystemInfoAttributes.File;

    public string DirectoryName => Parent.FullName;

    public long? Length { get; }

    public DateTime? LastWriteTime { get; }

    public virtual void ClearContent(IContext context)
    {
        using var stream = OpenWrite();
        stream.SetLength(0);
    }

    public override string ToString() => FullName;

    public abstract Stream OpenRead();

    public abstract Stream OpenWrite();

    public abstract void Delete();

    public virtual IFileInfo CopyTo(IDirectoryInfo destinationParent, IFileInfo? destination, string name, IContext context)
    {
        if (destination == null)
        {
            destination = destinationParent.CreateFile(name, context);
        }

        using (var sourceStream = OpenRead())
        using (var destStream = destination.OpenWrite())
        {
            destStream.SetLength(0);
            sourceStream.CopyTo(destStream);
        }

        return destination;
    }

    public virtual IFileInfo MoveTo(IDirectoryInfo destinationParent, IFileInfo? destination, string name, IContext context)
    {
        var result = CopyTo(destinationParent, destination, name, context);
        Delete();
        return result;
    }

    public abstract IFileInfo Rename(string newName, IContext context);
}