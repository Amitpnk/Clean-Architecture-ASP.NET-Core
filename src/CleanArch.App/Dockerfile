# syntax=docker/dockerfile:1
# Build from the repository root:  docker build -f src/CleanArch.App/Dockerfile -t cleanarch-web .
# Run:  docker run -p 8081:80 -e API_BASE_URL=http://localhost:8080/ cleanarch-web

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY global.json Directory.Build.props Directory.Packages.props ./
COPY src/CleanArch.App/CleanArch.App.csproj src/CleanArch.App/
RUN dotnet restore src/CleanArch.App/CleanArch.App.csproj

# Copy all of src/ (as the API image does): narrower COPY paths here trip a BuildKit
# "changes out of order" error on Docker Desktop for Windows.
COPY src/ src/
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish src/CleanArch.App/CleanArch.App.csproj \
    --configuration $BUILD_CONFIGURATION \
    --no-restore \
    --output /app/publish

# Blazor WebAssembly publishes static files only, so any static web server will do.
FROM nginx:1.27-alpine AS final

# The nginx image renders /etc/nginx/templates/*.template into conf.d at startup, substituting
# environment variables. That is how API_BASE_URL reaches the browser through /appsettings.json.
ENV API_BASE_URL=http://localhost:8080/
COPY src/CleanArch.App/nginx.conf.template /etc/nginx/templates/default.conf.template
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html

EXPOSE 80
