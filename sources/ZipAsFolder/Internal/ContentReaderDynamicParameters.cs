using System.Management.Automation;
using System.Text;

namespace ZipAsFolder.Internal;

internal sealed class ContentReaderDynamicParameters
{
    [Parameter]
    public SwitchParameter Raw { get; set; }

    [Parameter]
    [ArgumentToEncodingTransformation]
    public Encoding? Encoding { get; set; }

    public static ContentReaderDynamicParameters FromObject(object? other)
    {
        if (other is ContentReaderDynamicParameters result)
        {
            return result;
        }

        return new ContentReaderDynamicParameters();
    }
}