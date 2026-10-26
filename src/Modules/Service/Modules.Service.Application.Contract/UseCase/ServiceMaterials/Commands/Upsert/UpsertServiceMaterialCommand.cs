using Framwork.Bus.Command;
using Modules.Service.Application.Contract.DTOs.ServiceMaterials;

namespace Modules.Service.Application.Contract.UseCase.ServiceMaterials.Commands.Upsert;

public sealed record UpsertServiceMaterialCommand(long? Id, ServiceMaterialRequestDto Request) : ICommand<bool>;
