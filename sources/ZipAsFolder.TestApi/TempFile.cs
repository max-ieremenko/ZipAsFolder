using System;
using System.Diagnostics;
using System.IO;
using Shouldly;

namespace ZipAsFolder.TestApi;

public sealed class TempFile : IDisposable
{
    public TempFile()
    {
        Location = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
    }

    public string Location { get; }

    public static TempFile FromResources(string resourceName, Type? resourceAnchor = null)
    {
        if (resourceAnchor == null)
        {
            resourceAnchor = new StackTrace().GetFrame(1)!.GetMethod()!.DeclaringType!;
        }

        var source = resourceAnchor.Assembly.GetManifestResourceStream(resourceAnchor.Namespace + "." + resourceName);
        source.ShouldNotBeNull(resourceName);

        var result = new TempFile();

        using (source)
        using (var dest = new FileStream(result.Location, FileMode.Create, FileAccess.ReadWrite))
        {
            source.CopyTo(dest);
        }

        return result;
    }

    public void Dispose()
    {
        if (File.Exists(Location))
        {
            File.Delete(Location);
        }
    }
}