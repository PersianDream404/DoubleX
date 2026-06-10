# DoubleX

A modular .NET 10.0 application built with clean architecture principles, featuring a domain-driven design (DDD) approach with separated concerns across multiple layers.

## Project Structure

```
DoubleX/
├── src/
│   ├── BuildingBlocks/          # Shared infrastructure and core components
│   │   ├── Infrastructure/      # Data access, repositories, DbContext
│   │   ├── SharedKernel/        # Common abstractions, base entities, interfaces
│   │   └── Web/                 # Main web presentation layer
│   └── Modules/
│       └── Identity/            # Identity module (authentication & authorization)
│           ├── Identity.Application/       # Application logic and services
│           ├── Identity.Application.Contract/  # DTOs and contracts
│           ├── Identity.Domain/            # Domain entities and business rules
│           ├── Identity.Persistence/       # Data persistence for identity
│           └── Identity.Presentation/      # API endpoints and controllers
├── Directory.Build.props        # Global MSBuild properties
├── Directory.Packages.props     # Central package management
└── DoubleX.slnx                 # Solution file
```

## Architecture

The project follows a **modular monolith** architecture with clear separation of concerns:

### Building Blocks

- **SharedKernel**: Core abstractions, base entities, value objects, and common interfaces
  - Base entities (`Entity`, `ValueObject`)
  - Domain interfaces (`IRepository`, `IUnitOfWork`, `IAggregateRoot`)
  - Security policies and roles
  - Constants and configuration settings

- **Infrastructure**: Cross-cutting concerns and data access
  - Entity Framework Core DbContext
  - Generic repository pattern implementation
  - Unit of Work pattern
  - Dependency injection setup

- **Web**: Main ASP.NET Core web application
  - MVC controllers
  - Razor views
  - Static assets (CSS, JavaScript)

### Modules

Modules follow Clean Architecture principles with distinct layers:

1. **Domain Layer**: Enterprise logic and entities
2. **Application Layer**: Use cases and application services
3. **Persistence Layer**: Data access implementations
4. **Presentation Layer**: API endpoints and UI

## Technology Stack

- **.NET 10.0** - Latest .NET runtime
- **ASP.NET Core** - Web framework
- **Entity Framework Core** - ORM for data access
- **SQL Server** - Database provider
- **Mapster** - Object-object mapper
- **Scrutor** - Assembly scanning for dependency injection
- **Bootstrap** - Frontend CSS framework
- **jQuery** - Client-side JavaScript library

## Getting Started

### Prerequisites

- .NET 10.0 SDK or later
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code

### Installation

1. Clone the repository:
   ```bash
   git clone <repository-url>
   cd DoubleX
   ```

2. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

3. Configure the connection string in `src/BuildingBlocks/Web/appsettings.Development.json`

4. Apply database migrations:
   ```bash
   dotnet ef database update --project src/BuildingBlocks/Infrastructure
   dotnet ef database update --project src/Modules/Identity/Identity.Persistence
   ```

5. Run the application:
   ```bash
   dotnet run --project src/BuildingBlocks/Web
   ```

## Development

### Building

```bash
dotnet build
```

### Running Tests

```bash
dotnet test
```

### Code Style

The project uses:
- Nullable reference types enabled
- Implicit usings enabled
- Central package management via `Directory.Packages.props`

## Key Features

- **Modular Design**: Features are organized into self-contained modules
- **Repository Pattern**: Abstracted data access with generic repositories
- **Unit of Work**: Transactional consistency across operations
- **Clean Architecture**: Clear separation between domain, application, and infrastructure
- **Soft Delete Support**: Built-in soft deletion for entities
- **Dependency Injection**: Auto-registration using Scrutor

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.
