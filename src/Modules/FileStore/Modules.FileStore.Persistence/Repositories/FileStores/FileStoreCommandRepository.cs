using Infrastructure.Repositories;
using Modules.Product.Domain.Interface.FileStores;
using Modules.FileStore.Persistence.Context;
namespace Modules.FileStore.Persistence.Repositories.FileStores;


public class FileStoreCommandRepository
    : CommandRepository<Domain.Entities.FileStores.FileStore>, IFileStoreCommandRepository
{
    public FileStoreCommandRepository(FileStoreWriteDbContext context) : base(context)
    {
    }

 
}
