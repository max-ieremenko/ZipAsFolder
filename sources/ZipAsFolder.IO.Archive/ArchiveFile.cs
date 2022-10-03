using System;
using System.Collections.Generic;
using ZipAsFolder.Archive;
using ZipAsFolder.IO.Archive.Internal;

namespace ZipAsFolder.IO.Archive;

internal sealed class ArchiveFile : DirectoryInfoBase, IArchive
{
    private readonly IFileInfo _file;
    private readonly IArchiveContentProvider _archive;
    private ArchiveDirectoryInfo? _tree;

    public ArchiveFile(IFileInfo file, IArchiveContentProvider archive)
        : base(file.Parent, file.Name, file.FullName, file.Length, file.LastWriteTime)
    {
        _file = file;
        _archive = archive;
    }

    public override FileSystemInfoAttributes Attributes => FileSystemInfoAttributes.File | FileSystemInfoAttributes.Archive;

    public override void ClearContent(IContext context)
    {
        using (var stream = _file.OpenWrite())
        {
            _archive.ClearContent(stream);
        }
    }

    public override bool IsEmpty(IContext context) => AsTree(context.Path).IsEmpty(context);

    public override IEnumerable<IDirectoryInfo> EnumerateDirectories(ISearchFilter? filter, IContext context) => AsTree(context.Path).EnumerateDirectories(filter, context);

    public override IEnumerable<IFileInfo> EnumerateFiles(ISearchFilter? filter, IContext context) => AsTree(context.Path).EnumerateFiles(filter, context);

    public override IFileSystemInfo? GetChild(string childName, IContext context) => AsTree(context.Path).GetChild(childName, context);

    public override IFileInfo CreateFile(string name, IContext context) => AsTree(context.Path).CreateFile(name, context);

    public override IDirectoryInfo CreateDirectory(string name, IContext context) => AsTree(context.Path).CreateDirectory(name, context);

    public override void Delete(IContext context) => _file.Delete();

    public override IDirectoryInfo Rename(string newName, IContext context)
    {
        var newLocation = _file.Rename(newName, context);

        var result = new ArchiveFile(newLocation, _archive);
        result._tree = _tree;

        return result;
    }

    public IArchiveContent OpenRead() => _archive.OpenRead(_file.OpenRead());

    public IArchiveContent OpenWrite() => _archive.OpenWrite(_file.OpenWrite());

    internal IFileInfo AsFile() => _file;

    private ArchiveDirectoryInfo AsTree(IPath path)
    {
        if (_tree == null)
        {
            var tree = new ArchiveDirectoryInfo(this, Array.Empty<string>(), this, Name, FullName, Length, LastWriteTime);
            new EntryReader(this, path).Read(tree);
            _tree = tree;
        }

        return _tree;
    }
}