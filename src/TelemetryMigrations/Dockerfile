FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/TelemetryMigrations/TelemetryMigrations.csproj", "src/TelemetryMigrations/"]
RUN dotnet restore "src/TelemetryMigrations/TelemetryMigrations.csproj"
COPY src/TelemetryMigrations/ src/TelemetryMigrations/
WORKDIR "/src/src/TelemetryMigrations"
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TelemetryMigrations.dll"]
