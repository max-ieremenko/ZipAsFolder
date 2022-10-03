using ZipAsFolder.Archive;

namespace ZipAsFolder.IO.Archive.Internal;

internal static class ArchiveExtensions
{
    public static ArchiveFileInfo AddFile(
        this ArchiveDirectoryInfo parent,
        IArchiveEntry entry,
        string name,
        IArchive archive,
        IPath path)
    {
        var fullName = path.Combine(parent.FullName, name);

        var file = new ArchiveFileInfo(archive, parent, name, fullName, entry);
        parent.FileByName.Add(name, file);
        return file;
    }

    public static ArchiveDirectoryInfo AddFolder(
        this ArchiveDirectoryInfo parent,
        IArchiveEntry? entry,
        string[] archivePath,
        string name,
        IArchive archive,
        IPath path)
    {
        var fullName = path.Combine(parent.FullName, name);

        var folder = new ArchiveDirectoryInfo(archive, archivePath, parent, name, fullName, entry?.Length, entry?.LastWriteTime);
        parent.FolderByName.Add(name, folder);
        return folder;
    }
}