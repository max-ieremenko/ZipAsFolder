using System.Management.Automation;
using ZipAsFolder.Internal;
using ZipAsFolder.IO;

namespace ZipAsFolder;

public partial class ZipAsFolderProvider
{
    protected override void GetItem(string path)
    {
        GetLogger().WriteLine("GetItem path={0}", path);

        var item = GetProvider().GetItem(RootPath(path));
        if (item != null)
        {
            GetContext().WriteItem(item);
        }
    }

    protected override object GetItemDynamicParameters(string path) => null!;

    protected override bool ItemExists(string path)
    {
        GetLogger().WriteLine("ItemExists path={0}", path);

        var result = GetProvider().ItemExists(RootPath(path));

        GetLogger().WriteLine("    => {0}", result);
        return result;
    }

    protected override object ItemExistsDynamicParameters(string path) => null!;

    // https://docs.microsoft.com/en-us/powershell/scripting/developer/prog-guide/creating-a-windows-powershell-container-provider?view=powershell-7.2#retrieving-child-items
    protected override void GetChildItems(string path, bool recurse, uint depth)
    {
        GetLogger().WriteLine("GetChildItems path={0}; Filter={1}; recurse={2}; depth={3}", path, Filter, recurse, depth);

        WriteChildItems(RootPath(path), recurse ? depth : 1, false, ReturnContainers.ReturnMatchingContainers);
    }

    protected override void GetChildItems(string path, bool recurse)
    {
        GetLogger().WriteLine("GetChildItems path={0}; Filter={1}; recurse={2}", path, Filter, recurse);

        WriteChildItems(RootPath(path), recurse ? uint.MaxValue : 1, false, ReturnContainers.ReturnMatchingContainers);
    }

    protected override void GetChildNames(string path, ReturnContainers returnContainers)
    {
        GetLogger().WriteLine("GetChildNames path={0}; Filter={1}; returnContainers={1}", path, Filter, returnContainers);

        WriteChildItems(RootPath(path), 1, true, returnContainers);
    }

    protected override object GetChildNamesDynamicParameters(string path) => null!;

    protected override bool HasChildItems(string path)
    {
        GetLogger().WriteLine("HasChildItems path={0}", path);

        var result = GetProvider().HasChildItems(RootPath(path));

        GetLogger().WriteLine("    => {0}", result);
        return result;
    }

    protected override bool IsItemContainer(string path)
    {
        GetLogger().WriteLine("IsItemContainer path={0}", path);

        var result = GetProvider().IsItemContainer(RootPath(path));
        GetLogger().WriteLine("    => {0}", result);
        return result;
    }

    protected override object GetChildItemsDynamicParameters(string path, bool recurse)
    {
        return new GetChildDynamicParameters();
    }

    private void WriteChildItems(string path, uint depth, bool nameOnly, ReturnContainers returnContainers)
    {
        var fileFilter = WildcardPatternSearchFilter.FromPattern(Filter);
        var directoryFiler = returnContainers == ReturnContainers.ReturnAllContainers ? null : fileFilter;

        var attributes = GetChildDynamicParameters.FromObject(DynamicParameters).GetAttributes();

        GetProvider().WriteChildItems(path, depth, nameOnly, attributes, directoryFiler, fileFilter);
    }
}