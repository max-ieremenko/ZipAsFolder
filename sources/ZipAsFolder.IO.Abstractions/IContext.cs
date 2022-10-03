using System.Threading;

namespace ZipAsFolder.IO;

public interface IContext
{
    IPath Path { get; }

    CancellationToken Token { get; }
}