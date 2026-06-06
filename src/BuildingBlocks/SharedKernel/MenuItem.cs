namespace SmeOpsHub.SharedKernel;

public class MenuItem
{
    public string Key { get; }
    public string DisplayName { get; }
    public string Url { get; }
    public int Order { get; }

    public MenuItem(string key, string displayName, string url, int order = 0)
    {
        Key = key;
        DisplayName = displayName;
        Url = url;
        Order = order;
    }
}
