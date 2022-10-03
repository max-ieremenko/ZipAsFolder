using Moq;
using NUnit.Framework;
using Shouldly;
using ZipAsFolder.IO;

namespace ZipAsFolder.Suite.Internal;

[TestFixture]
public class FileSystemProviderTest
{
    private Mock<IFileSystem> _system = null!;
    private Mock<IContext> _context = null!;
    private FileSystemProvider _sut = null!;

    [SetUp]
    public void BeforeEachTest()
    {
        _system = new Mock<IFileSystem>(MockBehavior.Strict);
        _context = new Mock<IContext>(MockBehavior.Strict);

        _sut = new FileSystemProvider(new[] { _system.Object });
    }

    [Test]
    public void GetRoot()
    {
        var root = new Mock<IDirectoryInfo>(MockBehavior.Strict);

        var systemRoot = root.Object;
        _system
            .Setup(s => s.TryGetRoot("/", out systemRoot))
            .Returns(true);

        var actual = _sut.GetRoot("/");

        actual.ShouldBe(root.Object);
    }

    [Test]
    public void CreateFileRecognizedAsDirectory()
    {
        var expected = new Mock<IDirectoryInfo>(MockBehavior.Strict);
        expected
            .Setup(e => e.ClearContent(_context.Object));

        _system
            .Setup(s => s.CanFileNameBeDirectory("11.zip"))
            .Returns(true);

        var file = new Mock<IFileInfo>(MockBehavior.Strict);

        var parent = new Mock<IDirectoryInfo>(MockBehavior.Strict);
        parent
            .Setup(p => p.CreateFile("11.zip", _context.Object))
            .Returns(file.Object);

        _system
            .Setup(s => s.AsDirectory(file.Object))
            .Returns(expected.Object);

        _sut.CreateFile(parent.Object, "11.zip", _context.Object).ShouldBe(expected.Object);

        expected.VerifyAll();
    }

    [Test]
    public void CreateFile()
    {
        var expected = new Mock<IFileInfo>(MockBehavior.Strict);
        var parent = new Mock<IDirectoryInfo>(MockBehavior.Strict);

        _system
            .Setup(s => s.CanFileNameBeDirectory("the name"))
            .Returns(false);

        parent
            .Setup(p => p.CreateFile("the name", _context.Object))
            .Returns(expected.Object);

        _sut.CreateFile(parent.Object, "the name", _context.Object).ShouldBe(expected.Object);
    }
}