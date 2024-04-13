using ZipAsFolder.IO;
using ZipAsFolder.TestApi;

namespace ZipAsFolder.Suite.Internal;

[TestFixture]
public class TextContentReaderTest
{
    private Mock<IFileInfo> _file = null!;
    private TextContentReader _sut = null!;

    [SetUp]
    public void BeforeEachTest()
    {
        _file = new Mock<IFileInfo>(MockBehavior.Strict);
        _sut = new TextContentReader(_file.Object, false, null);
    }

    [Test]
    public void AfterEachTest()
    {
        _sut.Dispose();
    }

    [Test]
    public void ReadEmpty()
    {
        _file
            .Setup(s => s.OpenRead())
            .Returns(Stream.Null);

        _sut.Read(10, default).ShouldBeEmpty();
    }

    [Test]
    public void ReadFirstLine()
    {
        _file
            .Setup(s => s.OpenRead())
            .Returns(new StringBuilder().AppendLine("line 1").AppendLine("line 2").AsStream());

        _sut.Read(1, default).ShouldBe(new[] { "line 1" });
    }

    [Test]
    public void ReadFirst2Lines()
    {
        _file
            .Setup(s => s.OpenRead())
            .Returns(new StringBuilder().AppendLine("line 1").AppendLine("line 2").AppendLine("line 2").AsStream());

        _sut.Read(2, default).ShouldBe(new[] { "line 1", "line 2" });
    }

    [Test]
    public void ReadFirstThenSecond()
    {
        _file
            .Setup(s => s.OpenRead())
            .Returns(new StringBuilder().AppendLine("line 1").AppendLine("line 2").AsStream());

        _sut.Read(1, default).ShouldBe(new[] { "line 1" });
        _sut.Read(1, default).ShouldBe(new[] { "line 2" });
    }

    [Test]
    public void ReadAll()
    {
        _file
            .Setup(s => s.OpenRead())
            .Returns(new StringBuilder().AppendLine("line 1").AppendLine("line 2").AsStream());

        _sut.Read(10, default).ShouldBe(new[] { "line 1", "line 2" });
    }

    [Test]
    public void ReadRaw()
    {
        _sut.Raw = true;

        var expected = new StringBuilder().AppendLine("line 1").AppendLine("line 2");

        _file
            .Setup(s => s.OpenRead())
            .Returns(expected.AsStream());

        _sut.Read(1, default).ShouldBe(new[] { expected.ToString() });
    }

    [Test]
    public void CancelRead()
    {
        var tokenSource = new CancellationTokenSource();
        tokenSource.Cancel();

        _file
            .Setup(s => s.OpenRead())
            .Returns(new StringBuilder().AppendLine("line 1").AppendLine("line 2").AsStream());

        _sut.Read(10, tokenSource.Token).ShouldBeEmpty();
    }
}