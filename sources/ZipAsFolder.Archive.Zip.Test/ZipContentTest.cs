using System.IO;
using System.IO.Compression;
using System.Linq;
using NUnit.Framework;
using Shouldly;
using ZipAsFolder.TestApi;

namespace ZipAsFolder.Archive.Zip;

[TestFixture]
public class ZipContentTest
{
    private ZipContent _sut = null!;
    private TempFile _zip = null!;

    [SetUp]
    public void BeforeEachTest()
    {
        _zip = TempFile.FromResources("Content.zip");

        var stream = new FileStream(_zip.Location, FileMode.Open, FileAccess.ReadWrite);
        var archive = new ZipArchive(stream, ZipArchiveMode.Update, leaveOpen: false);
        _sut = new ZipContent(archive, CompressionLevel.Optimal);
    }

    [TearDown]
    public void AfterEachTest()
    {
        _sut.Dispose();
        _zip.Dispose();
    }

    [Test]
    public void GetEntries()
    {
        var entries = _sut.GetEntries().ToArray();

        entries.Length.ShouldBe(7);

        var entry = entries
            .SingleOrDefault(i => i.Path.Length == 1 && i.Path[0] == "11.txt")
            .ShouldBeOfType<ZipEntry>();

        entry.Length.ShouldBe(2);
        entry.IsFile.ShouldBeTrue();

        entry = entries
            .SingleOrDefault(i => i.Path.Length == 2 && i.Path[0] == "2" && i.Path[1] == "2.2")
            .ShouldBeOfType<ZipEntry>();

        entry.Length.ShouldBe(0);
        entry.IsFile.ShouldBeFalse();
    }

    [Test]
    public void OpenRead()
    {
        using var actual = _sut.OpenRead(new[] { "11.txt" });

        actual.CanRead.ShouldBeTrue();
        actual.Read(new byte[1], 0, 1).ShouldBe(1);
    }

    [Test]
    public void OpenReadNested()
    {
        using var inner = _sut.OpenRead(new[] { "inner.zip" });
        using var nested = new ZipContent(new ZipArchive(inner, ZipArchiveMode.Read, leaveOpen: false), CompressionLevel.Optimal);

        using var actual = nested.OpenRead(new[] { "11.txt" });

        actual.CanRead.ShouldBeTrue();
        actual.Read(new byte[1], 0, 1).ShouldBe(1);
    }

    [Test]
    public void OpenWrite()
    {
        using var actual = _sut.OpenWrite(new[] { "11.txt" });

        actual.CanWrite.ShouldBeTrue();
        actual.SetLength(0);
    }

    [Test]
    public void OpenWriteNested()
    {
        using var inner = _sut.OpenRead(new[] { "inner.zip" });
        using var nested = new ZipContent(new ZipArchive(inner, ZipArchiveMode.Update, leaveOpen: false), CompressionLevel.Optimal);

        using var actual = nested.OpenRead(new[] { "11.txt" });

        actual.CanWrite.ShouldBeTrue();
        actual.SetLength(0);
    }

    [Test]
    [TestCase(new string[0], "22.txt")]
    [TestCase(new[] { "1" }, "22.txt")]
    public void CreateFileEntry(string[] path, string name)
    {
        var actual = _sut.CreateFileEntry(path, name).ShouldBeOfType<ZipEntry>();

        var fullName = path.Concat(new[] { name }).ToArray();

        actual.IsFile.ShouldBeTrue();
        actual.Path.ShouldBe(fullName);
        actual.Length.ShouldBe(0);

        _sut.OpenRead(fullName).Dispose();
    }

    [Test]
    [TestCase(new string[0], "22.txt")]
    [TestCase(new[] { "1" }, "22.txt")]
    public void CreateFileEntryNested(string[] path, string name)
    {
        using var inner = _sut.OpenRead(new[] { "inner.zip" });
        using var nested = new ZipContent(new ZipArchive(inner, ZipArchiveMode.Update, leaveOpen: false), CompressionLevel.Optimal);

        var actual = nested.CreateFileEntry(path, name).ShouldBeOfType<ZipEntry>();

        var fullName = path.Concat(new[] { name }).ToArray();

        actual.IsFile.ShouldBeTrue();
        actual.Path.ShouldBe(fullName);
        actual.Length.ShouldBe(0);

        nested.OpenRead(fullName).Dispose();
    }

    [Test]
    [TestCase(new string[0], "folder")]
    [TestCase(new[] { "1" }, "folder")]
    public void CreateDirectoryEntry(string[] path, string name)
    {
        var actual = _sut.CreateDirectoryEntry(path, name);

        var fullName = path.Concat(new[] { name }).ToArray();

        actual.IsFile.ShouldBeFalse();
        actual.Path.ShouldBe(fullName);
        actual.Length.ShouldBe(0);

        var entries = _sut.GetEntries().Where(i => i.Path.SequenceEqual(fullName)).ToArray();
        entries.Length.ShouldBe(1);
    }

    [Test]
    [TestCase(new string[0], "folder")]
    [TestCase(new[] { "1" }, "folder")]
    public void CreateDirectoryEntryNested(string[] path, string name)
    {
        using var inner = _sut.OpenRead(new[] { "inner.zip" });
        using var nested = new ZipContent(new ZipArchive(inner, ZipArchiveMode.Update, leaveOpen: false), CompressionLevel.Optimal);

        var actual = nested.CreateDirectoryEntry(path, name).ShouldBeOfType<ZipEntry>();

        actual.IsFile.ShouldBeFalse();
        actual.Path.ShouldBe(path.Concat(new[] { name }));
        actual.Length.ShouldBe(0);
    }

    [Test]
    [TestCase("11.txt")]
    [TestCase("2", "2.2", "2.2.txt")]
    public void RemoveFileEntry(params string[] path)
    {
        _sut.GetEntries().ShouldContain(i => i.Path.SequenceEqual(path));

        _sut.RemoveFileEntry(path);

        _sut.GetEntries().ShouldNotContain(i => i.Path.SequenceEqual(path));
    }

    [Test]
    [TestCase("11.txt")]
    [TestCase("2", "22.txt")]
    public void RemoveFileEntryNested(params string[] path)
    {
        using var inner = _sut.OpenRead(new[] { "inner.zip" });
        using var nested = new ZipContent(new ZipArchive(inner, ZipArchiveMode.Update, leaveOpen: false), CompressionLevel.Optimal);

        nested.GetEntries().ShouldContain(i => i.Path.SequenceEqual(path));

        nested.RemoveFileEntry(path);

        nested.GetEntries().ShouldNotContain(i => i.Path.SequenceEqual(path));
    }
}