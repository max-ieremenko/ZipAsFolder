using System;
using System.IO;
using Moq;
using NUnit.Framework;
using Shouldly;
using ZipAsFolder.TestApi;

namespace ZipAsFolder.IO.FileSystem;

[TestFixture]
public class FileSystemFileInfoTest
{
    private Mock<IContext> _context = null!;
    private TempDirectory _directory = null!;
    private FileSystemFileInfo _sut = null!;

    [SetUp]
    public void BeforeEachTest()
    {
        _directory = new TempDirectory();

        var file = Path.Combine(_directory.Location, "test.txt");
        File.WriteAllBytes(file, Array.Empty<byte>());

        var parent = new Mock<IDirectoryInfo>(MockBehavior.Strict);

        _context = new Mock<IContext>(MockBehavior.Strict);
        _context
            .SetupGet(c => c.Path)
            .Returns(new FileSystemPath());

        _sut = new FileSystemFileInfo(parent.Object, new FileInfo(file));
    }

    [TearDown]
    public void AfterEachTest()
    {
        _directory.Dispose();
    }

    [Test]
    public void Rename()
    {
        var actual = _sut
            .Rename("new.txt", _context.Object)
            .ShouldBeOfType<FileSystemFileInfo>();

        actual.Name.ShouldBe("new.txt");
        actual.NativeFullName.ShouldBe(Path.Combine(_directory.Location, "new.txt"));

        FileAssert.Exists(actual.NativeFullName);
        FileAssert.DoesNotExist(_sut.NativeFullName);
    }

    [Test]
    public void RenameFileExists()
    {
        File.WriteAllBytes(Path.Combine(_directory.Location, "new.txt"), Array.Empty<byte>());

        Assert.Throws<IOException>(() => _sut.Rename("new.txt", _context.Object));

        FileAssert.Exists(_sut.NativeFullName);
    }
}