using SharedKernel.Interface;
using SmeOpsHub.SharedKernel;
using System.Security.Claims;

namespace SmeOpsHub.Web.Infrastructure.Security;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _http;

    public CurrentUserService(IHttpContextAccessor http)
    {
        _http = http;
    }

    public string? UserId => _http.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    public string? UserName => _http.HttpContext?.User?.Identity?.Name;
    public string? TraceId => _http.HttpContext?.TraceIdentifier;
}
