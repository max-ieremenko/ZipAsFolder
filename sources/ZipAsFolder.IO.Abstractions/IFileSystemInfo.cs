using System;

namespace ZipAsFolder.IO;

public interface IFileSystemInfo
{
    IDirectoryInfo? Parent { get; }

    string Name { get; }

    string FullName { get; }

    string DirectoryName { get; }

    FileSystemInfoAttributes Attributes { get; }

    long? Length { get; }

    DateTime? LastWriteTime { get; }

    void ClearContent(IContext context);
}