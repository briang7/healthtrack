# CLAUDE.md

## Project: HealthTrack Patient Portal API

HIPAA-aware Patient Portal REST API using ASP.NET Core 9 with Clean Architecture.

## Build & Development Commands

```bash
dotnet build                                    # Build all projects
dotnet run --project src/HealthTrack.Api         # Run the API (https://localhost:5001)
dotnet test                                      # Run all tests
docker compose up -d                             # Start PostgreSQL + Redis
docker compose down                              # Stop containers
```

## Ports

- API: https://localhost:5001, http://localhost:5000
- PostgreSQL: localhost:5433 (user: healthtrack, pass: healthtrack_dev, db: healthtrack_dev)
- Redis: localhost:6380

## Architecture

Clean Architecture with CQRS (MediatR):

```
src/HealthTrack.Domain/          -> Pure C#, zero dependencies
src/HealthTrack.Application/     -> MediatR, FluentValidation, AutoMapper
src/HealthTrack.Infrastructure/  -> EF Core, Identity, Redis, Repositories
src/HealthTrack.Api/             -> Controllers, Middleware, Swagger
```

Dependency rule: Domain <- Application <- Infrastructure <- Api

## Key Patterns

- **CQRS**: Commands and Queries in `Application/Features/{Feature}/Commands|Queries/`
- **Pipeline Behaviors**: ValidationBehavior, LoggingBehavior, AuditBehavior, CachingBehavior
- **Repository + UnitOfWork**: Defined in Domain interfaces, implemented in Infrastructure
- **Value Objects**: Address, PhoneNumber, DateOfBirth, InsuranceInfo, EmergencyContact (records)
- **Entity Configurations**: One per entity in `Infrastructure/Persistence/Configurations/`

## Demo Accounts

- admin@healthtrack.dev / Demo123! (Admin role)
- provider@healthtrack.dev / Demo123! (Provider role)
- patient@healthtrack.dev / Demo123! (Patient role)

## EF Core Migrations

```bash
dotnet ef migrations add <Name> --project src/HealthTrack.Infrastructure --startup-project src/HealthTrack.Api
dotnet ef database update --project src/HealthTrack.Infrastructure --startup-project src/HealthTrack.Api
```
