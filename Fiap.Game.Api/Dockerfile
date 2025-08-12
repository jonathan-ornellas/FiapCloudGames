# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["Fiap.Game.sln", "./"]
COPY ["Fiap.Game.Api/Fiap.Game.Api.csproj", "Fiap.Game.Api/"]
COPY ["Fiap.Game.Domain/Fiap.Game.Domain.csproj", "Fiap.Game.Domain/"]
COPY ["Fiap.Game.Business/Fiap.Game.Business.csproj", "Fiap.Game.Business/"]
COPY ["Fiap.Game.Infra.Data/Fiap.Game.Infra.Data.csproj", "Fiap.Game.Infra.Data/"]
COPY ["Fiap.Game.Infra.CrossCutting/Fiap.Game.Infra.CrossCutting.csproj", "Fiap.Game.Infra.CrossCutting/"]

# Restore dependencies
RUN dotnet restore "Fiap.Game.Api/Fiap.Game.Api.csproj"

# Copy source code
COPY . .

# Build application
WORKDIR "/src/Fiap.Game.Api"
RUN dotnet build "Fiap.Game.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "Fiap.Game.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published application
COPY --from=publish /app/publish .

# Create non-root user
RUN groupadd -r appuser && useradd -r -g appuser appuser && chown -R appuser:appuser /app
USER appuser

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=30s --retries=3 \
  CMD curl -f http://localhost/health || exit 1

EXPOSE 80
ENTRYPOINT ["dotnet", "Fiap.Game.Api.dll"]

