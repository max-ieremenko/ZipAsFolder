using System;
using System.IO;
using NUnit.Framework;
using Shouldly;
using ZipAsFolder.TestApi;

namespace ZipAsFolder.IO.FileSystem;

[TestFixture]
public class FileSystemTest
{
    private TempDirectory _directory = null!;
    private FileSystem _sut = null!;

    [SetUp]
    public void BeforeEachTest()
    {
        _directory = new TempDirectory();
        _sut = new FileSystem();
    }

    [TearDown]
    public void AfterEachTest()
    {
        _directory.Dispose();
    }

    [Test]
    [Platform("Win")]
    public void TryGetRootWindows()
    {
        var path = Directory.GetCurrentDirectory();
        if (!Path.IsPathFullyQualified(path))
        {
            path = Path.Combine(AppContext.BaseDirectory, path);
        }

        var drive = path.Substring(0, path.IndexOf(':') + 1);

        // ensure that actual is drive, not current directory
        _sut.TryGetRoot(drive, out var actual).ShouldBeTrue();

        actual!.Name.ShouldBe(drive + '\\', StringCompareShould.IgnoreCase);
        actual.FullName.ShouldBe("zf:" + drive + '\\', StringCompareShould.IgnoreCase);
    }

    [Test]
    [Platform(Exclude = "Win")]
    public void TryGetRootLinux()
    {
        _sut.TryGetRoot("/", out var actual).ShouldBeTrue();

        actual!.Name.ShouldBe("/");
        actual.FullName.ShouldBe("zf:/");
    }
}