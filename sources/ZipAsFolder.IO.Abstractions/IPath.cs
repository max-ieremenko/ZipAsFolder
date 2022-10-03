namespace ZipAsFolder.IO;

public interface IPath
{
    public string ItemSeparator { get; }

    bool IsPathRooted(string path);

    string NormalizePath(string path);

    string Combine(string? normalizedPath1, string normalizedPath2);

    bool IsValidName(string? name);

    string GetParentPath(string path);

    string GetChildName(string normalizedPath);
}