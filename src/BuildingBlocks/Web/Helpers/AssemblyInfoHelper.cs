using System.Reflection;

namespace Web.Helpers;

public static class AssemblyInfoHelper
{
    public static string GetAssemblyVersion()
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version;
        return version != null ? version.ToString() : "Unknown";
    }

    public static DateTime GetBuildDate()
    {
        return System.IO.File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location);
    }
}
