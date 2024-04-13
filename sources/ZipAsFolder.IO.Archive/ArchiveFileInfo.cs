using ZipAsFolder.Archive;
using ZipAsFolder.IO.Archive.Internal;

namespace ZipAsFolder.IO.Archive;

internal sealed class ArchiveFileInfo : FileInfoBase
{
    private readonly IArchive _archive;
    private readonly string[] _archivePath;

    public ArchiveFileInfo(
        IArchive archive,
        ArchiveDirectoryInfo parent,
        string name,
        string fullName,
        IArchiveEntry info)
        : base(parent, name, fullName, info.Length, info.LastWriteTime)
    {
        _archive = archive;
        _archivePath = info.Path;
    }

    public new ArchiveDirectoryInfo Parent => (ArchiveDirectoryInfo)base.Parent;

    public override Stream OpenRead()
    {
        var content = _archive.OpenRead();
        Stream entry;
        try
        {
            entry = content.OpenRead(_archivePath);
        }
        catch
        {
            content.Dispose();
            throw;
        }

        return new EntryStream(content, entry);
    }

    public override Stream OpenWrite()
    {
        var content = _archive.OpenWrite();
        Stream entry;
        try
        {
            entry = content.OpenWrite(_archivePath);
        }
        catch
        {
            content.Dispose();
            throw;
        }

        return new EntryStream(content, entry);
    }

    public override void Delete()
    {
        using var content = _archive.OpenWrite();
        content.RemoveFileEntry(_archivePath);

        Parent.FileByName.Remove(Name);
    }

    public override IFileInfo Rename(string newName, IContext context)
    {
        var newEntryFullName = context.Path.Combine(Parent.FullName, newName);
        var newPath = _archivePath.AsSpan().Slice(0, _archivePath.Length - 1);

        IArchiveEntry newEntry;
        using (var content = _archive.OpenWrite())
        {
            newEntry = MoveTo(content, newPath, newName);
        }

        return new ArchiveFileInfo(_archive, Parent, newName, newEntryFullName, newEntry);
    }

    internal IArchiveEntry MoveTo(IArchiveContent content, in Span<string> path, string name)
    {
        var newEntry = content.CreateFileEntry(path, name);

        using (var source = content.OpenRead(_archivePath))
        using (var dest = content.OpenWrite(newEntry.Path))
        {
            source.CopyTo(dest);
        }

        content.RemoveFileEntry(_archivePath);

        return newEntry;
    }
}