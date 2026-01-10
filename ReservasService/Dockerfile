# =========================
# BUILD STAGE
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ReservasService/ReservasService.csproj ReservasService/
COPY Shared.Data/Shared.Data.csproj Shared.Data/
COPY Shared.DTOs/Shared.DTOs.csproj Shared.DTOs/
COPY Shared.EventBus/Shared.EventBus.csproj Shared.EventBus/

RUN dotnet restore ReservasService/ReservasService.csproj

COPY . .
WORKDIR /src/ReservasService
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# =========================
# RUNTIME STAGE
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Docker

EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ReservasService.dll"]
