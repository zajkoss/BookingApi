# BookingApi

REST API for managing resource reservations (conference rooms, desks, etc.).
Portfolio project demonstrating patterns and technologies used in modern backend applications.

## Tech Stack

- **Runtime:** .NET 10, ASP.NET Core
- **Database:** PostgreSQL 16 + Entity Framework Core (Code First, migrations)
- **Cache:** Redis 7
- **Validation:** FluentValidation
- **Object Mapping:** AutoMapper
- **Containerization:** Docker, Docker Compose
- **API Documentation:** Scalar (OpenAPI)
- **Version Control:** Git

## Features

- **CRUD** for Resources and Bookings
- **Pessimistic locking** (`SELECT FOR UPDATE`) — prevents double booking of the same resource
- **Optimistic locking** (Version field) — prevents lost updates on concurrent modifications
- **Redis caching** — cache invalidation on create/update/delete
- **Database migrations** — automatic on application startup (EF Core Code First)
- **Validation** — FluentValidation with async validators (e.g. resource existence check)
- **Global exception handler** — maps exceptions to HTTP status codes (409, 400, 500)
- **Health checks** — `/health` endpoint for PostgreSQL and Redis
- **Dockerized** — multi-stage Dockerfile + Docker Compose with healthchecks and proper startup order

## Architecture

- **Layered architecture** — Controller → Service → Repository
- **CQRS-inspired naming** — Commands for write operations (`CreateResourceCommand`, `CreateBookingCommand`), DTOs for read operations
- **Dependency Injection** — built-in ASP.NET Core DI container, interface-based abstractions
- **Generic Repository** — base repository with common CRUD operations, extended by domain-specific repositories
- **Cache-aside pattern** — services check Redis before hitting the database, cache invalidated on write operations
- **Containerization** — application, database and cache running in separate Docker containers

## Getting Started

### Prerequisites
- Docker
- Docker Compose

### Run

```bash
docker compose up --build
```

API available at `http://localhost:8080/scalar/v1`

## Roadmap

### In Progress
- Unit tests (xUnit + Moq + FluentAssertions)
- Integration tests (Testcontainers)
- Decorator pattern for cache (`CachedResourceService`)
- Circuit breaker for Redis

### Planned
- Unit of Work pattern
- Audit interceptor (`CreatedAt`/`UpdatedAt` automation)
- Extension methods for `Program.cs` service registration
- JWT authentication
- Users entity (replace `BookedBy` string)
- Many-to-many User ↔ Resource
- AWS S3 — resource image uploads
- Booking search/filtering (Specification pattern or Dynamic LINQ)
- i18n — validation messages from `.resx` files
- `DbUpdateConcurrencyException` handling (optimistic lock)

### Future (v3)
- Kafka — event-driven architecture (`BookingCreated`, `BookingCancelled` events)
- Streaming endpoint with cursor pagination (`IAsyncEnumerable`)