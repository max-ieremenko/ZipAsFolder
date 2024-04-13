using ZipAsFolder.IO;

namespace ZipAsFolder.Suite.Internal;

internal sealed class PathWalker : IPathWalker
{
    private readonly IFileSystemProvider _fileSystem;
    private readonly IContext _context;

    private int _pathIndex;

    public PathWalker(IFileSystemProvider fileSystem, IContext context, string path)
    {
        if (!context.Path.IsPathRooted(path))
        {
            throw new ArgumentOutOfRangeException(nameof(path), "path must be rooted.");
        }

        _fileSystem = fileSystem;
        _context = context;

        var items = SplitPath(path, context.Path.ItemSeparator);
        LastIsDirectory = IsLastDirectory(items);

        OptimizePath(items);

        Path = items;
        NextName = string.Empty;
    }

    public IFileSystemInfo? Current { get; private set; }

    public bool IsCompleted { get; private set; }

    public bool IsValid { get; private set; }

    public string NextName { get; private set; }

    public bool LastIsDirectory { get; }

    public bool NextIsLast => Path.Count == _pathIndex;

    internal List<string> Path { get; }

    public void SetNext(IFileSystemInfo next)
    {
        if (IsCompleted || !IsValid)
        {
            throw new InvalidOperationException();
        }

        Current = next;

        if (NextIsLast)
        {
            if (LastIsDirectory && !next.IsDirectory())
            {
                throw new ArgumentOutOfRangeException(nameof(next));
            }

            NextName = string.Empty;
            IsCompleted = true;
            return;
        }

        SetNextName();
        IsValid = Current.IsDirectory();
    }

    internal void Initialize()
    {
        if (Path.Count == 0)
        {
            return;
        }

        _pathIndex = 1;
        var parent = _fileSystem.GetRoot(Path[0]);
        if (parent == null)
        {
            return;
        }

        Current = parent;
        IsValid = true;

        if (Path.Count <= _pathIndex)
        {
            IsCompleted = true;
            return;
        }

        while (_pathIndex < Path.Count)
        {
            if (parent == null)
            {
                IsValid = false;
                return;
            }

            SetNextName();

            var next = _fileSystem.GetChild(parent, NextName, _context);
            if (next == null)
            {
                return;
            }

            Current = next;
            NextName = string.Empty;

            if (NextIsLast)
            {
                if (LastIsDirectory && !next.IsDirectory())
                {
                    IsValid = false;
                }

                IsCompleted = true;
                return;
            }

            parent = next as IDirectoryInfo;
        }
    }

    private static bool IsDirectory(string name)
    {
        var test = name[name.Length - 1];

        for (var i = 0; i < FileSystemPath.Separators.Length; i++)
        {
            if (test == FileSystemPath.Separators[i])
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsThisLocation(string name)
    {
        return name.EqualsIgnoreCase(".")
               || (name.Length == 2 && name[0] == '.' && IsDirectory(name));
    }

    private static bool IsParentLocation(string name)
    {
        return name.EqualsIgnoreCase("..")
               || (name.Length == 3 && name[0] == '.' && name[1] == '.' && IsDirectory(name));
    }

    private static List<string> SplitPath(string path, string itemSeparator)
    {
        var result = new List<string>();

        var rest = path.AsSpan();
        while (rest.Length > 0)
        {
            var index = rest.IndexOfAny(FileSystemPath.Separators);
            if (index < 0 || index == rest.Length - 1)
            {
                break;
            }

            if (index == 0)
            {
                rest = rest.Slice(1);
                result.Add(itemSeparator);
                continue;
            }

            var item = rest.Slice(0, index).ToString();
            result.Add(item);

            rest = rest.Slice(index + 1);
        }

        if (rest.Length > 0)
        {
            result.Add(rest.ToString());
        }

        return result;
    }

    private static bool IsLastDirectory(List<string> path)
    {
        if (path.Count == 0)
        {
            return false;
        }

        var last = path[path.Count - 1];
        if (!IsDirectory(last))
        {
            return false;
        }

        if (path.Count > 1)
        {
            path[path.Count - 1] = last.Substring(0, last.Length - 1);
        }

        return true;
    }

    private static void OptimizePath(List<string> path)
    {
        for (var i = 0; i < path.Count; i++)
        {
            var item = path[i];
            if (IsThisLocation(item))
            {
                path.RemoveAt(i);
                i--;

                continue;
            }

            if (IsParentLocation(item))
            {
                if (i < 2)
                {
                    path.Clear();
                    return;
                }

                path.RemoveAt(i);
                path.RemoveAt(i - 1);
                i -= 2;
            }
        }
    }

    private void SetNextName()
    {
        NextName = Path[_pathIndex];
        _pathIndex++;
    }
}