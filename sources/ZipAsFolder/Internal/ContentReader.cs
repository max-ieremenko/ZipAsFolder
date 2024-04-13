using System.Management.Automation.Provider;
using ZipAsFolder.Suite;

namespace ZipAsFolder.Internal;

// https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.management/get-content?view=powershell-7.2
internal sealed class ContentReader : IContentReader
{
    private readonly IFileContentReader _reader;
    private readonly CancellationToken _token;

    public ContentReader(IFileContentReader reader, CancellationToken token)
    {
        _reader = reader;
        _token = token;
    }

    public IList Read(long readCount)
    {
        return _reader.Read(readCount, _token);
    }

    public void Seek(long offset, SeekOrigin origin)
    {
        throw new NotImplementedException();
    }

    public void Close() => Dispose();

    public void Dispose() => _reader.Dispose();
}