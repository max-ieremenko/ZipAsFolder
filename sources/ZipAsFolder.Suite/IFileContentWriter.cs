using System;
using System.Collections;
using System.IO;
using System.Threading;

namespace ZipAsFolder.Suite;

public interface IFileContentWriter : IDisposable
{
    void Seek(long offset, SeekOrigin origin);

    void Write(IList content, CancellationToken token);
}