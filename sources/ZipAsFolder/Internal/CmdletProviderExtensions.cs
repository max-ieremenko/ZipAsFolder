using System;
using System.Linq.Expressions;
using System.Management.Automation;
using System.Management.Automation.Provider;
using System.Reflection;

namespace ZipAsFolder.Internal;

internal static class CmdletProviderExtensions
{
    private static readonly Func<CmdletProvider, object>? GetContext;
    private static readonly Func<object, InvocationInfo>? GetMyInvocation;

    static CmdletProviderExtensions()
    {
        var getContext = typeof(NavigationCmdletProvider)
            .GetProperty("Context", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            ?.GetMethod;

        if (getContext == null)
        {
            return;
        }

        GetContext = (Func<CmdletProvider, object>)getContext.CreateDelegate(typeof(Func<CmdletProvider, object>));

        var getMyInvocation = getContext
            .ReturnType
            .GetProperty("MyInvocation", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        if (getMyInvocation == null)
        {
            return;
        }

        var context = Expression.Parameter(typeof(object));
        var call = Expression.Property(Expression.Convert(context, getContext.ReturnType), getMyInvocation);

        GetMyInvocation = Expression.Lambda<Func<object, InvocationInfo>>(call, context).Compile();
    }

    public static InvocationInfo? MyInvocation(this CmdletProvider provider)
    {
        if (GetMyInvocation == null || GetContext == null)
        {
            return null;
        }

        var context = GetContext(provider);
        return GetMyInvocation(context);
    }
}