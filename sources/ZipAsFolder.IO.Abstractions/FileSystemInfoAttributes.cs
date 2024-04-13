namespace ZipAsFolder.IO;

[Flags]
public enum FileSystemInfoAttributes
{
    Directory = 1,
    File = 2,
    Archive = 4
}