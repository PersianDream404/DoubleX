using Microsoft.Extensions.DependencyModel;
using SharedKernel.Interface;
using SmeOpsHub.SharedKernel;
using System.Reflection;

namespace Web.Infrastructure.Modules;

public static class ModuleLoader
{
    private static readonly List<string> ModulePrefixes =
    [
        "Identity.",
        "Modules."
    ];

    public static IReadOnlyCollection<IModule> DiscoverModules()
    {
        var runtimeLibs = DependencyContext.Default?.RuntimeLibraries
            .Where(l => ModulePrefixes.Any(p =>
                l.Name.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
            .ToList() ?? new List<RuntimeLibrary>();

        foreach (var lib in runtimeLibs)
        {
            try
            {
                Assembly.Load(new AssemblyName(lib.Name));
            }
            catch
            {
            }
        }

        var moduleAssemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => ModulePrefixes.Any(p =>
                a.GetName().Name?.StartsWith(p, StringComparison.OrdinalIgnoreCase) == true))
            .ToList();

        var modules = moduleAssemblies
            .SelectMany(SafeGetTypes)
            .Where(t => t is not null && !t.IsAbstract && typeof(IModule).IsAssignableFrom(t))
            .Select(t => (IModule)Activator.CreateInstance(t!)!)
            //.OrderBy(m => m.Order)
            .ToArray();

        return modules;
    }

    private static IEnumerable<Type?> SafeGetTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types;
        }
    }
}

