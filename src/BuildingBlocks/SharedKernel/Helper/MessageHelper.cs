namespace SharedKernel.Helper;

public static class MessageHelper
{
    public static string Format(string messageTemplate, string entityName)
    {
        return string.Format(messageTemplate, entityName);
    }
}
