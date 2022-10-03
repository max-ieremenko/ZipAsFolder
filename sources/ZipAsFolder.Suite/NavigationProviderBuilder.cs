using System.Collections.Generic;
using ZipAsFolder.IO;
using ZipAsFolder.Suite.Internal;

namespace ZipAsFolder.Suite;

public readonly ref struct NavigationProviderBuilder
{
    private readonly IContext _context;
    private readonly ICmdlet _cmdlet;
    private readonly List<IFileSystem> _systems;

    public NavigationProviderBuilder(IContext context, ICmdlet cmdlet)
    {
        _context = context;
        _cmdlet = cmdlet;
        _systems = new List<IFileSystem>();
    }

    public NavigationProviderBuilder With(IFileSystem fileSystem)
    {
        _systems.Add(fileSystem);
        return this;
    }

    public NavigationProvider Build()
    {
        var provider = new FileSystemProvider(_systems.ToArray());
        return new NavigationProvider(provider, _context, _cmdlet);
    }
}