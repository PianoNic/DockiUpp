# DockiUp API (Komodo-style)

DockiUp manages Docker Compose projects and containers in a Komodo-inspired way: projects/stacks, container lifecycle, webhooks, and periodic updates.

## Configuration (Komodo-style)

- **SystemPaths:ProjectsPath** – Base directory for project folders.
- **SystemPaths:DockerSocket** – Optional Docker socket (e.g. `unix:///var/run/docker.sock`). If empty, default is used.
- **Webhook:WebhookSecret** – Optional secret for `POST /api/webhook`. If set, requests must send `X-Webhook-Secret` or `?secret=`.

## Projects (stacks)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/Project/GetProjects | List all projects with containers |
| GET | /api/Project/GetProject?projectId=1 or &dockerProjectName=myapp | Get one project |
| POST | /api/Project/DeployProject | Deploy new project (body: SetupProjectDto) |
| GET | /api/Project/StopProject?projectId=1 or &dockerProjectName=myapp | Stop project |
| POST | /api/Project/RestartProject?projectId=1 or &dockerProjectName=myapp | Restart project |
| POST | /api/Project/UpdateProject?projectId=1 | Pull (if Git) and restart |

## Containers

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/Container/GetContainer?containerId=... | Get container by id |
| POST | /api/Container/StartContainer?containerId=... | Start container |
| POST | /api/Container/StopContainer?containerId=... | Stop container |
| POST | /api/Container/RestartContainer?containerId=... | Restart container |

## Webhook

- **POST /api/Webhook?projectId=1**  
  Triggers update (pull + restart) for the project. If `Webhook:WebhookSecret` is set, send it in header `X-Webhook-Secret` or query `secret`.

## App

- **GET /api/App/GetAppInfo** – App version and environment.

## Periodic updates

Projects with **Update method = Periodically** and **Periodic interval** set are updated automatically on a poll (default every 1 minute). Each project runs at most once per its interval (e.g. every 60 minutes).

## Frontend API client (DockiUpp Frontend / OpenAPI Generator)

The **DockiUpp Frontend** (`src/DockiUp.Frontend`) uses **@openapitools/openapi-generator-cli** to generate a TypeScript Angular API client from the DockiUp OpenAPI spec. All API client generation happens in this repo only.

- **With backend running (recommended):** Start the DockiUp API (e.g. `dotnet run` in `src/DockiUp.API`, default URL `http://localhost:5098`), then from `src/DockiUp.Frontend` run:  
  `npm run apigen` or `npm run apigen:live`  
  Both fetch `/openapi/v1.json` from the running API and regenerate the client (AppService, ProjectService, ContainerService, etc.).

- **Without backend:** A copy of the spec is in `src/DockiUp.Frontend/openapi.json` for reference. For offline generation you can run `npx openapi-generator-cli generate --input-spec=openapi.json --generator-name=typescript-angular --output=src/app/api`; the backend OpenAPI document is the source of truth for the correct service split.

After generation, the client is under `src/app/api` (models and services). Re-run `apigen` or `apigen:live` after API changes to refresh the client.
