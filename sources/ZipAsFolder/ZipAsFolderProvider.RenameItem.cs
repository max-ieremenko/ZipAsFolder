using System;
using ZipAsFolder.IO;
using ZipAsFolder.Suite;

namespace ZipAsFolder;

public partial class ZipAsFolderProvider
{
    protected override void RenameItem(string path, string newName)
    {
        GetLogger().WriteLine("RenameItem path={0}; newName={1}", path, newName);

        var item = GetProvider().GetItem(RootPath(path));
        PathAssert.PathExists(item?.Parent != null, path);

        if (!string.IsNullOrEmpty(GetContext().Path.GetParentPath(newName)))
        {
            throw new InvalidOperationException("Cannot rename the specified target, because it represents a path or device name.");
        }

        if (item.Parent.GetChild(newName, GetContext()) != null)
        {
            throw new InvalidOperationException("Cannot create '{0}' because a file or directory with the same name already exists.".FormatWith(path));
        }

        if (!GetContext().ShouldRenameItem(item, newName))
        {
            return;
        }

        if (item.AsDirectory(out var directory))
        {
            var result = directory.Rename(newName, GetContext());
            GetContext().WriteItem(result);
        }
        else
        {
            var result = ((IFileInfo)item).Rename(newName, GetContext());
            GetContext().WriteItem(result);
        }
    }

    protected override object RenameItemDynamicParameters(string path, string newName) => null!;
}