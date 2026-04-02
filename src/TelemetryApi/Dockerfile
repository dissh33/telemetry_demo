FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/TelemetryContracts/TelemetryContracts.csproj", "src/TelemetryContracts/"]
COPY ["src/TelemetryApi/TelemetryApi.csproj", "src/TelemetryApi/"]
RUN dotnet restore "src/TelemetryApi/TelemetryApi.csproj"
COPY src/TelemetryContracts/ src/TelemetryContracts/
COPY src/TelemetryApi/ src/TelemetryApi/
WORKDIR "/src/src/TelemetryApi"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TelemetryApi.dll"]
