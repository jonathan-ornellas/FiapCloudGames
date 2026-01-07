FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["FiapCloudGames.Games.Api/FiapCloudGames.Games.Api.csproj", "FiapCloudGames.Games.Api/"]
COPY ["FiapCloudGames.Shared/FiapCloudGames.Shared.csproj", "FiapCloudGames.Shared/"]
COPY ["FiapCloudGames.EventSourcing/FiapCloudGames.EventSourcing.csproj", "FiapCloudGames.EventSourcing/"]

RUN dotnet restore "FiapCloudGames.Games.Api/FiapCloudGames.Games.Api.csproj"

COPY . .
WORKDIR "/src/FiapCloudGames.Games.Api"
RUN dotnet build "FiapCloudGames.Games.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FiapCloudGames.Games.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "FiapCloudGames.Games.Api.dll"]
