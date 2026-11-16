![.NET Core](https://github.com/Amitpnk/Clean-Architecture-ASP.NET-Core/workflows/.NET%20Core/badge.svg)

# Complete solution for Clean architecture using .NET Core

<br />
<p align="center">
  <a href="#">
    <img src="img/logo.png" alt="Logo" width="80" height="80">
  </a>

  <h3 align="center">Clean Architecture</h3>

  <p align="center">
    Complete solution template which is built on Clean Architecture with all essential feature using .NET Core!
    <br />
    <a href="https://amitpnk.github.io/Clean-Architecture-ASP.NET-Core"><strong>Explore the docs »</strong></a>
    <br />
    <br />
    <a href="https://marketplace.visualstudio.com/items?itemName=AmitNaik.CleanArchitecture">Download from Marketplace</a>
    ·
    <a href="https://github.com/Amitpnk/Clean-architecture-ASP.NET-Core/issues">Report Bug</a>
    ·
    <a href="https://github.com/Amitpnk/Clean-architecture-ASP.NET-Core/issues/new">Request Feature</a>
  </p>
</p>

## Give a Star! :star:
If you like or are using this project to learn or start your solution, please give it a star. Thanks!

## Support This Project

If you have found this project helpful, either as a library that you use or as a learning tool, please consider buying me a coffee:

<a href="https://www.buymeacoffee.com/codewithamit" target="_blank"><img src="https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png" alt="Buy Me A Coffee" style="height: 41px !important;width: 174px !important" ></a>

## Clean Architecture

Clean Architecture was introduced by Robert C. Martin (Uncle Bob) to provide a better way to build applications in perspective of better testability, maintainability, and dependability on the infrastructures like databases and services

Domain in center and building layer top of it. You can call it as Domain-centric Architecture too.

Though layer architectures like onion, Hexagonal all vary somewhat in their details, they are very similar. All DDD approach have the same objective, which is the separation of concerns. 

### Reference

* [Clean Architecture by Robert C. Martin (Uncle Bob)](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

## About The Project

This project provides complete solution which is built on Clean Architecture with all essential features, CI/CD, best practice, testing Strategy using .NET 10.

![image](img/clean-architecture.png)

## Technology stack

Architecture Pattern

- [x] Clean architecture
- [x] (Includes)
    - [x] Screaming architecture-[Functional organisation design](http://blog.cleancoder.com/uncle-bob/2011/09/30/Screaming-Architecture.html)
    - [x] Onion architecture

Design Pattern
- [x] CQRS design pattern
- [ ] Decorator design pattern
- [x] Mediator design pattern (lightweight custom CQRS dispatcher - no MediatR dependency)
    - [x] Dispatcher pipeline support (`IPipelineBehavior`) for cross-cutting steps around commands and queries
- [x] Repository design pattern
- [x] Generic Repository pattern (`IGenericRepositoryAsync<T>`)
- [x] Value Object pattern
- [x] Options pattern (`IOptions<JwtSettings>`, `IOptions<MailSettings>`)
- [x] Unit of work 
- [x] Factory design pattern
 
Backend
- [x] Language: C#
- [x] Framework: .NET 10 (LTS), ASP.NET Core 10

UI
- [x] Blazor WebAssembly client (`CleanArch.App`) calling the Web API
    - [x] JWT sign-in, registration and email confirmation, with protected routes
    - [x] Dashboard, categories, and events (list, details, create, edit, delete, CSV export)
    - [x] API errors (RFC 9457 problem details) shown inline, with validation messages

Database
- [x] MS SQL and Inmemory DB
- [x] DB Connectivity : Entity Framework Core 10 - Code First (migrations included)

Cloud server
- [x] Azure (deployment script)

Service
- [x] Web API (Restful service)
- [ ] gRPC
- [ ] Graphql

Feature
- [x] Dataseeding
- [x] Global exception handler (`IExceptionHandler` + RFC 9457 `ProblemDetails`)
- [x] Mapster (object mapping)
- [x] Fluent validation
- [x] Serilog (console + rolling file)
- [x] Swagger UI (Swashbuckle, OpenAPI 3)
- [x] Advanced Pagination
- [x] API Versioning (Asp.Versioning)
- [x] User Auditing (audit columns + AuditTrails table)
- [x] Mailkit (Mail service)
- [ ] Hangfire
- [x] Enabling CORS with CustomPolicy
- [x] Feature flags (Microsoft.FeatureManagement, e.g. `EnableEmailService`)
- [x] CSV export (CsvHelper) with a custom Swagger operation filter for file responses
- [x] `/info` endpoint (`MetaController`)
- [ ] BenchmarkDotNet

Authentication
- [x] JWT Authentication with ASP.NET Core Identity
- [x] Account flows: register, email confirmation, forgot password and reset password
- [ ] Refresh tokens (tokens are generated and returned; `/refresh-token` endpoint pending)
- [x] Role seeding (default roles and users); role/policy-based authorization pending

Monitoring tool
- [x] Health check UI
- [x] Health checks: EF DbContext, SQL Server and external URL, exposed at `/healthz`
- [ ] Kibana dashboard  

Testing
- [x] Unit testing for every layer (xUnit v3, Moq, EF Core InMemory)
- [x] Integration testing (xUnit v3 + Testcontainers: real SQL Server in Docker; `WebApplicationFactory` for the API)

Caching
- [ ] Inmemory Caching
- [ ] Distributed Redis Caching

CI/CD
- [x] Task runner: GitHub Actions and CircleCI
- [x] CI quality gates: GitHub Actions matrix build (Debug and Release), coverlet coverage collection, EF Core migrations up-to-date check
- [ ] Coverage report: Sonarcloud.io
- [ ] Quality report: Codacy
- [x] Docker image (multi-stage Dockerfile) and docker-compose (API + SQL Server)
- [ ] Kubernetes
- [ ] Cloud server: Azure for cloud

Documentation
- [x] Conventional commit - commit and commit message    
- [x] Docsify

Tooling
- [x] Central package management (Directory.Packages.props) with NuGet vulnerability audit
- [x] SDK pinned via global.json; dotnet-ef as a local tool (dotnet-tools.json)

## Running with Docker

```bash
docker compose up --build
```

This starts SQL Server 2022, the API and the Blazor UI; migrations and seed data are applied on startup.

| What | URL |
| --- | --- |
| Blazor UI | http://localhost:8081 |
| Swagger | http://localhost:8080/swagger |
| Health check UI | http://localhost:8080/healthcheck-ui |

Sign in with `superadmin@gmail.com` / `Password@123`. Override the SQL `sa` password with the `SA_PASSWORD` environment variable.
The UI container reads the API's address from `API_BASE_URL` (default `http://localhost:8080/`).

## Running locally

Start the API and the Blazor UI in two terminals (the API needs SQL Server; see `ConnectionStrings` in `appsettings.json`):

```bash
dotnet run --project src/CleanArch.Api --launch-profile CleanArch.Api   # https://localhost:5001
dotnet run --project src/CleanArch.App                                  # https://localhost:7200
```

The UI calls the API at `ApiBaseUrl` in `src/CleanArch.App/wwwroot/appsettings.json`. The UI's origin must be listed in
the API's `CORS:AllowedOrigins`; `https://localhost:7200`, `http://localhost:5200` and `http://localhost:8081` already are.

## Tests

| Project | Framework | Covers |
| --- | --- | --- |
| `CleanArch.Domain.UnitTests` | xUnit v3 | `BaseResponse`, `ValueObject`, entities, request validation attributes |
| `CleanArch.Application.UnitTests` | xUnit v3 | Every command/query handler and validator, CQRS dispatcher, mappings, exceptions |
| `CleanArch.Infrastructure.UnitTests` | xUnit v3 | CSV export, logged-in user, clock, service registration |
| `CleanArch.Persistence.UnitTests` | xUnit v3 | Repositories, audit trail, seed data, `AuthenticationService` (real Identity over EF InMemory) |
| `CleanArch.Api.UnitTests` | xUnit v3 | Controllers (mocked dispatcher), global exception handler, Swagger file-result filter |
| `CleanArch.App.UnitTests` | xUnit v3 | Blazor client: API client and error mapping, JWT parsing, auth state, bearer-token handler |
| `CleanArch.Persistence.IntegrationTests` | xUnit v3 | Migrations and repositories against real SQL Server |
| `CleanArch.Api.IntegrationTests` | xUnit v3 | HTTP endpoints and JWT auth against real SQL Server |

The integration tests use [Testcontainers](https://dotnet.testcontainers.org/) to start a throwaway SQL Server
container per test assembly, so Docker must be running. To run only the unit tests:

```bash
dotnet test CleanArch.sln --filter "FullyQualifiedName!~IntegrationTests"
```

## Licence Used

[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/Amitpnk/Clean-architecture-ASP.NET-Core/blob/develop/LICENSE)

See the contents of the LICENSE file for details

## Contact

Having any issues or troubles getting started? Drop a mail to amit.naik8103@gmail.com or [Raise a Bug or Feature Request](https://github.com/Amitpnk/Clean-architecture-ASP.NET-Core/issues/new). Always happy to help.
