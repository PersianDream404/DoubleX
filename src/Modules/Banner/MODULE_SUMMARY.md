# Banner Module - Structure Summary

## Directory Layout

```
Modules/Banner/
├── Modules.Banner.Domain/
│   ├── Entities/
│   │   └── Banner.cs
│   ├── Interface/
│   │   └── IBannerRepository.cs
│   └── Modules.Banner.Domain.csproj
│
├── Modules.Banner.Application/
│   ├── Common/
│   │   └── BannerService.cs
│   ├── UseCase/
│   ├── DependencyInjection.cs
│   └── Modules.Banner.Application.csproj
│
├── Modules.Banner.Application.Contract/
│   ├── DTOs/
│   │   ├── BannerDto.cs
│   │   ├── CreateBannerDto.cs
│   │   └── UpdateBannerDto.cs
│   ├── Services/
│   │   └── IBannerService.cs
│   ├── Interface/
│   └── Modules.Banner.Application.Contract.csproj
│
├── Modules.Banner.Persistence/
│   ├── Context/
│   │   └── BannerDbContext.cs
│   ├── Repositories/
│   │   └── BannerRepository.cs
│   ├── Configuration/
│   ├── Mapper/
│   ├── DependencyInjection.cs
│   └── Modules.Banner.Persistence.csproj
│
├── Modules.Banner.Presentation/
│   ├── Endpoints/
│   │   ├── CreateBannerEndpoint.cs
│   │   ├── UpdateBannerEndpoint.cs
│   │   ├── DeleteBannerEndpoint.cs
│   │   ├── GetAllBannersEndpoint.cs
│   │   └── GetActiveBannersEndpoint.cs
│   ├── BannerModule.cs
│   └── Modules.Banner.Presentation.csproj
│
├── INTEGRATION_GUIDE.md
├── MODULE_SUMMARY.md
└── DIRECTORY_STRUCTURE.md
```

## Layer Responsibilities

### Domain Layer (Modules.Banner.Domain)
- Contains `Banner` entity
- Defines repository interface `IBannerRepository`
- No external dependencies
- Purely business logic

### Application Layer (Modules.Banner.Application)
- `BannerService` implements `IBannerService`
- Uses Mapster for DTO mapping
- Orchestrates business logic
- Depends on Domain and Application.Contract

### Application.Contract Layer (Modules.Banner.Application.Contract)
- Defines public interfaces and DTOs
- `IBannerService` service interface
- `BannerDto`, `CreateBannerDto`, `UpdateBannerDto` DTOs
- No implementation - only contracts

### Persistence Layer (Modules.Banner.Persistence)
- `BannerDbContext` - EF Core configuration
- `BannerRepository` - Repository pattern implementation
- Entity configurations and mappings
- Depends on Domain and Application layers

### Presentation Layer (Modules.Banner.Presentation)
- Carter-based API endpoints
- `BannerModule` - module bootstrapper
- RESTful API for CRUD operations
- Depends on all other layers

## Project Dependencies

```
Presentation -> Application, Application.Contract, Persistence, Domain
Persistence -> Domain, Application.Contract, Infrastructure
Application -> Application.Contract, Domain
Application.Contract -> (no dependencies)
Domain -> SharedKernel
```

## Files Created

### Domain
- `Banner.cs` - Entity with properties for title, description, image, link, dates, etc.
- `IBannerRepository.cs` - Repository interface

### Application
- `BannerService.cs` - Service implementing IBannerService
- `DependencyInjection.cs` - Service registration and Mapster config

### Application.Contract
- `BannerDto.cs` - Read model
- `CreateBannerDto.cs` - Create request model
- `UpdateBannerDto.cs` - Update request model
- `IBannerService.cs` - Service contract

### Persistence
- `BannerDbContext.cs` - EF Core context with fluent configuration
- `BannerRepository.cs` - Repository implementation with queries
- `DependencyInjection.cs` - DbContext and repository registration

### Presentation
- `BannerModule.cs` - IModule implementation
- `CreateBannerEndpoint.cs` - POST /api/banners
- `UpdateBannerEndpoint.cs` - PUT /api/banners/{id}
- `DeleteBannerEndpoint.cs` - DELETE /api/banners/{id}
- `GetAllBannersEndpoint.cs` - GET /api/banners
- `GetActiveBannersEndpoint.cs` - GET /api/banners/active

## Technology Stack

- **Framework**: ASP.NET Core
- **ORM**: Entity Framework Core
- **API**: Carter (lightweight routing)
- **Mapping**: Mapster
- **Validation**: FluentValidation
- **DI**: Microsoft.Extensions.DependencyInjection

## Next Steps

1. ✅ Module structure created
2. ⬜ Integrate into Web project
3. ⬜ Run migrations
4. ⬜ Test endpoints
5. ⬜ Add validation rules
6. ⬜ Add unit tests
