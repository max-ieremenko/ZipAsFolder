using System.Management.Automation;

namespace ZipAsFolder.Internal;

internal sealed class ArgumentToEncodingTransformationAttribute : ArgumentTransformationAttribute
{
    public override object Transform(EngineIntrinsics engineIntrinsics, object inputData)
    {
        switch (inputData)
        {
            case string stringName:
                return Encoding.GetEncoding(stringName);

            case int intName:
                return Encoding.GetEncoding(intName);
        }

        return inputData;
    }
}