# syntax=docker/dockerfile:1
# Build from the repository root:  docker build -f src/CleanArch.Api/Dockerfile -t cleanarch-api .

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore first, from project files only, so the layer is cached until a csproj/props file changes.
COPY global.json Directory.Build.props Directory.Packages.props ./
COPY src/CleanArch.Api/CleanArch.Api.csproj src/CleanArch.Api/
COPY src/CleanArch.Application/CleanArch.Application.csproj src/CleanArch.Application/
COPY src/CleanArch.CrossCuttingConcerns/CleanArch.CrossCuttingConcerns.csproj src/CleanArch.CrossCuttingConcerns/
COPY src/CleanArch.Domain/CleanArch.Domain.csproj src/CleanArch.Domain/
COPY src/CleanArch.Infrastructure/CleanArch.Infrastructure.csproj src/CleanArch.Infrastructure/
COPY src/CleanArch.Persistence/CleanArch.Persistence.csproj src/CleanArch.Persistence/
RUN dotnet restore src/CleanArch.Api/CleanArch.Api.csproj

COPY src/ src/
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish src/CleanArch.Api/CleanArch.Api.csproj \
    --configuration $BUILD_CONFIGURATION \
    --no-restore \
    --output /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

COPY --from=build /app/publish .

# Serilog's file sink writes to ./Logs; the image's non-root user needs to own it.
RUN mkdir -p /app/Logs && chown -R $APP_UID /app/Logs
USER $APP_UID

ENTRYPOINT ["dotnet", "CleanArch.Api.dll"]
