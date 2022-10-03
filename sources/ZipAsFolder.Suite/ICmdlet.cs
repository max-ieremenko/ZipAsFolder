using ZipAsFolder.IO;

namespace ZipAsFolder.Suite;

public interface ICmdlet
{
    void PerformNewItem(IFileSystemInfo item);

    void WriteItem(IFileSystemInfo item, bool nameOnly = false);
}