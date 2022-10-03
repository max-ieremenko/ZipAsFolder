using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Runtime.InteropServices;

namespace ZipAsFolder;

public partial class ZipAsFolderProvider
{
    protected override Collection<PSDriveInfo> InitializeDefaultDrives()
    {
        var root = string.Empty;
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            root = "zf:/";
        }

        var info = new PSDriveInfo(Names.PsDriveName, ProviderInfo, root, "the description", Credential, false);

        return new Collection<PSDriveInfo> { info };
    }

    protected override PSDriveInfo RemoveDrive(PSDriveInfo drive)
    {
        throw new NotSupportedException();
    }

    protected override PSDriveInfo NewDrive(PSDriveInfo drive)
    {
        if (!drive.Name.EqualsIgnoreCase(Names.PsDriveName) && drive.Provider.Name.EqualsIgnoreCase(ProviderInfo.Name))
        {
            throw new NotSupportedException();
        }

        return base.NewDrive(drive);
    }

    protected override object NewDriveDynamicParameters() => null!;
}