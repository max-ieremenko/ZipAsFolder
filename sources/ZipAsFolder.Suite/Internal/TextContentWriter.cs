using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading;
using ZipAsFolder.IO;

namespace ZipAsFolder.Suite.Internal;

internal sealed class TextContentWriter : IFileContentWriter
{
    private Stream? _fileStream;
    private StreamWriter? _fileWriter;

    public TextContentWriter(IFileInfo file, bool noNewLine, Encoding? encoding)
    {
        File = file;
        NoNewLine = noNewLine;
        Encoding = encoding;
    }

    public IFileInfo File { get; }

    public bool NoNewLine { get; internal set; }

    public Encoding? Encoding { get; internal set; }

    public void Seek(long offset, SeekOrigin origin)
    {
        OpenWrite().Flush();

        _fileStream!.Seek(offset, origin);
    }

    public void Write(IList content, CancellationToken token)
    {
        var writer = OpenWrite();

        for (var i = 0; i < content.Count; i++)
        {
            var item = content[i];

            if (item is object[] array)
            {
                for (var j = 0; j < array.Length; j++)
                {
                    WriteItem(array[j], writer);
                }
            }
            else
            {
                WriteItem(item, writer);
            }
        }
    }

    public void Dispose()
    {
        _fileWriter?.Dispose();
        _fileStream?.Dispose();
    }

    internal void Flush() => OpenWrite().Flush();

    private static bool TryGetString(object? line, [NotNullWhen(true)] out string? text)
    {
        if (line == null)
        {
            text = null;
            return false;
        }

        if (line is string str)
        {
            text = str;
            return true;
        }

        text = line.ToString();
        return true;
    }

    private StreamWriter OpenWrite()
    {
        if (_fileWriter == null)
        {
            _fileStream?.Dispose();
            _fileStream = File.OpenWrite();
            _fileWriter = Encoding == null ? new StreamWriter(_fileStream) : new StreamWriter(_fileStream, Encoding);
        }

        return _fileWriter;
    }

    private void WriteItem(object item, StreamWriter writer)
    {
        if (!TryGetString(item, out var line))
        {
            return;
        }

        if (NoNewLine)
        {
            writer.Write(line);
        }
        else
        {
            writer.WriteLine(line);
        }
    }
}