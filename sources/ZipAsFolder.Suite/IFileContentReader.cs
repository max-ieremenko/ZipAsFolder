using System;
using System.Collections;
using System.Threading;

namespace ZipAsFolder.Suite;

public interface IFileContentReader : IDisposable
{
    IList Read(long readCount, CancellationToken token);
}