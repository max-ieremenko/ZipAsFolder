using System.Management.Automation;
using ZipAsFolder.IO;

namespace ZipAsFolder.Internal;

internal sealed class GetChildDynamicParameters
{
    private bool? _directory;
    private bool? _file;

    [Parameter]
    [Alias("ad")]
    public SwitchParameter Directory
    {
        get => _directory ?? true;
        set => _directory = value;
    }

    [Parameter]
    [Alias("af")]
    public SwitchParameter File
    {
        get => _file ?? true;
        set => _file = value;
    }

    public static GetChildDynamicParameters FromObject(object? other)
    {
        if (other is GetChildDynamicParameters result)
        {
            return result;
        }

        return new GetChildDynamicParameters();
    }

    internal FileSystemInfoAttributes GetAttributes()
    {
        if (!_directory.HasValue && !_file.HasValue)
        {
            return FileSystemInfoAttributes.File | FileSystemInfoAttributes.Directory;
        }

        var result = 0;

        if (_file == true)
        {
            result |= (int)FileSystemInfoAttributes.File;
        }

        if (_directory == true)
        {
            result |= (int)FileSystemInfoAttributes.Directory;
        }

        return (FileSystemInfoAttributes)result;
    }
}