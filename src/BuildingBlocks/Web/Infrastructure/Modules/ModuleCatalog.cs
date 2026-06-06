using SharedKernel.Interface;
using SmeOpsHub.SharedKernel;

namespace Web.Infrastructure.Modules;

public interface IModuleCatalog
{
    IReadOnlyCollection<IModule> Modules { get; }
    IReadOnlyCollection<MenuItem> MenuItems { get; }
}

public class ModuleCatalog : IModuleCatalog
{
    public IReadOnlyCollection<IModule> Modules { get; }
    public IReadOnlyCollection<MenuItem> MenuItems { get; }

    public ModuleCatalog(IEnumerable<IModule> modules, IEnumerable<MenuItem> menuItems)
    {
        Modules = modules.OrderBy(m => m.Order).ToArray();
        MenuItems = menuItems.OrderBy(m => m.Order).ToArray();
    }
}
