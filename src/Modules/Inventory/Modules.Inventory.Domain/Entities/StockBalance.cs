using SharedKernel.Base;

namespace Modules.Inventory.Domain.Entities;

public sealed class StockBalance : BaseEntityIdentity
{
    public long MaterialId { get; private set; }
    public decimal Quantity { get; private set; }
    public byte[] RowVersion { get; private set; } = [];
    public Material Material { get; private set; } = null!;

    private StockBalance()
    {
    }

    private StockBalance(long materialId)
    {
        MaterialId = materialId;
        Quantity = 0;
    }

    public static StockBalance Create(long materialId) => new(materialId);

    public bool TryApply(decimal change)
    {
        if (Quantity + change < 0)
        {
            return false;
        }

        Quantity += change;
        return true;
    }
}
