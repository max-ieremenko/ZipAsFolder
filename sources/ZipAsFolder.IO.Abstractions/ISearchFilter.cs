namespace ZipAsFolder.IO;

public interface ISearchFilter
{
    string Filter { get; }

    bool IsMatch(string input);
}