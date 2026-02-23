# Use base ASP.NET Core runtime (.NET 10)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Frontend build stage (DockiUp.Frontend - in-repo at src/DockiUp.Frontend)
FROM node:22 AS frontend-build
WORKDIR /src/frontend
COPY src/DockiUp.Frontend/package*.json ./
COPY src/DockiUp.Frontend/ ./
RUN npm install
RUN npx ng build --configuration=production 2>/dev/null || npm run build -- --configuration=production
RUN mkdir -p /artifacts/frontend && (cp -r dist/DockiUp.Frontend/browser/* /artifacts/frontend/ 2>/dev/null || cp -r dist/*/browser/* /artifacts/frontend/ 2>/dev/null || cp -r dist/* /artifacts/frontend/ || true)

# Backend build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/DockiUp.API/DockiUp.API.csproj", "src/DockiUp.API/"]
COPY ["src/DockiUp.Application/DockiUp.Application.csproj", "src/DockiUp.Application/"]
COPY ["src/DockiUp.Infrastructure/DockiUp.Infrastructure.csproj", "src/DockiUp.Infrastructure/"]
COPY ["src/DockiUp.Domain/DockiUp.Domain.csproj", "src/DockiUp.Domain/"]
RUN dotnet restore "src/DockiUp.API/DockiUp.API.csproj"
COPY . .
WORKDIR "/src/src/DockiUp.API"
RUN dotnet build "DockiUp.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "DockiUp.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
COPY --from=frontend-build /artifacts/frontend /app/publish/wwwroot

# Final production stage
FROM base AS final
WORKDIR /app
RUN apt-get update && \
    apt-get upgrade -y && \
    apt-get install -y git
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DockiUp.API.dll"]
