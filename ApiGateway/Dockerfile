# ===============================
# BUILD
# ===============================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ApiGateway/ApiGateway.csproj ApiGateway/
COPY Shared.Data/Shared.Data.csproj Shared.Data/
COPY Shared.DTOs/Shared.DTOs.csproj Shared.DTOs/
COPY Shared.EventBus/Shared.EventBus.csproj Shared.EventBus/

RUN dotnet restore ApiGateway/ApiGateway.csproj

COPY . .
WORKDIR /src/ApiGateway
RUN dotnet publish -c Release -o /app/publish

# ===============================
# RUNTIME
# ===============================
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "ApiGateway.dll"]
