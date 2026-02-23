# Development setup instructions

[![.NET](https://img.shields.io/badge/.NET-10.0-purple.svg)](https://dotnet.microsoft.com/)
[![EF Core](https://img.shields.io/badge/EF_Core-10.0-blue.svg)](https://docs.microsoft.com/en-us/ef/core/)
[![Docker](https://img.shields.io/badge/docker-required-blue.svg)](https://www.docker.com/)

This document contains essential commands and configurations for developers working on DockiUp.

## Development Environment Setup

### User Secrets Configuration

Add the following to your user secrets:

```json
{
  "ConnectionStrings": {
    "DockiUpDatabase": "Host=localhost;Port=5432;Database=dockiupdb-dev;Username=postgres;Password=d4vpas8w0rd13!!!;"
  },
  "SystemPaths": {
    "DockerSocket": "",
    "ProjectsPath": ""
  },
  "JWT_SECRET_KEY": ""
}
```

## Docker Commands

### Start Development Database

```sh
docker compose -f compose.dev.yml up -d
```

## Entity Framework (EF) Migrations

**When you change any database entity** (e.g. in `DockiUp.Domain` or `DockiUp.Infrastructure` / `DockiUpDbContext`), **create a new migration** so the schema stays in sync.

### Add a migration (recommended: use script)

From the repository root:

```powershell
.\scripts\Db-Script.ps1 -Command add-migration
```

Enter a descriptive name when prompted (e.g. `AddLastPeriodicUpdateAt`, `AddProjectSettings`).

### Add a migration (manual)

From the repository root:

```sh
dotnet ef migrations add <MigrationName> --project src/DockiUp.Infrastructure --startup-project src/DockiUp.API
```

Example:

```sh
dotnet ef migrations add AddNewColumn --project src/DockiUp.Infrastructure --startup-project src/DockiUp.API
```

Migrations are generated under `src/DockiUp.Infrastructure/Migrations`. The API applies them on startup.
