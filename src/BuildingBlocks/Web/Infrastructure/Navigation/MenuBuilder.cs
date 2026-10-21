using SharedKernel.Interface;
using SmeOpsHub.SharedKernel;

namespace Web.Infrastructure.Navigation;

public class MenuBuilder : IMenuBuilder
{
    private readonly List<MenuItem> _items = new();

    public IReadOnlyCollection<MenuItem> Items => _items.AsReadOnly();

    public void AddMenuItem(MenuItem item)
    {
        // Avoid duplicate keys (optional)
        if (_items.Any(x => x.Key == item.Key))
        {
            return;
        }

        _items.Add(item);
    }
}
