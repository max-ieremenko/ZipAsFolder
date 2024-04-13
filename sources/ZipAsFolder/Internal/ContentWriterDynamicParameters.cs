using System.Management.Automation;

namespace ZipAsFolder.Internal;

internal sealed class ContentWriterDynamicParameters
{
    [Parameter]
    public SwitchParameter NoNewline { get; set; }

    [Parameter]
    [ArgumentToEncodingTransformation]
    public Encoding? Encoding { get; set; }

    public static ContentWriterDynamicParameters FromObject(object? other)
    {
        if (other is ContentWriterDynamicParameters result)
        {
            return result;
        }

        return new ContentWriterDynamicParameters();
    }
}