using Framwork.Bus.Command;

namespace Modules.Service.Application.Contract.UseCase.ServiceMaterials.Commands.Remove;

public sealed record RemoveServiceMaterialCommand(long SalonId, long Id) : ICommand<bool>;
