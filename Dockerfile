# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY HealthTrack.sln .
COPY src/HealthTrack.Domain/HealthTrack.Domain.csproj src/HealthTrack.Domain/
COPY src/HealthTrack.Application/HealthTrack.Application.csproj src/HealthTrack.Application/
COPY src/HealthTrack.Infrastructure/HealthTrack.Infrastructure.csproj src/HealthTrack.Infrastructure/
COPY src/HealthTrack.Api/HealthTrack.Api.csproj src/HealthTrack.Api/

# Restore dependencies
RUN dotnet restore src/HealthTrack.Api/HealthTrack.Api.csproj

# Copy everything else
COPY . .

# Build and publish
RUN dotnet publish src/HealthTrack.Api/HealthTrack.Api.csproj -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "HealthTrack.Api.dll"]
