using ZipAsFolder.IO;

namespace ZipAsFolder.Suite.Internal;

[TestFixture]
public class TextContentWriterTest
{
    private Stream _content = null!;
    private TextContentWriter _sut = null!;

    [SetUp]
    public void BeforeEachTest()
    {
        _content = new MemoryStream();

        var file = new Mock<IFileInfo>(MockBehavior.Strict);
        file
            .Setup(f => f.OpenWrite())
            .Returns(_content);

        _sut = new TextContentWriter(file.Object, false, null);
    }

    [Test]
    public void AfterEachTest()
    {
        _sut.Dispose();
    }

    [Test]
    public void WriteTwoLines()
    {
        _sut.Write(new object[] { "line 1", "line 2" }, default);

        ReadContent().ShouldBe(new[] { "line 1", "line 2" });
    }

    [Test]
    public void WriteTwoLinesNoNewLine()
    {
        _sut.NoNewLine = true;
        _sut.Write(new object[] { "line 1", "line 2" }, default);

        ReadContent().ShouldBe(new[] { "line 1line 2" });
    }

    [Test]
    public void WriteNull()
    {
        _sut.Write(new object?[] { null }, default);

        // see FileSystemProvider implementation
        ReadContent().ShouldBeEmpty();
    }

    [Test]
    public void WriteNullNoNewLine()
    {
        _sut.NoNewLine = true;
        _sut.Write(new object?[] { null }, default);

        ReadContent().ShouldBeEmpty();
    }

    [Test]
    public void WriteObject()
    {
        _sut.Write(new object[] { 1, 2L }, default);

        ReadContent().ShouldBe(new[] { "1", "2" });
    }

    [Test]
    public void WriteObjectArray()
    {
        var line = new object[] { 1, 2L };
        _sut.Write(new object[] { line }, default);

        ReadContent().ShouldBe(new[] { "1", "2" });
    }

    [Test]
    public void Seek()
    {
        _sut.Write(new object[] { "line 1" }, default);
        _sut.Flush();

        _sut.Write(new object[] { "line 1" }, default);

        _sut.Seek(0, SeekOrigin.Begin);
        _sut.Write(new object[] { "line 2" }, default);

        ReadContent().ShouldBe(new[] { "line 2", "line 1" });
    }

    private List<string> ReadContent()
    {
        _sut.Flush();
        _content.Seek(0, SeekOrigin.Begin);

        var reader = _sut.Encoding == null ? new StreamReader(_content) : new StreamReader(_content, _sut.Encoding);
        var result = new List<string>();

        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            result.Add(line);
        }

        return result;
    }
}