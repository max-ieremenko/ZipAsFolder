using System.IO;

namespace ZipAsFolder.IO.FileSystem;

internal sealed class FileSystemFileInfo : FileInfoBase
{
    public FileSystemFileInfo(
        IDirectoryInfo parent,
        FileInfo info)
        : base(parent, info.Name, info.FullName, info.Length, info.LastWriteTime)
    {
        NativeFullName = info.FullName;
    }

    internal string NativeFullName { get; }

    public override Stream OpenRead()
    {
        return new FileStream(NativeFullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
    }

    public override Stream OpenWrite()
    {
        return new FileStream(NativeFullName, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
    }

    public override void Delete()
    {
        File.Delete(NativeFullName);
    }

    public override IFileInfo CopyTo(IDirectoryInfo destinationParent, IFileInfo? destination, string name, IContext context)
    {
        if (destination is FileSystemFileInfo file)
        {
            File.Copy(NativeFullName, file.NativeFullName, true);

            return new FileSystemFileInfo(destinationParent, new FileInfo(file.NativeFullName));
        }

        if (destination == null && destinationParent is FileSystemDirectoryInfo directory)
        {
            var destinationFullName = context.Path.Combine(directory.NativeFullName, name);
            File.Copy(NativeFullName, destinationFullName, false);

            return new FileSystemFileInfo(destinationParent, new FileInfo(destinationFullName));
        }

        return base.CopyTo(destinationParent, destination, name, context);
    }

    public override IFileInfo MoveTo(IDirectoryInfo destinationParent, IFileInfo? destination, string name, IContext context)
    {
        if (destination is FileSystemFileInfo file)
        {
            file.Delete();
            File.Move(NativeFullName, file.NativeFullName);

            return new FileSystemFileInfo(destinationParent, new FileInfo(file.NativeFullName));
        }

        if (destination == null && destinationParent is FileSystemDirectoryInfo directory)
        {
            var destinationFullName = context.Path.Combine(directory.NativeFullName, name);
            File.Move(NativeFullName, destinationFullName);

            return new FileSystemFileInfo(destinationParent, new FileInfo(destinationFullName));
        }

        return base.MoveTo(destinationParent, destination, name, context);
    }

    public override IFileInfo Rename(string newName, IContext context)
    {
        var file = new FileInfo(NativeFullName);

        var newLocation = context.Path.Combine(context.Path.GetParentPath(NativeFullName), newName);
        file.MoveTo(newLocation);

        return new FileSystemFileInfo(Parent, file);
    }
}