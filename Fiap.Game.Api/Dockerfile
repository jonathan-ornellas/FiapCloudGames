# =========================
# 🔹 BUILD STAGE
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Fiap.Game.sln", "./"]
COPY ["Fiap.Game.Api/Fiap.Game.Api.csproj", "Fiap.Game.Api/"]
COPY ["Fiap.Game.Domain/Fiap.Game.Domain.csproj", "Fiap.Game.Domain/"]
COPY ["Fiap.Game.Business/Fiap.Game.Business.csproj", "Fiap.Game.Business/"]
COPY ["Fiap.Game.Infra.Data/Fiap.Game.Infra.Data.csproj", "Fiap.Game.Infra.Data/"]
COPY ["Fiap.Game.Infra.CrossCutting/Fiap.Game.Infra.CrossCutting.csproj", "Fiap.Game.Infra.CrossCutting/"]

# Restaura dependências
RUN dotnet restore "Fiap.Game.Api/Fiap.Game.Api.csproj"

# Copia o restante do código
COPY . .

# Compila a aplicação
WORKDIR "/src/Fiap.Game.Api"
RUN dotnet build "Fiap.Game.Api.csproj" -c Release -o /app/build

# =========================
# 🔹 PUBLISH STAGE
# =========================
FROM build AS publish
RUN dotnet publish "Fiap.Game.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# =========================
# 🔹 RUNTIME STAGE
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Instala o curl (para healthcheck)
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copia os arquivos publicados
COPY --from=publish /app/publish .

# Garante que o Kestrel escute na porta 80 
ENV ASPNETCORE_URLS=http://+:80

ENV ASPNETCORE_ENVIRONMENT=Docker

# Cria um usuário não-root
RUN groupadd -r appuser && useradd -r -g appuser appuser && chown -R appuser:appuser /app

EXPOSE 80

ENTRYPOINT ["dotnet", "Fiap.Game.Api.dll"]
