# Banner Module - Integration Guide

## Overview
The Banner module is a modular component for managing promotional banners and advertisements in the TowX e-commerce platform. It follows a Clean Architecture pattern with clear separation of concerns across multiple layers.

## Module Structure

```
Modules/Banner/
├── Modules.Banner.Domain/              # Business logic and entities
│   ├── Entities/
│   │   └── Banner.cs                   # Banner domain entity
│   └── Interface/
│       └── IBannerRepository.cs         # Repository abstraction
│
├── Modules.Banner.Application/         # Application services
│   ├── Common/
│   │   └── BannerService.cs             # Business service implementation
│   ├── UseCase/                         # CQRS handlers (if needed)
│   └── DependencyInjection.cs           # Service registration
│
├── Modules.Banner.Application.Contract/# External contracts/interfaces
│   ├── DTOs/
│   │   ├── BannerDto.cs                 # Read model
│   │   ├── CreateBannerDto.cs           # Create command
│   │   └── UpdateBannerDto.cs           # Update command
│   └── Services/
│       └── IBannerService.cs            # Service interface
│
├── Modules.Banner.Persistence/        # Data access
│   ├── Context/
│   │   └── BannerDbContext.cs           # Entity Framework DbContext
│   ├── Repositories/
│   │   └── BannerRepository.cs          # Repository implementation
│   └── DependencyInjection.cs           # Persistence registration
│
└── Modules.Banner.Presentation/       # API endpoints
    ├── Endpoints/
    │   ├── CreateBannerEndpoint.cs
    │   ├── UpdateBannerEndpoint.cs
    │   ├── DeleteBannerEndpoint.cs
    │   ├── GetAllBannersEndpoint.cs
    │   └── GetActiveBannersEndpoint.cs
    └── BannerModule.cs                 # Module configuration
```

## Key Features

### Banner Entity
- **Title**: Banner title (max 500 characters)
- **Description**: Banner description (max 1000 characters)
- **ImageUrl**: URL to banner image (max 1000 characters)
- **Link**: Navigation link (max 1000 characters)
- **Order**: Display order
- **IsActive**: Active/inactive status
- **StartDate**: When banner becomes active
- **EndDate**: Optional expiration date
- **BannerType**: Banner type (e.g., Hero, Sidebar)

### API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/banners` | Create new banner |
| PUT | `/api/banners/{id}` | Update banner |
| DELETE | `/api/banners/{id}` | Delete banner |
| GET | `/api/banners` | Get all banners |
| GET | `/api/banners/active` | Get active banners only |

## Integration Steps

### 1. Update Project References
Add project references in your main Web project:
```xml
<ProjectReference Include="..\Modules\Banner\Modules.Banner.Presentation\Modules.Banner.Presentation.csproj" />
<ProjectReference Include="..\Modules\Banner\Modules.Banner.Persistence\Modules.Banner.Persistence.csproj" />
```

### 2. Configure in Program.cs
```csharp
// Add at startup
var bannerModule = new BannerModule();
bannerModule.RegisterServices(services, configuration);
bannerModule.MapEndpoints(app);
```

### 3. Database Migration
Create and apply migration:
```bash
dotnet ef migrations add AddBannerModule --project Modules/Banner/Modules.Banner.Persistence --startup-project Web
dotnet ef database update --project Modules/Banner/Modules.Banner.Persistence --startup-project Web
```

### 4. Connection String
Ensure your `appsettings.json` contains:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=...;"
  }
}
```

## Banner Types
The module supports different banner types for various placements:
- `Hero` - Full-width header banners
- `Sidebar` - Side banner placements
- `Popup` - Modal/popup banners
- `Footer` - Footer banners
- Custom types as needed

## Service Layer

### IBannerService Interface
```csharp
public interface IBannerService
{
    Task<BannerDto> CreateBannerAsync(CreateBannerDto dto, CancellationToken cancellationToken);
    Task<BannerDto> UpdateBannerAsync(UpdateBannerDto dto, CancellationToken cancellationToken);
    Task<bool> DeleteBannerAsync(int id, CancellationToken cancellationToken);
    Task<BannerDto> GetBannerByIdAsync(int id, CancellationToken cancellationToken);
    Task<IEnumerable<BannerDto>> GetAllBannersAsync(CancellationToken cancellationToken);
    Task<IEnumerable<BannerDto>> GetActiveBannersAsync(CancellationToken cancellationToken);
    Task<IEnumerable<BannerDto>> GetBannersByTypeAsync(string bannerType, CancellationToken cancellationToken);
}
```

## Future Enhancements
- [ ] Add banner image upload service integration
- [ ] Add banner analytics/tracking
- [ ] Add banner scheduling
- [ ] Add multi-language support
- [ ] Add banner categories
- [ ] Add click tracking and metrics
- [ ] Add unit tests
- [ ] Add integration tests
