using System;
using System.Collections.Generic;
using ZipAsFolder.Archive;
using ZipAsFolder.IO.Archive.Internal;

namespace ZipAsFolder.IO.Archive;

internal sealed class ArchiveDirectoryInfo : DirectoryInfoBase
{
    private readonly IArchive _archive;

    public ArchiveDirectoryInfo(
        IArchive archive,
        string[] archivePath,
        IDirectoryInfo? parent,
        string name,
        string fullName,
        long? length,
        DateTime? lastWriteTime)
        : base(parent, name, fullName, length, lastWriteTime)
    {
        ArchivePath = archivePath;
        _archive = archive;
        FolderByName = new Dictionary<string, ArchiveDirectoryInfo>(StringComparer.OrdinalIgnoreCase);
        FileByName = new Dictionary<string, ArchiveFileInfo>(StringComparer.OrdinalIgnoreCase);
    }

    internal string[] ArchivePath { get; }

    internal Dictionary<string, ArchiveDirectoryInfo> FolderByName { get; }

    internal Dictionary<string, ArchiveFileInfo> FileByName { get; }

    public override bool IsEmpty(IContext context)
    {
        return FolderByName.Count == 0 && FileByName.Count == 0;
    }

    public override IEnumerable<IDirectoryInfo> EnumerateDirectories(ISearchFilter? filter, IContext context)
    {
        foreach (var entry in FolderByName.Values)
        {
            if (filter.IsNullOrMatch(entry.Name))
            {
                yield return entry;
            }
        }
    }

    public override IEnumerable<IFileInfo> EnumerateFiles(ISearchFilter? filter, IContext context)
    {
        foreach (var entry in FileByName.Values)
        {
            if (filter.IsNullOrMatch(entry.Name))
            {
                yield return entry;
            }
        }
    }

    public override IFileSystemInfo? GetChild(string childName, IContext context)
    {
        if (FolderByName.TryGetValue(childName, out var folder))
        {
            return folder;
        }

        if (FileByName.TryGetValue(childName, out var file))
        {
            return file;
        }

        return null;
    }

    public override IFileInfo CreateFile(string name, IContext context)
    {
        IArchiveEntry entry;
        using (var content = _archive.OpenWrite())
        {
            entry = content.CreateFileEntry(ArchivePath, name);
        }

        return this.AddFile(entry, name, _archive, context.Path);
    }

    public override IDirectoryInfo CreateDirectory(string name, IContext context)
    {
        IArchiveEntry entry;
        using (var content = _archive.OpenWrite())
        {
            entry = content.CreateDirectoryEntry(ArchivePath, name);
        }

        return this.AddFolder(entry, entry.Path, name, _archive, context.Path);
    }

    public override void Delete(IContext context)
    {
        foreach (var directory in FolderByName.Values)
        {
            if (context.Token.IsCancellationRequested)
            {
                return;
            }

            directory.Delete(context);
        }

        FolderByName.Clear();

        foreach (var file in FileByName.Values)
        {
            if (context.Token.IsCancellationRequested)
            {
                return;
            }

            file.Delete();
        }

        FileByName.Clear();

        using var content = _archive.OpenWrite();
        content.RemoveDirectoryEntry(ArchivePath);
    }

    public override IDirectoryInfo Rename(string newName, IContext context)
    {
        var newEntryFullName = context.Path.Combine(Parent!.FullName, newName);

        IArchiveEntry newEntry;
        using (var content = _archive.OpenWrite())
        {
            newEntry = MoveTo(content, ArchivePath.AsSpan().Slice(0, ArchivePath.Length - 1), newName);
        }

        return new ArchiveDirectoryInfo(_archive, newEntry.Path, Parent, newName, newEntryFullName, newEntry.Length, newEntry.LastWriteTime);
    }

    private IArchiveEntry MoveTo(IArchiveContent content, in Span<string> path, string name)
    {
        var result = content.CreateDirectoryEntry(path, name);

        foreach (var file in FileByName.Values)
        {
            file.MoveTo(content, result.Path, file.Name);
        }

        foreach (var folder in FolderByName.Values)
        {
            folder.MoveTo(content, result.Path, folder.Name);
        }

        content.RemoveDirectoryEntry(ArchivePath);
        return result;
    }
}