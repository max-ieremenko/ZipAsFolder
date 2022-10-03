namespace ZipAsFolder.Internal;

internal interface ILogger
{
    void WriteLine(string line);

    void WriteLine(string format, object? arg);

    void WriteLine(string format, object? arg1, object? arg2);

    void WriteLine(string format, object? arg1, object? arg2, object? arg3);

    void WriteLine(string format, object? arg1, object? arg2, object? arg3, object? arg4);

    void WriteLine(string format, object? arg1, object? arg2, object? arg3, object? arg4, object? arg5);

    void WriteLine(string format, object? arg1, object? arg2, object? arg3, object? arg4, object? arg5, object? arg6);

    void WriteLine(string format, object? arg1, object? arg2, object? arg3, object? arg4, object? arg5, object? arg6, object? arg7);
}