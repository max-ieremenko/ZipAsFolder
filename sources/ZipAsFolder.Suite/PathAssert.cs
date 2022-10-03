using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace ZipAsFolder.Suite;

public static class PathAssert
{
    public static void PathExists([DoesNotReturnIf(false)] bool condition, string path)
    {
        if (!condition)
        {
            throw new IOException("Could not find a part of the path [{0}].".FormatWith(path));
        }
    }

    public static void FileDoesNoExist([DoesNotReturnIf(false)] bool condition, string path, bool addForceHint = false)
    {
        if (!condition)
        {
            var message = "The file [{0}] already exists.{1}".FormatWith(
                path,
                addForceHint ? "Use the -Force parameter if you want to overwrite the destination file." : string.Empty);
            throw new InvalidOperationException(message);
        }
    }

    public static void DirectoryDoesNoExist([DoesNotReturnIf(false)] bool condition, string path)
    {
        if (!condition)
        {
            throw new InvalidOperationException("The directory [{0}] already exists.".FormatWith(path));
        }
    }

    public static void PathIsFile([DoesNotReturnIf(false)] bool condition, string path)
    {
        if (!condition)
        {
            throw new InvalidOperationException("The path [{0}] is directory.".FormatWith(path));
        }
    }

    public static void PathIsDirectory([DoesNotReturnIf(false)] bool condition, string path)
    {
        if (!condition)
        {
            throw new InvalidOperationException("The path [{0}] is file.".FormatWith(path));
        }
    }
}