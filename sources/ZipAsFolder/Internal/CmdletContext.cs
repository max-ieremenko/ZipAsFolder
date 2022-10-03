using System.Management.Automation.Provider;
using System.Threading;
using ZipAsFolder.IO;
using ZipAsFolder.Suite;

namespace ZipAsFolder.Internal;

internal sealed class CmdletContext : IContext, ICmdlet
{
    private readonly CmdletProvider _cmdlet;
    private readonly CancellationTokenSource _cancellationSource;

    public CmdletContext(CmdletProvider cmdlet, IPath path, ZipAsFolderProviderSettings settings)
    {
        Path = path;
        Settings = settings;
        _cmdlet = cmdlet;
        _cancellationSource = new();
    }

    public ZipAsFolderProviderSettings Settings { get; }

    public IPath Path { get; }

    public CancellationToken Token => _cancellationSource.Token;

    public void WriteItem(IFileSystemInfo item, bool nameOnly = false)
    {
        _cmdlet.WriteItemObject(nameOnly ? item.Name : item, item.FullName, item.IsDirectory());
    }

    public void PerformNewItem(IFileSystemInfo item)
    {
        if (item.IsDirectory())
        {
            ShouldNewDirectoryItem(item.FullName);
        }
        else
        {
            ShouldNewFileItem(item.FullName);
        }
    }

    internal bool ShouldRenameItem(IFileSystemInfo item, string newName)
    {
        var target = "Item: {0} Destination: {1}".FormatWith(item.FullName, newName);
        var action = item.IsDirectory() ? "Rename Directory" : "Rename File";

        return _cmdlet.ShouldProcess(target, action);
    }

    internal bool ShouldRemoveItem(IFileSystemInfo item)
    {
        var target = "Item: {0}".FormatWith(item.FullName);
        var action = item.IsDirectory() ? "Remove Directory" : "Remove File";

        return _cmdlet.ShouldProcess(target, action);
    }

    internal bool ShouldNewDirectoryItem(string itemPath)
    {
        var target = "Destination: {0}".FormatWith(itemPath);

        return _cmdlet.ShouldProcess(target, "Create Directory");
    }

    internal bool ShouldNewFileItem(string itemPath)
    {
        var target = "Destination: {0}".FormatWith(itemPath);

        return _cmdlet.ShouldProcess(target, "Create File");
    }

    internal bool ShouldMoveItem(IFileSystemInfo item, string destination)
    {
        var target = "Item: {0} Destination: {1}".FormatWith(item.FullName, destination);
        var action = item.IsDirectory() ? "Move Directory" : "Move File";

        return _cmdlet.ShouldProcess(target, action);
    }

    internal bool ShouldCopyItem(IFileSystemInfo item, string copyPath)
    {
        var target = "Item: {0} Destination: {1}".FormatWith(item.FullName, copyPath);
        var action = item.IsDirectory() ? "Copy Directory" : "Copy File";

        return _cmdlet.ShouldProcess(target, action);
    }

    internal bool ShouldClearContent(IFileSystemInfo item)
    {
        var target = "Item: {0}".FormatWith(item.FullName);

        return _cmdlet.ShouldProcess(target, "Clear Content");
    }

    internal void Cancel() => _cancellationSource.Cancel();
}