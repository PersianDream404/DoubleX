---
name: backend-development
description: "Use when implementing or modifying ASP.NET Core backend modules, APIs, CQRS commands and queries, repositories, EF Core persistence, mappings, validation, or dependency injection in this repository. Follow the verified Customer module patterns without forcing domain-specific logic onto other modules."
---

# Backend Development

Use this skill for backend work in this repository. Treat existing source code as the source of truth: inspect the target module first and use Customer as the primary reference when a convention is unclear. Customer is an architectural example, not a template for domain behavior.

## Required workflow

1. Inspect the target module's projects, folders, related feature, and callers before editing.
2. If a pattern is unclear, inspect the corresponding Customer feature and relevant shared BuildingBlocks abstractions. Verify paths, signatures, dependencies, and actual behavior; do not infer them from names.
3. Trace a related request end-to-end (endpoint -> bus/handler -> interface -> implementation -> database/result) before changing a layer.
4. Reuse existing base classes, interfaces, buses, pagination helpers, result types, validators, utilities, and registration mechanisms when appropriate.
5. Make the smallest change that satisfies the requirement. Preserve module boundaries and unrelated work; do not add layers, abstractions, packages, or refactors speculatively.
6. If a verified project pattern conflicts with the requirement, explain the conflict and choose the smallest suitable adjustment. Distinguish established conventions from defects; do not silently copy a bug or inconsistent behavior.
7. Build the affected project (and host if relevant) and run relevant tests when available. Report exactly what was run and any pre-existing warnings or failures.

## Verified solution and Customer structure

The solution uses separate projects per module and layer. Customer is registered in `TowX.slnx` and currently has:

- `Modules.Customer.Domain`: entity and domain repository contract.
- `Modules.Customer.Application.Contract`: request/response DTOs, command/query records, and query repository contract.
- `Modules.Customer.Application`: command/query handlers, FluentValidation validators, and application DI registration.
- `Modules.Customer.Persistence`: EF Core contexts/configuration/migrations, manual projections, repository implementations, and infrastructure DI.
- `Modules.Customer.Presentation`: `IEndpoint` minimal API endpoints and module registration.

Dependencies verified in Customer project files:

- Domain depends on SharedKernel.
- Application.Contract depends on Domain, Framwork, and SharedKernel.
- Application depends on Application.Contract, Domain, and Framwork.
- Persistence depends on Application.Contract, Domain, Infrastructure, SharedKernel, and Salon Persistence for its existing salon relationship/migration integration.
- Presentation composes Application, Application.Contract, Persistence, Framwork, and SharedKernel.

Do not create every project/layer just because Customer has it. Follow the target module's existing structure and only add the pieces required by the feature.

## Feature folders and names

Customer feature files are organized by capability under `Customers`:

- Application handlers: `UseCase/Customers/Commands/<Feature>/` and `UseCase/Customers/Queries/<Feature>/`.
- Contracts: `UseCase/Customers/Commands/<Feature>/` and `UseCase/Customers/Queries/<Feature>/`.
- DTOs: `DTOs/Customers/{Create,Update,Get,GetAll}/`.
- Query repository contract: `Interfaces/Customers/`.
- Persistence implementations and mapper: `Repositories/Customers/` and `Mapper/Customers/`.
- API endpoints: `Endpoints/Customers/`.

Use the target module's established singular/plural and folder conventions. For a new Customer capability, place files with the matching feature rather than accumulating unrelated feature types in a shared file. Avoid moving unrelated code.

## Domain model

`Customer` is a sealed `BaseEntityIdentity` entity. It uses a private EF constructor, private-set properties, a static `Create(...)` factory, and an `Update(...)` method. It has primitive properties and a `SalonId`; no Customer-specific value objects or separate domain services are currently implemented.

When changing a domain model:

- Keep invariants and state changes in the domain entity where the target module does so.
- Use existing base entity and repository abstractions after verifying their contracts.
- Do not introduce value objects or a domain service without a domain requirement and precedent in the target module.
- Keep persistence-specific concerns out of the domain project.

Reference: [Customer entity](../../../src/Modules/Customer/Modules.Customer.Domain/Entities/Customer.cs).

## Contracts, DTOs, and handlers

Customer request DTOs use mutable properties, data-annotation constraints, and localized `[Display]` metadata. Create and update DTOs inherit shared fields from `CustomerRequestDto`; list and detail DTOs are separate, with details currently inheriting the list DTO. CQRS messages are records implementing `ICommand<T>` or `IQuery<T>`.

Handlers implement `ICommandHandler<TCommand,TResult>` or `IQueryHandler<TQuery,TResult>` and return `Ardalis.Result` values. They orchestrate repository calls and domain behavior; for example, create verifies the referenced salon, constructs the entity with `Customer.Create`, then calls the command repository.

- Keep DTOs in Application.Contract and handlers in Application.
- Reuse a shared request DTO only when fields and semantics are genuinely shared.
- Keep query results and commands typed to the existing bus contracts.
- Pass `CancellationToken` through endpoint, bus, handler, repository, and EF operations.
- Do not add a service layer by default: Customer currently has no separate service abstraction.

References: [Customer request DTO](../../../src/Modules/Customer/Modules.Customer.Application.Contract/DTOs/Customers/CustomerRequestDto.cs), [create command](../../../src/Modules/Customer/Modules.Customer.Application.Contract/UseCase/Customers/Commands/Create/CreateCustomerCommand.cs), [create handler and validator](../../../src/Modules/Customer/Modules.Customer.Application/UseCase/Customers/Commands/Create/CreateCustomerCommandHandler.cs).

## Repositories and mapping

Customer separates read and write repository contracts:

- `ICustomerCommandRepository` is in Domain and extends the shared `ICommandRepository<Customer>`.
- `ICustomerQueryRepository` is in Application.Contract and extends `IQueryRepository<Customer>`.
- `CustomerCommandRepository` delegates standard writes to `CommandRepository<Customer>`.
- `CustomerQueryRepository` extends `QueryRepository<Customer>` and contains Customer-specific reads.

Use a manual static mapper when the target module's convention calls for explicit projection. Customer's `CustomerMapper` returns `Expression<Func<Customer,...>>` projections; the query repository passes these expressions to EF `.Select(...)`, so projection occurs in the database query. There is no global mapper registration in Customer.

- Do not replace a verified manual projection pattern with runtime object mapping or add a mapping package without a demonstrated need.
- Do not create a mapper merely by habit; inspect the target module and reuse its established approach.
- Keep projections explicit and update affected DTOs and projections together.
- Preserve cancellation, tracking, and projection behavior appropriate to the operation.

References: [query repository contract](../../../src/Modules/Customer/Modules.Customer.Application.Contract/Interfaces/Customers/ICustomerQueryRepository.cs), [query repository](../../../src/Modules/Customer/Modules.Customer.Persistence/Repositories/Customers/CustomerQueryRepository.cs), [Customer mapper](../../../src/Modules/Customer/Modules.Customer.Persistence/Mapper/Customers/CustomerMapper.cs).

## Validation and errors

Customer uses FluentValidation validators beside handlers. Rules currently cover required values, lengths, national-code format, positive IDs, salon ID, and past birth dates. Some messages use shared localized `SharedValidationMessages`; others are literal Persian strings. Request DTOs also have data annotations, so do not assume one validation mechanism replaces the other.

Handlers return `Result.Success`, `Result.NotFound`, or other `Ardalis.Result` outcomes. Customer handlers do not wrap operations in catch-all exception handlers and do not log. Do not add broad catches, hide failures, or return success-shaped fallbacks. Follow the target module's established exception policy and preserve explicit errors.

Customer endpoint status mappings are not fully uniform: list/create map failures to BadRequest, detail maps to NotFound, and delete/toggle map failures to NotFound. Verify the operation's result contract and API convention before selecting status codes; do not treat these differences as a universal standard.

Reference: [shared validation messages](../../../src/BuildingBlocks/Framwork/Validation/Resources/SharedValidationMessages.Designer.cs).

## Persistence and migrations

Customer has separate write and read `DbContext` classes, both derived from `BaseDbContext`, applying configurations from the Persistence assembly. `CustomerConfiguration` configures table/column constraints, required fields, a restricted salon FK, and an index on `(SalonId, NationalCode)`. SQL Server EF Core migrations live in `Persistence/Migrations`.

The current context maps a minimal `CustomerSalonReference` to the existing `Salons` table. The Customer migration creates `Customers` and its FK; it must not recreate the Salon table. Startup migration application explicitly migrates the Salon write context before Customer because of this relationship.

- Add entity constraints/indexes/relationships to `IEntityTypeConfiguration<T>` and create/update migrations through the repository's established EF tooling.
- Keep read/write contexts and design-time factory aligned with existing module conventions.
- Inspect generated migrations carefully for unrelated tables, altered schemas, and FK targets.
- Do not change database schema or apply migrations to unrelated contexts without a requirement.

References: [Customer contexts](../../../src/Modules/Customer/Modules.Customer.Persistence/Context/CustomerDbContexts.cs), [entity configuration](../../../src/Modules/Customer/Modules.Customer.Persistence/Configuration/CustomerConfiguration.cs), [migration registration](../../../src/Modules/Customer/Modules.Customer.Persistence/Extensions/CustomerMigrationExtensions.cs).

## API endpoints and responses

Customer uses minimal API endpoints implementing `IEndpoint`, nested endpoint classes, and a shared `CustomerApiInfo` prefix/tag. Endpoints dispatch through `ICommandBus` or `IQueryBus`, pass cancellation tokens, and map `Ardalis.Result` to HTTP results with `BaseEndpoint` helpers such as `Ok`, `BadRequest`, and `NotFound`. The module discovers endpoint implementations by assembly scanning.

Follow the target module's route/version/tag conventions and inspect its shared response helpers. Verify method, route, binding, authorization, status codes, and response shape; do not invent an endpoint or assume every operation returns the same envelope.

Customer endpoints currently do not declare `.RequireAuthorization()` or endpoint authorization attributes. This is an observation, not a recommendation: inspect the host's global authentication/authorization setup and the target endpoint before changing security behavior. No Customer-specific logging or exception middleware is present in the module source inspected.

References: [Customer list endpoint](../../../src/Modules/Customer/Modules.Customer.Presentation/Endpoints/Customers/GetCustomersEndpoint.cs), [Customer module registration](../../../src/Modules/Customer/Modules.Customer.Presentation/CustomerModule.cs).

## Pagination, filtering, and sorting

Customer list request extends `PagedParamData` and adds `SalonId` and optional `Q`. The query validator requires a positive salon ID and limits `Q` to 250 characters. The repository scopes results to that salon, trims `Q`, searches `FullName`, `NationalCode`, or `MobilePhone`, orders by `FullName`, applies the manual projection, and calls the existing `ToPagedList(...)` helper.

Customer also implements a separate paged SelectList flow at `GET /api/Customer/SelectList`. Its request DTO extends `PagedParamData` and requires a positive `SalonId` through the query validator; optional `Q` is limited to 250 characters. The query uses the same salon scope and search fields as the full list, additionally filters to active customers (`Status`), orders by full name, and projects only `Id`, `FullName`, and `Status` through `CustomerMapper.ToGetSelectListDto()`. It returns `PagedList<GetSelectListCustomerResponseDto>` via `ToPagedList(...)`.

Reuse the paging abstraction and verify its defaults and bounds before changing behavior. Add filters only when required, validate them at the application boundary, and keep filtering/sorting database-translatable.

For a SelectList feature, verify which records are eligible and the minimum fields its caller needs; do not blindly copy Customer's active-only filter, salon scope, or search fields to a different domain. Keep the select projection explicit and database-translatable, and wire the request DTO, query contract, handler/validator, repository contract/implementation, mapper, and endpoint consistently.

References: [Customer list request](../../../src/Modules/Customer/Modules.Customer.Application.Contract/DTOs/Customers/GetAll/GetCustomersRequestDto.cs), [list query handler](../../../src/Modules/Customer/Modules.Customer.Application/UseCase/Customers/Queries/GetAll/GetCustomersQueryHandler.cs), [SelectList request](../../../src/Modules/Customer/Modules.Customer.Application.Contract/DTOs/Customers/GetAll/GetSelectListCustomerRequestDto.cs), [SelectList response](../../../src/Modules/Customer/Modules.Customer.Application.Contract/DTOs/Customers/GetAll/GetSelectListCustomerResponseDto.cs), [SelectList query handler and validator](../../../src/Modules/Customer/Modules.Customer.Application/UseCase/Customers/Queries/GetSelectList/GetSelectListCustomerQueryHandler.cs), [SelectList endpoint](../../../src/Modules/Customer/Modules.Customer.Presentation/Endpoints/Customers/GetSelectListCustomersEndpoint.cs), [query repository](../../../src/Modules/Customer/Modules.Customer.Persistence/Repositories/Customers/CustomerQueryRepository.cs), [manual select projection](../../../src/Modules/Customer/Modules.Customer.Persistence/Mapper/Customers/CustomerMapper.cs).

## Dependency injection, services, and logging

`AddCustomerApplicationServices()` uses Scrutor to scan the Application assembly for command handlers, query handlers, and validators and registers them scoped. `AddCustomerInfrastructure(...)` registers EF contexts and command/query repositories scoped. The presentation module registers those extensions and scans endpoint implementations as singletons.

Customer has no separate service interface/implementation and no module-local logging calls. When another feature does require a service, inspect that module and shared DI patterns first; do not create services or logging infrastructure merely to match a generic architecture checklist. Use the project's existing logging/error handling if present, and never log sensitive customer data.

References: [Application DI](../../../src/Modules/Customer/Modules.Customer.Application/DependencyInjection.cs), [Persistence DI](../../../src/Modules/Customer/Modules.Customer.Persistence/DependencyInjection.cs).

## Testing and verification

No Customer-specific test project or test files were found in the solution during this analysis. Do not claim tests exist or invent a testing framework. If relevant tests exist for the target module, run focused tests; otherwise build the affected project(s), then the host if module registration or composition changed. Add tests only using a verified existing test convention or an explicitly requested testing setup.

## Pre-implementation checklist

- [ ] Have I inspected the target module and all directly related layers/callers?
- [ ] Have I traced the existing feature path from endpoint through persistence (or verified that this feature does not use that path)?
- [ ] Have I verified actual abstractions, namespaces, signatures, routes, and project references?
- [ ] Does the requirement need a repository, mapper, service, endpoint, context, or new project—or does an existing abstraction already cover it?
- [ ] Have I checked validation, authorization, errors, and data constraints at the correct boundaries?
- [ ] Are schema and migration consequences understood, including cross-module relationships?
- [ ] Am I avoiding unrelated changes and copying any identified inconsistency as if it were a standard?

## Post-implementation checklist

- [ ] Feature behavior is wired end-to-end, with cancellation and explicit error paths preserved.
- [ ] DTOs, validators, handlers, mapping, DI, endpoints, and persistence are consistent with verified target conventions.
- [ ] New/updated migration was inspected for correct tables, columns, indexes, and FKs.
- [ ] Affected project(s) and host build successfully; relevant tests ran when available.
- [ ] No unrelated files, packages, abstractions, logging of sensitive data, or unverified API behavior were introduced.
- [ ] Final summary states exact files, checks run, and known limitations or pre-existing warnings.
