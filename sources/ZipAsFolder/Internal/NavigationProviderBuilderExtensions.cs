using System.Collections.Generic;
using System.IO.Compression;
using ZipAsFolder.Archive.Zip;
using ZipAsFolder.IO;
using ZipAsFolder.IO.Archive;
using ZipAsFolder.IO.FileSystem;
using ZipAsFolder.Suite;

namespace ZipAsFolder.Internal;

internal static class NavigationProviderBuilderExtensions
{
    public static readonly FileSystemPath Path = new();

    public static readonly FileSystem FileSystem = new();

    public static NavigationProviderBuilder WithFileSystem(this in NavigationProviderBuilder builder)
    {
        return builder.With(FileSystem);
    }

    public static NavigationProviderBuilder WithZipFileSystem(
        this in NavigationProviderBuilder builder,
        CompressionLevel compressionLevel,
        IList<string> extensions)
    {
        var system = new ArchiveFileSystem(new ZipContentProvider(compressionLevel), extensions);
        return builder.With(system);
    }
}