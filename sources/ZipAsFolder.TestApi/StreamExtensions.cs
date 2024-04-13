namespace ZipAsFolder.TestApi;

public static class StreamExtensions
{
    public static Stream AsStream(this StringBuilder text)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(text.ToString()));
    }
}