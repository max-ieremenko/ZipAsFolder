using System;
using NUnit.Framework;
using Shouldly;

namespace ZipAsFolder;

[TestFixture]
public class EnumExtensionsTest
{
    [Flags]
    public enum SomeEnum
    {
        Directory = 1,
        File = 2,
        Archive = 4
    }

    [Test]
    [TestCase(SomeEnum.Directory, SomeEnum.Directory, true)]
    [TestCase(SomeEnum.File, SomeEnum.Directory, false)]
    [TestCase(SomeEnum.Directory | SomeEnum.Archive, SomeEnum.Directory, true)]
    [TestCase(SomeEnum.Directory | SomeEnum.Archive, SomeEnum.Archive, true)]
    [TestCase(SomeEnum.Directory | SomeEnum.Archive, SomeEnum.File, false)]
    [TestCase(SomeEnum.File | SomeEnum.Archive, SomeEnum.File | SomeEnum.Directory, true)]
    public void ContainsFlag(SomeEnum value, SomeEnum flag, bool expected)
    {
        value.ContainsFlag(flag).ShouldBe(expected);
    }
}