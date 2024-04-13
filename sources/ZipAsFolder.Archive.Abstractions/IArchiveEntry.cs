namespace ZipAsFolder.Archive;

public interface IArchiveEntry
{
    string[] Path { get; }

    DateTime? LastWriteTime { get; }

    bool IsFile { get; }

    long? Length { get; }
}