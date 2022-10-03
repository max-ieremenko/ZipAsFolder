using System;
using Moq;
using NUnit.Framework;
using Shouldly;
using ZipAsFolder.Archive;

namespace ZipAsFolder.IO.Archive;

[TestFixture]
public class ArchiveFileSystemTest
{
    private Mock<IArchiveContentProvider> _contentProvider = null!;
    private ArchiveFileSystem _sut = null!;

    [SetUp]
    public void BeforeEachTest()
    {
        _contentProvider = new Mock<IArchiveContentProvider>(MockBehavior.Strict);

        _sut = new ArchiveFileSystem(_contentProvider.Object, new[] { ".zip" });
    }

    [Test]
    public void GetDirectoryFromFile()
    {
        var parent = new Mock<IDirectoryInfo>(MockBehavior.Strict);

        var file = new Mock<IFileInfo>(MockBehavior.Strict);
        file
            .SetupGet(f => f.Extension)
            .Returns(".ZIP");
        file
            .SetupGet(f => f.Parent)
            .Returns(parent.Object);
        file
            .SetupGet(f => f.Name)
            .Returns("content.zip");
        file
            .SetupGet(f => f.FullName)
            .Returns("/path/content.zip");
        file
            .SetupGet(f => f.Length)
            .Returns(1);
        file
            .SetupGet(f => f.LastWriteTime)
            .Returns(DateTime.Now);

        var actual = _sut.AsDirectory(file.Object);

        var zip = actual.ShouldBeOfType<ArchiveFile>();
        zip.Name.ShouldBe("content.zip");
        zip.Parent.ShouldBe(parent.Object);
        zip.FullName.ShouldBe("zf:/path/content.zip");
    }

    [Test]
    public void GetDirectoryFromTextFile()
    {
        var file = new Mock<IFileInfo>(MockBehavior.Strict);
        file
            .SetupGet(f => f.Name)
            .Returns("11.txt");

        _sut.AsDirectory(file.Object).ShouldBeNull();
    }
}