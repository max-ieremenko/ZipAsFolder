using System.IO;
using ZipAsFolder.IO;

namespace ZipAsFolder;

public partial class ZipAsFolderProvider
{
    private string CurrentDirectory => SessionState.Path.CurrentFileSystemLocation.Path;

    protected override string MakePath(string parent, string child)
    {
        GetLogger().WriteLine("MakePath parent={0}; child={1}", parent, child);

        string result;
        var normalizeChild = GetContext().Path.NormalizePath(FileSystemPath.FromPsDrivePath(child));
        if (GetContext().Path.IsPathRooted(normalizeChild))
        {
            result = child;
        }
        else
        {
            result = GetContext().Path.Combine(GetContext().Path.NormalizePath(parent), GetContext().Path.NormalizePath(child));
        }

        var test = base.MakePath(parent, child);

        GetLogger().WriteLine("    => {0} test {1}", result, test);
        return result;
    }

    protected override string NormalizeRelativePath(string path, string basePath)
    {
        GetLogger().WriteLine("NormalizeRelativePath path={0}; basePath={1}", path, basePath);
        var result = base.NormalizeRelativePath(path, basePath);

        GetLogger().WriteLine("    => {0}", result);
        return result;
    }

    protected override bool IsValidPath(string path)
    {
        GetLogger().WriteLine("IsValidPath path={0}", path);
        return !string.IsNullOrWhiteSpace(path);
    }

    protected override bool ConvertPath(string path, string filter, ref string updatedPath, ref string updatedFilter)
    {
        GetLogger().WriteLine("ConvertPath path={0}; filter={1}", path, filter);
        var result = base.ConvertPath(path, filter, ref updatedPath, ref updatedFilter);

        GetLogger().WriteLine("    => {0}; updatedPath={1}; updatedFilter={2}", result, updatedPath, updatedFilter);
        return result;
    }

    protected override string[] ExpandPath(string path)
    {
        GetLogger().WriteLine("ExpandPath path={0}", path);
        var result = base.ExpandPath(path);

        GetLogger().WriteLine("    => {0}", result);
        return result;
    }

    protected override string GetParentPath(string path, string root)
    {
        GetLogger().WriteLine("GetParentPath path={0}; root={1}", path, root);

        var result = GetContext().Path.GetParentPath(GetContext().Path.NormalizePath(path));

        GetLogger().WriteLine("    => {0}", result);
        return result;
    }

    protected override string GetChildName(string path)
    {
        GetLogger().WriteLine("GetChildName path={0}", path);

        var result = GetContext().Path.GetChildName(GetContext().Path.NormalizePath(path));

        GetLogger().WriteLine("    => {0}", result);
        return result;
    }

    private string RootPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return CurrentDirectory;
        }

        path = FileSystemPath.FromPsDrivePath(path);

        if (Path.IsPathFullyQualified(path))
        {
            return path;
        }

        return Path.Combine(CurrentDirectory, path);
    }
}