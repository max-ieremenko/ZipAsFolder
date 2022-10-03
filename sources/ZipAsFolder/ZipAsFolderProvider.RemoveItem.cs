using ZipAsFolder.IO;
using ZipAsFolder.Suite;

namespace ZipAsFolder;

public partial class ZipAsFolderProvider
{
    protected override void RemoveItem(string path, bool recurse)
    {
        GetLogger().WriteLine("RemoveItem path={0}; recurse={1}", path, recurse);

        var item = GetProvider().GetItem(RootPath(path));
        PathAssert.PathExists(item != null, path);

        item = GetProvider().TryDowngradeToFile(item);

        if (!GetContext().ShouldRemoveItem(item))
        {
            return;
        }

        if (item.AsDirectory(out var directory))
        {
            directory.Delete(GetContext());
        }
        else
        {
            ((IFileInfo)item).Delete();
        }
    }

    protected override object RemoveItemDynamicParameters(string path, bool recurse) => null!;
}