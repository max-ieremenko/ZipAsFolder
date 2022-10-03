using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace ZipAsFolder.Archive.Zip;

internal sealed class ZipContent : IArchiveContent
{
    private static readonly Func<ZipArchiveEntry, IArchiveEntry> Cast = ToEntry;
    private readonly ZipArchive _content;
    private readonly CompressionLevel _compressionLevel;

    public ZipContent(ZipArchive content, CompressionLevel compressionLevel)
    {
        _content = content;
        _compressionLevel = compressionLevel;
    }

    public IEnumerable<IArchiveEntry> GetEntries() => _content.Entries.Select(Cast);

    public Stream OpenRead(Span<string> path)
    {
        var entry = GetEntry(path, false);
        return entry.Open();
    }

    public Stream OpenWrite(Span<string> path) => OpenRead(path);

    public IArchiveEntry CreateFileEntry(Span<string> path, string name)
    {
        var entryName = ZipEntry.CreateFullName(path, name, false);
        var entry = _content.CreateEntry(entryName, _compressionLevel);
        return Cast(entry);
    }

    public IArchiveEntry CreateDirectoryEntry(Span<string> path, string name)
    {
        var entryName = ZipEntry.CreateFullName(path, name, true);
        var entry = _content.CreateEntry(entryName, _compressionLevel);
        return Cast(entry);
    }

    public void RemoveFileEntry(Span<string> path)
    {
        var entry = GetEntry(path, false);
        entry.Delete();
    }

    public void RemoveDirectoryEntry(Span<string> path)
    {
        var entry = GetEntry(path, true);
        entry.Delete();
    }

    public void Dispose() => _content.Dispose();

    private static IArchiveEntry ToEntry(ZipArchiveEntry entry)
    {
        return new ZipEntry(entry);
    }

    private ZipArchiveEntry GetEntry(in Span<string> path, bool isDirectory)
    {
        var entryName = ZipEntry.CreateFullName(path, null, isDirectory);
        var entry = _content.GetEntry(entryName);
        if (entry == null)
        {
            throw new KeyNotFoundException("Entry [{0}] not found.".FormatWith(entryName));
        }

        return entry;
    }
}