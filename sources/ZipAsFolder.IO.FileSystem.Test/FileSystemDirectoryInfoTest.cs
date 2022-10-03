using System;
using System.IO;
using Moq;
using NUnit.Framework;
using Shouldly;
using ZipAsFolder.TestApi;

namespace ZipAsFolder.IO.FileSystem;

[TestFixture]
public class FileSystemDirectoryInfoTest
{
    private TempDirectory _temp = null!;
    private Mock<IContext> _context = null!;
    private FileSystemDirectoryInfo _sut = null!;

    [SetUp]
    public void BeforeEachTest()
    {
        _temp = new TempDirectory();
        _sut = new FileSystemDirectoryInfo(null, new DirectoryInfo(_temp.Location));

        _context = new Mock<IContext>(MockBehavior.Strict);
        _context
            .SetupGet(c => c.Path)
            .Returns(new FileSystemPath());
    }

    [TearDown]
    public void AfterEachTest()
    {
        _temp.Dispose();
    }

    [Test]
    [TestCase("file.txt", "File.TXT", true)]
    [TestCase(null, "not-found.txt", false)]
    [TestCase("file.txt", "*", false)]
    public void GetChild(string? fileName, string childName, bool expected)
    {
        if (fileName != null)
        {
            File.Create(Path.Combine(_temp.Location, fileName)).Dispose();
        }

        var actual = _sut.GetChild(childName, _context.Object);

        if (expected)
        {
            actual.ShouldNotBeNull();
            actual.Name.ShouldBe(fileName);
            actual.Parent.ShouldBe(_sut);
        }
        else
        {
            actual.ShouldBeNull();
        }
    }

    [Test]
    public void Rename()
    {
        var newName = Guid.NewGuid().ToString();

        var actual = _sut.Rename(newName, _context.Object).ShouldBeOfType<FileSystemDirectoryInfo>();

        actual.Name.ShouldBe(newName);

        DirectoryAssert.Exists(actual.NativeFullName);
        DirectoryAssert.DoesNotExist(_sut.NativeFullName);

        _temp = new TempDirectory(actual.NativeFullName);
    }

    [Test]
    public void RenameDirectoryExists()
    {
        using var existing = new TempDirectory();

        Assert.Throws<IOException>(() => _sut.Rename(Path.GetFileName(existing.Location), _context.Object));

        DirectoryAssert.Exists(_sut.NativeFullName);
    }
}