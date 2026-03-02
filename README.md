# HealthTrack - Patient Portal API

A HIPAA-aware Patient Portal REST API built with ASP.NET Core 9, demonstrating enterprise .NET patterns including Clean Architecture, CQRS with MediatR, audit logging, and JWT authentication.

## Tech Stack

| Layer | Technology |
|-------|-----------|
| **Framework** | ASP.NET Core 9 / .NET 9 |
| **Architecture** | Clean Architecture + CQRS |
| **Database** | PostgreSQL 16 (via EF Core 9) |
| **Caching** | Redis 7 |
| **Auth** | ASP.NET Identity + JWT Bearer |
| **Mediator** | MediatR 14 |
| **Validation** | FluentValidation 12 |
| **Mapping** | AutoMapper 16 |
| **Logging** | Serilog (Console + File sinks) |
| **API Docs** | Swagger / Swashbuckle |
| **Testing** | xUnit + FluentAssertions + Moq |
| **Containers** | Docker Compose |

## Architecture

```
src/
  HealthTrack.Domain/          # Entities, Value Objects, Enums, Interfaces (zero dependencies)
  HealthTrack.Application/     # CQRS Commands/Queries, Handlers, Validators, DTOs
  HealthTrack.Infrastructure/  # EF Core, Identity, Redis, Repositories, JWT
  HealthTrack.Api/             # Controllers, Middleware, Program.cs
tests/
  HealthTrack.Domain.Tests/
  HealthTrack.Application.Tests/
  HealthTrack.Infrastructure.Tests/
  HealthTrack.Api.Tests/
```

**Dependency Rule:** Domain <- Application <- Infrastructure <- Api

## Quick Start

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### 1. Start Infrastructure

```bash
docker compose up -d
```

This starts PostgreSQL (port 5433) and Redis (port 6380).

### 2. Run the API

```bash
dotnet run --project src/HealthTrack.Api
```

The API starts at **https://localhost:5001** with Swagger UI.

### 3. Open Swagger

Navigate to https://localhost:5001/swagger to explore all endpoints.

## Demo Accounts

| Email | Password | Role |
|-------|----------|------|
| admin@healthtrack.dev | Demo123! | Admin |
| provider@healthtrack.dev | Demo123! | Provider |
| patient@healthtrack.dev | Demo123! | Patient |

## API Endpoints

### Authentication
| Method | Route | Description |
|--------|-------|-------------|
| POST | `/api/v1/auth/register` | Register a new user |
| POST | `/api/v1/auth/login` | Login and get JWT token |
| POST | `/api/v1/auth/refresh` | Refresh an expired token |

### Patients
| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| GET | `/api/v1/patients` | Any | List patients (paginated, searchable) |
| GET | `/api/v1/patients/{id}` | Any | Get patient details |
| POST | `/api/v1/patients` | Provider/Admin | Create a patient |
| PUT | `/api/v1/patients/{id}` | Provider/Admin | Update a patient |
| DELETE | `/api/v1/patients/{id}` | Admin | Delete a patient |

### Appointments
| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| GET | `/api/v1/appointments` | Any | List appointments with filters |
| GET | `/api/v1/appointments/{id}` | Any | Get appointment details |
| POST | `/api/v1/appointments` | Any | Book an appointment |
| PATCH | `/api/v1/appointments/{id}/cancel` | Any | Cancel an appointment |
| PATCH | `/api/v1/appointments/{id}/reschedule` | Any | Reschedule an appointment |

### Prescriptions
| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| GET | `/api/v1/prescriptions` | Any | List prescriptions |
| GET | `/api/v1/prescriptions/{id}` | Any | Get prescription details |
| POST | `/api/v1/prescriptions` | Provider/Admin | Create a prescription |
| POST | `/api/v1/prescriptions/{id}/refill` | Any | Request a refill |
| POST | `/api/v1/prescriptions/check-interactions` | Any | Check drug interactions |

### Clinical Notes
| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| GET | `/api/v1/clinical-notes` | Any | List clinical notes |
| GET | `/api/v1/clinical-notes/{id}` | Any | Get note details |
| POST | `/api/v1/clinical-notes` | Provider/Admin | Create a SOAP note |
| PUT | `/api/v1/clinical-notes/{id}/amend` | Provider/Admin | Amend a note (versioned) |

### Providers
| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| GET | `/api/v1/providers` | Any | List providers |
| GET | `/api/v1/providers/{id}` | Any | Get provider details |
| POST | `/api/v1/providers` | Admin | Create a provider |
| PUT | `/api/v1/providers/{id}` | Admin | Update a provider |

### Audit & Health
| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| GET | `/api/v1/audit-logs` | Admin | Query audit trail |
| GET | `/api/v1/health` | None | API health check |

## Features

- **Clean Architecture** - Strict dependency rule, domain-centric design
- **CQRS Pattern** - Separate command/query models via MediatR
- **Pipeline Behaviors** - Cross-cutting validation, logging, caching, audit
- **JWT Authentication** - Bearer tokens with refresh token rotation
- **Role-Based Authorization** - Admin, Provider, Patient, Nurse, Staff roles
- **Audit Logging** - Automatic tracking of all data mutations
- **Drug Interaction Checking** - Mock service for medication safety
- **Clinical Note Versioning** - Immutable amendment history (HIPAA compliance)
- **Redis Caching** - Distributed cache for read-heavy queries
- **Comprehensive Seeding** - 10 patients, 3 providers, 20 appointments, 15 prescriptions, 10 clinical notes

## Development Commands

```bash
# Build
dotnet build

# Run
dotnet run --project src/HealthTrack.Api

# Test
dotnet test

# Add EF Core migration
dotnet ef migrations add <Name> --project src/HealthTrack.Infrastructure --startup-project src/HealthTrack.Api

# Apply migrations
dotnet ef database update --project src/HealthTrack.Infrastructure --startup-project src/HealthTrack.Api
```

## Ports

| Service | Port |
|---------|------|
| API (HTTPS) | 5001 |
| API (HTTP) | 5000 |
| PostgreSQL | 5433 |
| Redis | 6380 |

## Seed Data

The database is automatically seeded on first run with:

- **3 Providers** - Dr. Sarah Chen (Cardiology), Dr. James Wilson (Family Medicine), Dr. Maria Rodriguez (Pediatrics)
- **10 Patients** - Diverse demographics with varied medical histories, allergies, and medications
- **20 Appointments** - Mix of completed, scheduled, cancelled, and recurring
- **15 Prescriptions** - Various medications and statuses including refill requests
- **10 Clinical Notes** - SOAP format examples across different note types
- **5 Pharmacies** - Local pharmacy data
- **3 Users** - Admin, Provider, and Patient demo accounts
