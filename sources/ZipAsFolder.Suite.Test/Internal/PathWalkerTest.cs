using ZipAsFolder.IO;

namespace ZipAsFolder.Suite.Internal;

[TestFixture]
public class PathWalkerTest
{
    private Mock<IFileSystemProvider> _fileSystem = null!;
    private Mock<IContext> _context = null!;
    private Mock<IPath> _path = null!;
    private Mock<IDirectoryInfo> _root = null!;

    [SetUp]
    public void BeforeEachTest()
    {
        _root = new Mock<IDirectoryInfo>(MockBehavior.Strict);
        _root
            .SetupGet(r => r.FullName)
            .Returns("/");

        _path = new Mock<IPath>(MockBehavior.Strict);
        _path
            .Setup(p => p.ItemSeparator)
            .Returns("/");

        _context = new Mock<IContext>(MockBehavior.Strict);
        _context
            .SetupGet(c => c.Path)
            .Returns(_path.Object);

        _fileSystem = new Mock<IFileSystemProvider>(MockBehavior.Strict);
        _fileSystem
            .Setup(f => f.GetRoot("/"))
            .Returns(_root.Object);
    }

    [Test]
    [TestCase("/", "/")]
    [TestCase("/a", "/", "a")]
    [TestCase("c:/a", "c:", "a")]
    [TestCase("/a/b", "/", "a", "b")]
    [TestCase("/a/", "/", "a")]
    [TestCase("/./", "/")]
    [TestCase("/a/.", "/", "a")]
    [TestCase("/a/./b", "/", "a", "b")]
    [TestCase("/a/./b/./", "/", "a", "b")]
    [TestCase("/a/b/..", "/", "a")]
    [TestCase("/a/b/../..", "/")]
    [TestCase("/a/../b", "/", "b")]
    [TestCase("/a/../b/c/../", "/", "b")]
    [TestCase("/a/..", "/")]
    [TestCase("/a/../", "/")]
    [TestCase("/a/../..")]
    public void SplitPath(string path, params string[] expected)
    {
        _path
            .Setup(p => p.IsPathRooted(path))
            .Returns(true);

        var sut = new PathWalker(_fileSystem.Object, _context.Object, path);

        sut.Path.ShouldBe(expected);
        sut.LastIsDirectory.ShouldBe(path.EndsWith("/"));
    }

    [Test]
    public void RootNotFound()
    {
        _fileSystem
            .Setup(f => f.GetRoot("/"))
            .Returns((IDirectoryInfo?)null);

        var sut = CreateSut("/11");

        sut.IsValid.ShouldBeFalse();
    }

    [Test]
    public void ResolveRoot()
    {
        var sut = CreateSut("/");

        sut.Current.ShouldBe(_root.Object);
        sut.NextName.ShouldBeNullOrEmpty();
        sut.IsValid.ShouldBeTrue();
    }

    [Test]
    [TestCase("/11/22")]
    [TestCase("/11/22/")]
    public void ResolveFullPathToDirectory(string path)
    {
        var dir11 = new Mock<IDirectoryInfo>(MockBehavior.Strict);
        dir11
            .SetupGet(d => d.FullName)
            .Returns("/11");

        var dir22 = new Mock<IDirectoryInfo>(MockBehavior.Strict);

        _fileSystem
            .Setup(f => f.GetChild(_root.Object, "11", _context.Object))
            .Returns(dir11.Object);

        _fileSystem
            .Setup(f => f.GetChild(dir11.Object, "22", _context.Object))
            .Returns(dir22.Object);

        var sut = CreateSut(path);

        sut.Current.ShouldBe(dir22.Object);
        sut.NextName.ShouldBeNullOrEmpty();
        sut.IsValid.ShouldBeTrue();
    }

    [Test]
    public void ResolveFullPathToFile()
    {
        var dir = new Mock<IDirectoryInfo>(MockBehavior.Strict);
        dir
            .SetupGet(d => d.FullName)
            .Returns("/11");

        var file = new Mock<IFileInfo>(MockBehavior.Strict);

        _fileSystem
            .Setup(f => f.GetChild(_root.Object, "11", _context.Object))
            .Returns(dir.Object);

        _fileSystem
            .Setup(f => f.GetChild(dir.Object, "22", _context.Object))
            .Returns(file.Object);

        var sut = CreateSut("/11/22");

        sut.Current.ShouldBe(file.Object);
        sut.NextName.ShouldBeNullOrEmpty();
        sut.IsValid.ShouldBeTrue();
    }

    [Test]
    public void LastMustBeADirectory()
    {
        var dir = new Mock<IDirectoryInfo>(MockBehavior.Strict);
        dir
            .SetupGet(d => d.FullName)
            .Returns("/11");

        var file = new Mock<IFileInfo>(MockBehavior.Strict);

        _fileSystem
            .Setup(f => f.GetChild(_root.Object, "11", _context.Object))
            .Returns(dir.Object);

        _fileSystem
            .Setup(f => f.GetChild(dir.Object, "22", _context.Object))
            .Returns(file.Object);

        var sut = CreateSut("/11/22/");

        sut.IsValid.ShouldBeFalse();
    }

    [Test]
    public void FileInTheMiddle()
    {
        var file = new Mock<IFileInfo>(MockBehavior.Strict);
        file
            .SetupGet(d => d.FullName)
            .Returns("/11");

        _fileSystem
            .Setup(f => f.GetChild(_root.Object, "11", _context.Object))
            .Returns(file.Object);

        var sut = CreateSut("/11/22");

        sut.IsValid.ShouldBeFalse();
    }

    [Test]
    public void ResolvePartially()
    {
        var dir = new Mock<IDirectoryInfo>(MockBehavior.Strict);
        dir
            .SetupGet(d => d.FullName)
            .Returns("/11");

        _fileSystem
            .Setup(f => f.GetChild(_root.Object, "11", _context.Object))
            .Returns(dir.Object);

        _fileSystem
            .Setup(f => f.GetChild(dir.Object, "22", _context.Object))
            .Returns((IFileSystemInfo?)null);

        var sut = CreateSut("/11/22");

        sut.Current.ShouldBe(dir.Object);
        sut.IsValid.ShouldBeTrue();
        sut.NextName.ShouldBe("22");
        sut.NextIsLast.ShouldBeTrue();
    }

    [Test]
    public void SetNext()
    {
        _fileSystem
            .Setup(f => f.GetChild(_root.Object, "1", _context.Object))
            .Returns((IFileSystemInfo?)null);

        var sut = CreateSut("/1/2/3");

        sut.Current.ShouldBe(_root.Object);
        sut.IsValid.ShouldBeTrue();
        sut.NextName.ShouldBe("1");
        sut.NextIsLast.ShouldBeFalse();
        sut.IsCompleted.ShouldBeFalse();

        var dir1 = new Mock<IDirectoryInfo>(MockBehavior.Strict);
        dir1
            .SetupGet(d => d.FullName)
            .Returns("/1");

        sut.SetNext(dir1.Object);

        sut.Current.ShouldBe(dir1.Object);
        sut.IsValid.ShouldBeTrue();
        sut.NextName.ShouldBe("2");
        sut.NextIsLast.ShouldBeFalse();
        sut.IsCompleted.ShouldBeFalse();

        var dir2 = new Mock<IDirectoryInfo>(MockBehavior.Strict);
        dir2
            .SetupGet(d => d.FullName)
            .Returns("/1/2");

        sut.SetNext(dir2.Object);

        sut.Current.ShouldBe(dir2.Object);
        sut.IsValid.ShouldBeTrue();
        sut.NextName.ShouldBe("3");
        sut.NextIsLast.ShouldBeTrue();
        sut.IsCompleted.ShouldBeFalse();

        var file = new Mock<IFileInfo>(MockBehavior.Strict);

        sut.SetNext(file.Object);

        sut.Current.ShouldBe(file.Object);
        sut.IsValid.ShouldBeTrue();
        sut.NextName.ShouldBeNullOrEmpty();
        sut.IsCompleted.ShouldBeTrue();
    }

    [Test]
    public void SetNextFileInTheMiddle()
    {
        _fileSystem
            .Setup(f => f.GetChild(_root.Object, "1", _context.Object))
            .Returns((IFileSystemInfo?)null);

        var sut = CreateSut("/1/2");

        var file = new Mock<IFileInfo>(MockBehavior.Strict);

        sut.SetNext(file.Object);

        sut.Current.ShouldBe(file.Object);
        sut.IsValid.ShouldBeFalse();
        sut.NextName.ShouldBe("2");
        sut.IsCompleted.ShouldBeFalse();
    }

    private PathWalker CreateSut(string path)
    {
        _path
            .Setup(p => p.IsPathRooted(path))
            .Returns(true);

        var result = new PathWalker(_fileSystem.Object, _context.Object, path);
        result.Initialize();
        return result;
    }
}