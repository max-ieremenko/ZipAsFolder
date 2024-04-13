namespace ZipAsFolder.Suite;

public interface IFileContentReader : IDisposable
{
    IList Read(long readCount, CancellationToken token);
}