using System;
using System.Collections.Generic;
using System.IO;
using Moq;
using NUnit.Framework;
using Shouldly;
using ZipAsFolder.Archive;

namespace ZipAsFolder.IO.Archive.Internal;

[TestFixture]
public class EntryReaderTest
{
    private Mock<IArchive> _archive = null!;
    private List<IArchiveEntry> _entries = null!;
    private ArchiveDirectoryInfo _root = null!;
    private Mock<IContext> _context = null!;
    private Mock<IPath> _path = null!;

    [SetUp]
    public void BeforeEachTest()
    {
        _entries = new List<IArchiveEntry>();

        var archiveContent = new Mock<IArchiveContent>(MockBehavior.Strict);
        archiveContent
            .Setup(c => c.GetEntries())
            .Returns(_entries);
        archiveContent
            .Setup(c => c.Dispose());

        _archive = new Mock<IArchive>(MockBehavior.Strict);
        _archive
            .Setup(a => a.OpenRead())
            .Returns(archiveContent.Object);

        _path = new Mock<IPath>(MockBehavior.Strict);
        _path
            .Setup(p => p.Combine(It.IsAny<string>(), It.IsAny<string>()))
            .Returns<string, string>((path1, path2) => path1 + "/" + path2);

        _context = new Mock<IContext>(MockBehavior.Strict);

        _root = new ArchiveDirectoryInfo(_archive.Object, Array.Empty<string>(), null, "name", "root-path", null, null);
    }

    [Test]
    public void ReadEmpty()
    {
        new EntryReader(_archive.Object, _path.Object).Read(_root);

        _root.IsEmpty(_context.Object).ShouldBeTrue();
    }

    [Test]
    public void ReadFile()
    {
        _entries.Add(new StubArchiveEntry
        {
            Path = new[] { "name.txt" },
            FullName = "full-name.txt",
            Length = 1,
            IsFile = true,
            LastWriteTime = DateTime.Today
        });

        new EntryReader(_archive.Object, _path.Object).Read(_root);

        _root.FolderByName.ShouldBeEmpty();
        _root.FileByName.Keys.ShouldBe(new[] { "name.txt" });

        var file = _root.FileByName["name.txt"];
        file.Length.ShouldBe(1);
        file.LastWriteTime.ShouldBe(DateTime.Today);
        file.FullName.ShouldBe("zf:root-path/name.txt");
        file.Name.ShouldBe("name.txt");
        file.Parent.ShouldBe(_root);
    }

    [Test]
    public void ReadFolder()
    {
        _entries.Add(new StubArchiveEntry
        {
            Path = new[] { "name" },
            FullName = "full-name",
            Length = 1,
            IsFile = false,
            LastWriteTime = DateTime.Today
        });

        new EntryReader(_archive.Object, _path.Object).Read(_root);

        _root.FolderByName.Keys.ShouldBe(new[] { "name" });
        _root.FileByName.ShouldBeEmpty();

        var folder = _root.FolderByName["name"];
        folder.ArchivePath.ShouldBe(new[] { "name" });
        folder.Length.ShouldBe(1);
        folder.LastWriteTime.ShouldBe(DateTime.Today);
        folder.FullName.ShouldBe("zf:root-path/name");
        folder.Name.ShouldBe("name");
        folder.Parent.ShouldBe(_root);
    }

    [Test]
    public void ReadFileFolderIsNotDefined()
    {
        _entries.Add(new StubArchiveEntry
        {
            Path = new[] { "folder", "name.txt" },
            FullName = "folder/name.txt",
            Length = 1,
            IsFile = true,
            LastWriteTime = DateTime.Today
        });

        new EntryReader(_archive.Object, _path.Object).Read(_root);

        _root.FolderByName.Keys.ShouldBe(new[] { "folder" });
        _root.FileByName.ShouldBeEmpty();

        var folder = _root.FolderByName["folder"];
        folder.Length.ShouldBeNull();
        folder.LastWriteTime.ShouldBeNull();
        folder.FullName.ShouldBe("zf:root-path/folder");
        folder.Name.ShouldBe("folder");
        folder.Parent.ShouldBe(_root);

        folder.FolderByName.ShouldBeEmpty();
        folder.ArchivePath.ShouldBe(new[] { "folder" });
        folder.FileByName.Keys.ShouldBe(new[] { "name.txt" });
        folder.FileByName["name.txt"].Parent.ShouldBe(folder);
    }

    [Test]
    public void ReadFileFolderIsDefined()
    {
        _entries.Add(new StubArchiveEntry
        {
            Path = new[] { "folder", "name.txt" },
            FullName = "folder/name.txt",
            Length = 1,
            IsFile = true,
            LastWriteTime = DateTime.Today
        });

        _entries.Add(new StubArchiveEntry
        {
            Path = new[] { "folder" },
            FullName = "folder",
            Length = 1,
            IsFile = false,
            LastWriteTime = DateTime.Today
        });

        new EntryReader(_archive.Object, _path.Object).Read(_root);

        _root.FolderByName.Keys.ShouldBe(new[] { "folder" });
        _root.FileByName.ShouldBeEmpty();

        var folder = _root.FolderByName["folder"];
        folder.ArchivePath.ShouldBe(new[] { "folder" });
        folder.Length.ShouldBe(1);
        folder.LastWriteTime.ShouldBe(DateTime.Today);
        folder.FullName.ShouldBe("zf:root-path/folder");
        folder.Name.ShouldBe("folder");
        folder.Parent.ShouldBe(_root);

        folder.FolderByName.ShouldBeEmpty();
        folder.FileByName.Keys.ShouldBe(new[] { "name.txt" });
        folder.FileByName["name.txt"].Parent.ShouldBe(folder);
    }
}