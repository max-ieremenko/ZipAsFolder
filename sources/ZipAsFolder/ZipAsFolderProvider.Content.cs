using System.Management.Automation.Provider;
using ZipAsFolder.Internal;
using ZipAsFolder.Suite;

namespace ZipAsFolder;

public partial class ZipAsFolderProvider
{
    public void ClearContent(string path)
    {
        GetLogger().WriteLine("ClearContent path={0}", path);

        var target = GetProvider().GetItem(RootPath(path));
        PathAssert.PathExists(target != null, path);

        if (GetContext().ShouldClearContent(target))
        {
            GetProvider().ClearContent(target);
        }
    }

    public object ClearContentDynamicParameters(string path) => null!;

    public IContentReader GetContentReader(string path)
    {
        GetLogger().WriteLine("GetContentReader path={0}", path);

        var target = GetProvider().GetItem(RootPath(path));
        if (target == null)
        {
            return null!;
        }

        var options = ContentReaderDynamicParameters.FromObject(DynamicParameters);
        var reader = GetProvider().CreateTextReader(target, options.Raw, options.Encoding);
        return new ContentReader(reader, GetContext().Token);
    }

    public object GetContentReaderDynamicParameters(string path) => new ContentReaderDynamicParameters();

    public IContentWriter GetContentWriter(string path)
    {
        GetLogger().WriteLine("GetContentWriter path={0}", path);

        path = RootPath(path);
        var target = GetProvider().GetItem(path);
        if (target == null)
        {
            target = GetProvider().CreateFile(path, false, false);
            GetContext().PerformNewItem(target);
        }

        var options = ContentWriterDynamicParameters.FromObject(DynamicParameters);
        var writer = GetProvider().CreateTextWriter(target, options.NoNewline, options.Encoding);
        return new ContentWriter(writer, GetContext().Token);
    }

    public object GetContentWriterDynamicParameters(string path) => new ContentWriterDynamicParameters();
}