using ZipAsFolder.Archive;

namespace ZipAsFolder.IO.Archive;

internal sealed class StubArchiveEntry : IArchiveEntry
{
    public string[] Path { get; set; } = Array.Empty<string>();

    public string FullName { get; set; } = string.Empty;

    public DateTime? LastWriteTime { get; set; }

    public bool IsFile { get; set; }

    public long? Length { get; set; }
}