using ZipAsFolder.Archive;

namespace ZipAsFolder.IO.Archive.Internal;

internal interface IArchive
{
    IArchiveContent OpenRead();

    IArchiveContent OpenWrite();
}