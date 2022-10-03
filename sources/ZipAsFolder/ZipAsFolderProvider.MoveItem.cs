using ZipAsFolder.Internal;
using ZipAsFolder.IO;
using ZipAsFolder.Suite;

namespace ZipAsFolder;

public partial class ZipAsFolderProvider
{
    protected override void MoveItem(string path, string destination)
    {
        GetLogger().WriteLine(
            "MoveItem path={0}; destination={1}, Force={2}, Filter={3}, Include={4}, Exclude={5}",
            path,
            destination,
            (bool)Force,
            Filter,
            Include.Count,
            Exclude.Count);

        var source = GetProvider().GetItem(RootPath(path));
        PathAssert.PathExists(source != null, path);

        source = GetProvider().TryDowngradeToFile(source);

        var destinationPath = RootPath(destination);

        if (!GetContext().ShouldMoveItem(source, destinationPath))
        {
            return;
        }

        if (source.AsDirectory(out var directory))
        {
            var fileFilter = WildcardPatternSearchFilter.FromPattern(Filter);
            GetProvider().MoveDirectory(directory, destinationPath, Force, fileFilter);
        }
        else
        {
            GetProvider().MoveFile((IFileInfo)source, Force, destinationPath);
        }
    }

    protected override object MoveItemDynamicParameters(string path, string destination) => null!;
}