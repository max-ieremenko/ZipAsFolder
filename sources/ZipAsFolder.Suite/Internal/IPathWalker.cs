using ZipAsFolder.IO;

namespace ZipAsFolder.Suite.Internal;

internal interface IPathWalker
{
    IFileSystemInfo? Current { get; }

    bool IsCompleted { get; }

    bool IsValid { get; }

    string NextName { get; }

    bool LastIsDirectory { get; }

    bool NextIsLast { get; }

    void SetNext(IFileSystemInfo next);
}