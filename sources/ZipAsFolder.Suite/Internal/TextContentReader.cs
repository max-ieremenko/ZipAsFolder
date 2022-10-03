using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using ZipAsFolder.IO;

namespace ZipAsFolder.Suite.Internal;

internal sealed class TextContentReader : IFileContentReader
{
    private Stream? _fileStream;
    private StreamReader? _fileReader;

    public TextContentReader(IFileInfo file, bool raw, Encoding? encoding)
    {
        File = file;
        Raw = raw;
        Encoding = encoding;
    }

    public IFileInfo File { get; }

    public bool Raw { get; internal set; }

    public Encoding? Encoding { get; internal set; }

    IList IFileContentReader.Read(long linesCount, CancellationToken token) => (IList)Read(linesCount, token);

    public void Dispose()
    {
        _fileStream?.Dispose();
        _fileReader?.Dispose();
    }

    internal IList<string> Read(long linesCount, CancellationToken token)
    {
        var reader = OpenRead();
        if (linesCount == 0 || reader.EndOfStream || token.IsCancellationRequested)
        {
            return Array.Empty<string>();
        }

        string? line;

        if (Raw)
        {
            line = reader.ReadToEnd();
            return new[] { line };
        }

        // by default readCount=1
        var result = new List<string>((int)Math.Min(linesCount, 100));

        while (linesCount > 0 && !token.IsCancellationRequested && (line = reader.ReadLine()) != null)
        {
            result.Add(line);
            linesCount--;
        }

        return result;
    }

    private StreamReader OpenRead()
    {
        if (_fileReader == null)
        {
            _fileStream?.Dispose();
            _fileStream = File.OpenRead();
            _fileReader = Encoding == null ? new StreamReader(_fileStream) : new StreamReader(_fileStream, Encoding);
        }

        return _fileReader;
    }
}