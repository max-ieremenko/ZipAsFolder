namespace ZipAsFolder.TestApi;

public sealed class TempDirectory : IDisposable
{
    public TempDirectory()
        : this(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()))
    {
    }

    public TempDirectory(string location)
    {
        Location = location;
        Directory.CreateDirectory(Location);
    }

    public string Location { get; }

    public void Dispose()
    {
        if (Directory.Exists(Location))
        {
            Directory.Delete(Location, true);
        }
    }
}