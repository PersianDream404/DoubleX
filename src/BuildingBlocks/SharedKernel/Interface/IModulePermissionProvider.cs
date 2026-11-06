namespace SharedKernel.Interface;

public interface IModulePermissionProvider
{
    IEnumerable<string> GetPermissions();
}
