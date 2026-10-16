namespace SharedKernel.Interface;

public record ContactLookupItem(Guid Id, string FullName, string? CompanyName);

public interface ICrmContactLookup
{
    Task<IReadOnlyList<ContactLookupItem>> GetActiveContactsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<ContactLookupItem>> GetContactsByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default);
}
