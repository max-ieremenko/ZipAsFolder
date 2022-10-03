using ZipAsFolder.Internal;
using ZipAsFolder.IO;
using ZipAsFolder.Suite;

namespace ZipAsFolder;

public partial class ZipAsFolderProvider
{
    protected override void CopyItem(string path, string copyPath, bool recurse)
    {
        GetLogger().WriteLine(
            "CopyItem path={0}; copyPath={1}, recurse={2}, Force={3}, Filter={4}, Include={5}, Exclude={6}",
            path,
            copyPath,
            recurse,
            (bool)Force,
            Filter,
            Include.Count,
            Exclude.Count);

        var source = GetProvider().GetItem(RootPath(path));
        PathAssert.PathExists(source != null, path);

        source = GetProvider().TryDowngradeToFile(source);

        if (!GetContext().ShouldCopyItem(source, copyPath))
        {
            return;
        }

        if (source.AsDirectory(out var directory))
        {
            var fileFilter = WildcardPatternSearchFilter.FromPattern(Filter);
            GetProvider().CopyDirectory(directory, RootPath(copyPath), Force, recurse, fileFilter);
        }
        else
        {
            GetProvider().CopyFile((IFileInfo)source, RootPath(copyPath));
        }
    }

    protected override object CopyItemDynamicParameters(string path, string destination, bool recurse) => null!;
}