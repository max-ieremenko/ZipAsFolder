using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ZipAsFolder.IO.Archive.Internal;

internal sealed class EntryStream : Stream
{
    private readonly IDisposable _owner;
    private readonly Stream _source;

    public EntryStream(IDisposable owner, Stream source)
    {
        _owner = owner;
        _source = source;
    }

    public override bool CanRead => _source.CanRead;

    public override bool CanSeek => _source.CanSeek;

    public override bool CanWrite => _source.CanWrite;

    public override long Length => _source.Length;

    public override long Position
    {
        get => _source.Position;
        set => _source.Position = value;
    }

    public override bool CanTimeout => _source.CanTimeout;

    public override int ReadTimeout
    {
        get => _source.ReadTimeout;
        set => _source.ReadTimeout = value;
    }

    public override int WriteTimeout
    {
        get => _source.WriteTimeout;
        set => _source.WriteTimeout = value;
    }

    public override void Flush() => _source.Flush();

    public override Task FlushAsync(CancellationToken cancellationToken) => _source.FlushAsync(cancellationToken);

    public override long Seek(long offset, SeekOrigin origin) => _source.Seek(offset, origin);

    public override void SetLength(long value) => _source.SetLength(value);

    public override int Read(byte[] buffer, int offset, int count) => _source.Read(buffer, offset, count);

    public override int Read(Span<byte> buffer) => _source.Read(buffer);

    public override int EndRead(IAsyncResult asyncResult) => _source.EndRead(asyncResult);

    public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state) => _source.BeginRead(buffer, offset, count, callback, state);

    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => _source.ReadAsync(buffer, offset, count, cancellationToken);

    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) => _source.ReadAsync(buffer, cancellationToken);

    public override int ReadByte() => _source.ReadByte();

    public override void Write(byte[] buffer, int offset, int count)
    {
        _source.Write(buffer, offset, count);
    }

    public override void Write(ReadOnlySpan<byte> buffer) => _source.Write(buffer);

    public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state) => _source.BeginWrite(buffer, offset, count, callback, state);

    public override void EndWrite(IAsyncResult asyncResult) => _source.EndWrite(asyncResult);

    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => _source.WriteAsync(buffer, offset, count, cancellationToken);

    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default) => _source.WriteAsync(buffer, cancellationToken);

    public override void WriteByte(byte value) => _source.WriteByte(value);

    public override void CopyTo(Stream destination, int bufferSize) => _source.CopyTo(destination, bufferSize);

    public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken) => _source.CopyToAsync(destination, bufferSize, cancellationToken);

    public override void Close()
    {
        _owner.Dispose();
        base.Close();
    }
}