using System.Management.Automation;
using System.Management.Automation.Provider;
using ZipAsFolder.Internal;
using ZipAsFolder.Suite;

namespace ZipAsFolder;

[CmdletProvider(Names.ProviderName, ProviderCapabilities.ShouldProcess | ProviderCapabilities.Filter)]
public sealed partial class ZipAsFolderProvider : NavigationCmdletProvider, IContentCmdletProvider
{
    private CmdletContext? _context;
    private ILogger? _logger;
    private NavigationProvider? _navigationProvider;

    protected override ProviderInfo Start(ProviderInfo providerInfo)
    {
        GetProvider();
        return base.Start(providerInfo);
    }

    protected override void StopProcessing()
    {
        GetContext().Cancel();
        base.StopProcessing();
    }

    private CmdletContext GetContext()
    {
        if (_context == null)
        {
            var settings = ZipAsFolderProviderSettings.FromPsVariable(SessionState.PSVariable);
            _context = new CmdletContext(this, NavigationProviderBuilderExtensions.Path, settings);
        }

        return _context;
    }

    private NavigationProvider GetProvider()
    {
        if (_navigationProvider == null)
        {
            var context = GetContext();
            _navigationProvider = new NavigationProviderBuilder(context, context)
                .WithFileSystem()
                .WithZipFileSystem(context.Settings.CompressionLevel, context.Settings.GetZipExtensions())
                .Build();
        }

        return _navigationProvider;
    }

    private ILogger GetLogger()
    {
        if (_logger == null)
        {
            _logger = LoggerFactory.CreateLogger(this);
        }

        return _logger;
    }
}