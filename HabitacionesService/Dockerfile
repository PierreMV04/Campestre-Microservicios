# =========================
# Build stage
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY HabitacionesService/HabitacionesService.csproj HabitacionesService/
COPY Shared.Data/Shared.Data.csproj Shared.Data/
COPY Shared.EventBus/Shared.EventBus.csproj Shared.EventBus/

RUN dotnet restore HabitacionesService/HabitacionesService.csproj

COPY . .
WORKDIR /src/HabitacionesService
RUN dotnet publish -c Release -o /app/publish

# =========================
# Runtime stage
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "HabitacionesService.dll"]
