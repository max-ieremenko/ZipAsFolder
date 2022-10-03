using System;
using System.Collections.Generic;
using System.IO;

namespace ZipAsFolder.Archive;

public interface IArchiveContent : IDisposable
{
    IEnumerable<IArchiveEntry> GetEntries();

    Stream OpenRead(Span<string> path);

    Stream OpenWrite(Span<string> path);

    IArchiveEntry CreateFileEntry(Span<string> path, string name);

    IArchiveEntry CreateDirectoryEntry(Span<string> path, string name);

    void RemoveFileEntry(Span<string> path);

    void RemoveDirectoryEntry(Span<string> path);
}