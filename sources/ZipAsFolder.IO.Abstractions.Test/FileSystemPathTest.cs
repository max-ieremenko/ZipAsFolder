namespace ZipAsFolder.IO;

[TestFixture]
public class FileSystemPathTest
{
    private FileSystemPath _sut = null!;

    [SetUp]
    public void BeforeEachTest()
    {
        _sut = new FileSystemPath("/");
    }

    [Test]
    [TestCase("11", "11")]
    [TestCase("11\\", "11/")]
    [TestCase("zf:11", "zf:11")]
    [TestCase("zf:11\\", "zf:11/")]
    [TestCase("zf:11\\22", "zf:11/22")]
    [TestCase("zf://11\\\\22//", "zf:/11/22/")]
    public void NormalizePath(string path, string expected)
    {
        _sut.NormalizePath(path).ShouldBe(expected);
    }

    [Test]
    [TestCase(null, false)]
    [TestCase("  ", false)]
    [TestCase("  1", true)]
    [TestCase("1/", false)]
    [TestCase("1*", false)]
    public void IsValidName(string name, bool expected)
    {
        _sut.IsValidName(name).ShouldBe(expected);
    }

    [Test]
    [TestCase("", "", "")]
    [TestCase("a", "b", "a/b")]
    [TestCase("a/", "b", "a/b")]
    [TestCase("a", "b/", "a/b/")]
    [TestCase("a/", "b/", "a/b/")]
    [TestCase("zf:/a", "zf:/b", "zf:/a/b")]
    [TestCase("zf:a", "zf:/b", "zf:a/b")]
    [TestCase("/a", "zf:/b", "zf:/a/b")]
    [TestCase("zf:a", "zf:/b/", "zf:a/b/")]
    public void Combine(string path1, string path2, string expected)
    {
        _sut.Combine(path1, path2).ShouldBe(expected);
    }

    [Test]
    [TestCase("", "")]
    [TestCase("c:/", "")]
    [TestCase("c:/a", "c:/")]
    [TestCase("c:/a/..", "c:/a")]
    [TestCase("a", "")]
    [TestCase("/a", "/")]
    [TestCase("/", "")]
    [TestCase("a/b", "a")]
    [TestCase("/a/b", "/a")]
    [TestCase("/a/b/", "/a")]
    [TestCase("zf:", "")]
    [TestCase("zf:/", "")]
    [TestCase("zf:a", "")]
    [TestCase("zf:/a", "zf:/")]
    [TestCase("zf:a/b", "zf:a")]
    [TestCase("zf:/a/b", "zf:/a")]
    public void GetParentPath(string path, string expected)
    {
        _sut.GetParentPath(path).ShouldBe(expected);
    }

    [Test]
    [TestCase("", "")]
    [TestCase("a", "a")]
    [TestCase("/a", "a")]
    [TestCase("/a/b", "b")]
    [TestCase("zf:", "")]
    [TestCase("zf:a", "a")]
    [TestCase("zf:/", "")]
    [TestCase("zf:/a", "a")]
    [TestCase("zf:a/b", "b")]
    public void GetChildName(string path, string expected)
    {
        _sut.GetChildName(path).ShouldBe(expected);
    }
}