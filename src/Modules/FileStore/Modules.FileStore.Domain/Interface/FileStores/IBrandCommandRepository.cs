using SharedKernel.Interface.Repositories;


namespace Modules.Product.Domain.Interface.FileStores;

public interface IFileStoreCommandRepository : ICommandRepository<FileStore.Domain.Entities.FileStores.FileStore>
{
}
