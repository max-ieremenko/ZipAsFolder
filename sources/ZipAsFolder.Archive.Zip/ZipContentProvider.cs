namespace ZipAsFolder.Archive.Zip;

public sealed class ZipContentProvider : IArchiveContentProvider
{
    private readonly CompressionLevel _compressionLevel;

    public ZipContentProvider(CompressionLevel compressionLevel)
    {
        _compressionLevel = compressionLevel;
    }

    public IArchiveContent OpenRead(Stream stream)
    {
        var zip = new ZipArchive(stream, ZipArchiveMode.Read, leaveOpen: false);
        return new ZipContent(zip, _compressionLevel);
    }

    public IArchiveContent OpenWrite(Stream stream)
    {
        var zip = new ZipArchive(stream, ZipArchiveMode.Update, leaveOpen: false);
        return new ZipContent(zip, _compressionLevel);
    }

    public void ClearContent(Stream stream)
    {
        stream.SetLength(0);
        new ZipArchive(stream, ZipArchiveMode.Create).Dispose();
    }
}