using System.IO;

namespace ZipAsFolder.Archive;

public interface IArchiveContentProvider
{
    IArchiveContent OpenRead(Stream stream);

    IArchiveContent OpenWrite(Stream stream);

    void ClearContent(Stream stream);
}