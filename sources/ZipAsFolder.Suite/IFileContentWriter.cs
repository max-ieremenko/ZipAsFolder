namespace ZipAsFolder.Suite;

public interface IFileContentWriter : IDisposable
{
    void Seek(long offset, SeekOrigin origin);

    void Write(IList content, CancellationToken token);
}