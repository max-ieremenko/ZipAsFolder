using System.Collections;
using System.IO;
using System.Management.Automation.Provider;
using System.Threading;
using ZipAsFolder.Suite;

namespace ZipAsFolder.Internal;

internal sealed class ContentWriter : IContentWriter
{
    private readonly IFileContentWriter _writer;
    private readonly CancellationToken _token;

    public ContentWriter(IFileContentWriter writer, CancellationToken token)
    {
        _writer = writer;
        _token = token;
    }

    public void Seek(long offset, SeekOrigin origin)
    {
        _writer.Seek(offset, origin);
    }

    public IList Write(IList content)
    {
        _writer.Write(content, _token);
        return content;
    }

    public void Dispose() => _writer.Dispose();

    public void Close() => Dispose();
}