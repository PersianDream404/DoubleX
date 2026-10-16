using Framwork.Bus.Command;
using Modules.Banner.Application.Contract.DTOs.Banners.Create;
using Modules.Banner.Application.Contract.DTOs.Banners.Update;

namespace Modules.Banner.Application.Contract.UseCase.Banners.Commands;

public record CreateBannerCommand(CreateBannerRequestDto request) : ICommand<bool>;
public record UpdateBannerCommand(UpdateBannerRequestDto request) : ICommand<bool>;
public record DeleteBannerCommand(long Id) : ICommand<bool>;
public record ToggleBannerCommand(long Id) : ICommand<bool>;
