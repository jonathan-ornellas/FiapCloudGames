FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["FiapCloudGames.Users.Api/FiapCloudGames.Users.Api.csproj", "FiapCloudGames.Users.Api/"]
COPY ["FiapCloudGames.Shared/FiapCloudGames.Shared.csproj", "FiapCloudGames.Shared/"]
COPY ["FiapCloudGames.EventSourcing/FiapCloudGames.EventSourcing.csproj", "FiapCloudGames.EventSourcing/"]

RUN dotnet restore "FiapCloudGames.Users.Api/FiapCloudGames.Users.Api.csproj"

COPY . .
WORKDIR "/src/FiapCloudGames.Users.Api"
RUN dotnet build "FiapCloudGames.Users.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FiapCloudGames.Users.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "FiapCloudGames.Users.Api.dll"]
