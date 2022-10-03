using System.Collections.Generic;
using System.IO.Compression;
using System.Management.Automation;

namespace ZipAsFolder.Internal;

internal sealed class ZipAsFolderProviderSettings
{
    private readonly List<string> _zipExtensions;

    public ZipAsFolderProviderSettings()
    {
        _zipExtensions = new List<string>();

        SetDefaultZipExtensions();
        CompressionLevel = CompressionLevel.Optimal;
    }

    public string[]? ZipExtensions
    {
        get => _zipExtensions.ToArray();
        set
        {
            _zipExtensions.Clear();

            if (value == null || value.Length == 0)
            {
                SetDefaultZipExtensions();
            }
            else
            {
                _zipExtensions.AddRange(value);
            }
        }
    }

    public CompressionLevel CompressionLevel { get; set; }

    public static ZipAsFolderProviderSettings FromPsVariable(PSVariableIntrinsics variable)
    {
        var result = variable.GetValue(Names.ProviderSettingsName) as ZipAsFolderProviderSettings;

        if (result == null)
        {
            result = new ZipAsFolderProviderSettings();
            variable.Set(Names.ProviderSettingsName, result);
        }

        return result;
    }

    internal IList<string> GetZipExtensions() => _zipExtensions;

    private void SetDefaultZipExtensions()
    {
        _zipExtensions.Clear();
        _zipExtensions.Add(".zip");
        _zipExtensions.Add(".nupkg");
    }
}