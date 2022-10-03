using System;
using System.Globalization;
using System.Management.Automation;
using System.Management.Automation.Provider;
using System.Text;
using System.Threading;

namespace ZipAsFolder.Internal;

internal static class LoggerFactory
{
    private static long _instanceCounter;

    public static ILogger CreateLogger(NavigationCmdletProvider provider)
    {
        var parameters = provider.MyInvocation()?.BoundParameters;
        if (parameters == null || !parameters.TryGetValue("Debug", out var debug) || !(SwitchParameter)debug)
        {
            return EmptyLogger.Instance;
        }

        var instanceName = Interlocked.Increment(ref _instanceCounter).ToString();
        return new DebugLogger(provider, instanceName);
    }

    private sealed class EmptyLogger : ILogger
    {
        public static readonly EmptyLogger Instance = new();

        public void WriteLine(string line)
        {
        }

        public void WriteLine(string format, object? arg)
        {
        }

        public void WriteLine(string format, object? arg1, object? arg2)
        {
        }

        public void WriteLine(string format, object? arg1, object? arg2, object? arg3)
        {
        }

        public void WriteLine(string format, object? arg1, object? arg2, object? arg3, object? arg4)
        {
        }

        public void WriteLine(string format, object? arg1, object? arg2, object? arg3, object? arg4, object? arg5)
        {
        }

        public void WriteLine(string format, object? arg1, object? arg2, object? arg3, object? arg4, object? arg5, object? arg6)
        {
        }

        public void WriteLine(string format, object? arg1, object? arg2, object? arg3, object? arg4, object? arg5, object? arg6, object? arg7)
        {
        }
    }

    private sealed class DebugLogger : ILogger
    {
        private readonly NavigationCmdletProvider _provider;
        private readonly string _instanceName;

        public DebugLogger(NavigationCmdletProvider provider, string instanceName)
        {
            _provider = provider;
            _instanceName = instanceName;
        }

        public void WriteLine(string line)
        {
            _provider.WriteDebug(CreateText().Append(line).ToString());
        }

        public void WriteLine(string format, object? arg)
        {
            _provider.WriteDebug(CreateText().AppendFormat(format, arg).ToString());
        }

        public void WriteLine(string format, object? arg1, object? arg2)
        {
            _provider.WriteDebug(CreateText().AppendFormat(format, arg1, arg2).ToString());
        }

        public void WriteLine(string format, object? arg1, object? arg2, object? arg3)
        {
            _provider.WriteDebug(CreateText().AppendFormat(format, arg1, arg2, arg3).ToString());
        }

        public void WriteLine(string format, object? arg1, object? arg2, object? arg3, object? arg4)
        {
            _provider.WriteDebug(CreateText().AppendFormat(format, arg1, arg2, arg3, arg4).ToString());
        }

        public void WriteLine(string format, object? arg1, object? arg2, object? arg3, object? arg4, object? arg5)
        {
            _provider.WriteDebug(CreateText().AppendFormat(format, arg1, arg2, arg3, arg4, arg5).ToString());
        }

        public void WriteLine(string format, object? arg1, object? arg2, object? arg3, object? arg4, object? arg5, object? arg6)
        {
            _provider.WriteDebug(CreateText().AppendFormat(format, arg1, arg2, arg3, arg4, arg5, arg6).ToString());
        }

        public void WriteLine(string format, object? arg1, object? arg2, object? arg3, object? arg4, object? arg5, object? arg6, object? arg7)
        {
            _provider.WriteDebug(CreateText().AppendFormat(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7).ToString());
        }

        private StringBuilder CreateText()
        {
            return new StringBuilder()
                .Append(DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture))
                .Append(" ")
                .Append(_instanceName)
                .Append(" ")
                .Append(Thread.CurrentThread.ManagedThreadId)
                .Append(" ");
        }
    }
}