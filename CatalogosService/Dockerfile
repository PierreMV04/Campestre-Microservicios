# =========================
# BUILD
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar csproj y restaurar dependencias
COPY CatalogosService/CatalogosService.csproj CatalogosService/
COPY Shared.Data/Shared.Data.csproj Shared.Data/
COPY Shared.DTOs/Shared.DTOs.csproj Shared.DTOs/
COPY Shared.EventBus/Shared.EventBus.csproj Shared.EventBus/

RUN dotnet restore CatalogosService/CatalogosService.csproj

# Copiar todo el código
COPY . .

# Publicar
WORKDIR /src/CatalogosService
RUN dotnet publish -c Release -o /app/publish

# =========================
# RUNTIME
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

# Puerto interno del contenedor
EXPOSE 8080

# Variables de entorno
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Docker

ENTRYPOINT ["dotnet", "CatalogosService.dll"]
