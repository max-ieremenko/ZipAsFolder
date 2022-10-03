using System;
using System.Collections.Generic;
using System.Linq;
using ZipAsFolder.Archive;

namespace ZipAsFolder.IO.Archive.Internal;

internal readonly ref struct EntryReader
{
    private readonly IArchive _archive;
    private readonly IPath _path;

    public EntryReader(IArchive archive, IPath path)
    {
        _archive = archive;
        _path = path;
    }

    public void Read(ArchiveDirectoryInfo root)
    {
        using var content = _archive.OpenRead();

        Read(root, content.GetEntries());
    }

    private void Read(ArchiveDirectoryInfo root, IEnumerable<IArchiveEntry> entries)
    {
        /*
            1/
            1/11.txt
            2/
            2/2.txt
            2/2.2/2.2.txt
            inner.zip
         */

        var ordered = entries
            .OrderBy(i => i.IsFile ? 1 : 0)
            .ThenBy(i => i.Path.Length);

        foreach (var entry in ordered)
        {
            var owner = root;

            for (var i = 0; i < entry.Path.Length - 1; i++)
            {
                var pathItem = entry.Path[i];
                if (!owner.FolderByName.TryGetValue(pathItem, out var next))
                {
                    var path = new string[i + 1];
                    Array.Copy(entry.Path, 0, path, 0, path.Length);

                    next = owner.AddFolder(null, path, pathItem, _archive, _path);
                }

                owner = next;
            }

            var entryName = entry.Path[entry.Path.Length - 1];

            if (entry.IsFile)
            {
                owner.AddFile(entry, entryName, _archive, _path);
            }
            else if (!owner.FolderByName.ContainsKey(entryName))
            {
                owner.AddFolder(entry, entry.Path, entryName, _archive, _path);
            }
        }
    }
}