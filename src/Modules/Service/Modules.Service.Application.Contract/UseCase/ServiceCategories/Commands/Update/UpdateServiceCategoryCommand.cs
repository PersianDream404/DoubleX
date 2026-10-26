using Framwork.Bus.Command;
using Modules.Service.Application.Contract.DTOs.ServiceCategories;

namespace Modules.Service.Application.Contract.UseCase.ServiceCategories.Commands.Update;

public sealed record UpdateServiceCategoryCommand(long Id, UpdateServiceCategoryRequestDto Request) : ICommand<bool>;
