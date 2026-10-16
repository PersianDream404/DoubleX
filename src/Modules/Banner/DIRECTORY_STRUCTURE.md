# Banner Module - Directory Structure

## Complete Module Structure

```
src/Modules/Banner/
│
├── Modules.Banner.Domain/
│   ├── bin/
│   ├── obj/
│   ├── Entities/
│   │   └── Banner.cs                          # Banner domain entity
│   ├── Interface/
│   │   └── IBannerRepository.cs               # Repository contract
│   ├── Enums/                                 # (placeholder for future enums)
│   └── Modules.Banner.Domain.csproj           # Domain project file
│
├── Modules.Banner.Application/
│   ├── bin/
│   ├── obj/
│   ├── Common/
│   │   └── BannerService.cs                   # Core service implementation
│   ├── UseCase/                               # (CQRS queries/commands if needed)
│   ├── DependencyInjection.cs                 # Service registration
│   └── Modules.Banner.Application.csproj      # Application project file
│
├── Modules.Banner.Application.Contract/
│   ├── bin/
│   ├── obj/
│   ├── DTOs/
│   │   ├── BannerDto.cs                       # Read model DTO
│   │   ├── CreateBannerDto.cs                 # Create request DTO
│   │   └── UpdateBannerDto.cs                 # Update request DTO
│   ├── Services/
│   │   └── IBannerService.cs                  # Public service interface
│   ├── Interface/                             # (additional interfaces)
│   └── Modules.Banner.Application.Contract.csproj
│
├── Modules.Banner.Persistence/
│   ├── bin/
│   ├── obj/
│   ├── Context/
│   │   └── BannerDbContext.cs                 # EF Core DbContext
│   ├── Repositories/
│   │   └── BannerRepository.cs                # Repository implementation
│   ├── Configuration/                         # (Entity configurations)
│   ├── Mapper/                                # (Mapster configurations)
│   ├── Migrations/                            # (EF Core migrations)
│   ├── DependencyInjection.cs                 # Persistence DI setup
│   └── Modules.Banner.Persistence.csproj      # Persistence project file
│
├── Modules.Banner.Presentation/
│   ├── bin/
│   ├── obj/
│   ├── Endpoints/
│   │   ├── CreateBannerEndpoint.cs            # POST /api/banners
│   │   ├── UpdateBannerEndpoint.cs            # PUT /api/banners/{id}
│   │   ├── DeleteBannerEndpoint.cs            # DELETE /api/banners/{id}
│   │   ├── GetAllBannersEndpoint.cs           # GET /api/banners
│   │   └── GetActiveBannersEndpoint.cs        # GET /api/banners/active
│   ├── BannerModule.cs                        # Module configuration
│   └── Modules.Banner.Presentation.csproj     # Presentation project file
│
├── INTEGRATION_GUIDE.md                       # Integration instructions
├── MODULE_SUMMARY.md                          # Module overview
└── DIRECTORY_STRUCTURE.md                     # This file
```

## Layer Descriptions

### Domain Layer (`Modules.Banner.Domain`)
**Purpose**: Contains business logic and domain models
- **Entities**: Business entities (Banner.cs)
- **Interfaces**: Contracts for repositories
- **No External Dependencies**: Only references SharedKernel

### Application Layer (`Modules.Banner.Application`)
**Purpose**: Application services and business logic orchestration
- **Common**: Core services (BannerService.cs)
- **UseCase**: CQRS handlers (queries/commands) - optional
- **DependencyInjection**: Service registration
- **Dependencies**: Domain, Framework, FluentValidation, Mapster

### Contract Layer (`Modules.Banner.Application.Contract`)
**Purpose**: Public interface definitions and DTOs
- **DTOs**: Data transfer objects (BannerDto, CreateBannerDto, UpdateBannerDto)
- **Services**: Public service contracts (IBannerService)
- **Interfaces**: Additional contracts
- **No Dependencies**: Only external dependencies

### Persistence Layer (`Modules.Banner.Persistence`)
**Purpose**: Data access and database configuration
- **Context**: EF Core DbContext (BannerDbContext)
- **Repositories**: Data access implementations (BannerRepository)
- **Configuration**: Entity mappings and configurations
- **Migrations**: EF Core migrations
- **Dependencies**: Domain, Infrastructure, EntityFrameworkCore

### Presentation Layer (`Modules.Banner.Presentation`)
**Purpose**: API endpoints and HTTP routing
- **Endpoints**: Carter-based route handlers
  - Create banner: POST /api/banners
  - Update banner: PUT /api/banners/{id}
  - Delete banner: DELETE /api/banners/{id}
  - Get all banners: GET /api/banners
  - Get active banners: GET /api/banners/active
- **Module**: BannerModule bootstrapper (implements IModule)
- **Dependencies**: All other layers, Carter

## File Summary

| File | Layer | Purpose |
|------|-------|---------|
| Banner.cs | Domain | Entity definition |
| IBannerRepository.cs | Domain | Repository interface |
| BannerService.cs | Application | Service implementation |
| DependencyInjection.cs (App) | Application | Service registration |
| BannerDto.cs | Contract | Read model |
| CreateBannerDto.cs | Contract | Create request |
| UpdateBannerDto.cs | Contract | Update request |
| IBannerService.cs | Contract | Service interface |
| BannerDbContext.cs | Persistence | EF Core context |
| BannerRepository.cs | Persistence | Data access |
| DependencyInjection.cs (Persist) | Persistence | Persistence registration |
| BannerModule.cs | Presentation | Module bootstrapper |
| *Endpoint.cs (5 files) | Presentation | API routes |

## Project References

```
Modules.Banner.Presentation.csproj
  ├─→ Modules.Banner.Application
  ├─→ Modules.Banner.Application.Contract
  ├─→ Modules.Banner.Persistence
  ├─→ Framwork
  └─→ Carter, MediatR

Modules.Banner.Application.csproj
  ├─→ Modules.Banner.Application.Contract
  ├─→ Framwork
  └─→ FluentValidation, Mapster, Scrutor

Modules.Banner.Persistence.csproj
  ├─→ Modules.Banner.Domain
  ├─→ Modules.Banner.Application.Contract
  ├─→ Infrastructure
  └─→ Microsoft.EntityFrameworkCore

Modules.Banner.Application.Contract.csproj
  └─→ (no project references)

Modules.Banner.Domain.csproj
  └─→ SharedKernel
```

## Integration Checklist

- [ ] Add project references to Web.csproj
- [ ] Register module in Program.cs
- [ ] Update connection string in appsettings.json
- [ ] Create database migration
- [ ] Apply migration to database
- [ ] Test endpoints with Postman/curl
- [ ] Add authentication/authorization if needed
- [ ] Add validation rules to DTOs
- [ ] Add unit tests
- [ ] Add integration tests
