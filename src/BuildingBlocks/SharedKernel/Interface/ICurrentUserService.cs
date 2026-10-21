namespace SharedKernel.Interface;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? UserName { get; }
    string? TraceId { get; }
}
