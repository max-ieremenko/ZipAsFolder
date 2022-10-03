using System;
using ZipAsFolder.IO;

namespace ZipAsFolder;

public partial class ZipAsFolderProvider
{
    protected override void NewItem(string path, string itemTypeName, object? newItemValue)
    {
        GetLogger().WriteLine("NewItem path={0}; itemTypeName={1}; newItemValue={2}", path, itemTypeName, newItemValue);

        var fullPath = RootPath(path);

        if ("file".EqualsIgnoreCase(itemTypeName) || string.IsNullOrEmpty(itemTypeName))
        {
            if (!GetContext().ShouldNewFileItem(fullPath))
            {
                return;
            }

            var info = GetProvider().CreateFile(fullPath, true, Force);

            if (newItemValue != null && info.AsFile(out var file))
            {
                using (var writer = GetProvider().CreateTextWriter(file, false, null))
                {
                    writer.Write(new[] { newItemValue }, GetContext().Token);
                }
            }

            GetContext().WriteItem(info);
        }
        else if ("directory".EqualsIgnoreCase(itemTypeName))
        {
            if (!GetContext().ShouldNewDirectoryItem(fullPath))
            {
                return;
            }

            var directory = GetProvider().CreateDirectory(fullPath, Force);
            GetContext().WriteItem(directory);
        }
        else
        {
            throw new NotSupportedException("New-Item -ItemType {0} is not supported.".FormatWith(itemTypeName));
        }
    }

    protected override object NewItemDynamicParameters(string path, string itemTypeName, object newItemValue) => null!;
}