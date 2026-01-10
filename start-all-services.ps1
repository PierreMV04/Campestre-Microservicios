# Script para iniciar todos los microservicios
# Ejecutar desde la carpeta Microservicios

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Hotel Microservices - Start Script   " -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$basePath = $PSScriptRoot

# Función para iniciar servicio en nueva ventana
function Start-Service {
    param (
        [string]$ServiceName,
        [string]$ServicePath,
        [string]$Port
    )
    
    Write-Host "Iniciando $ServiceName en puerto $Port..." -ForegroundColor Yellow
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$ServicePath'; Write-Host 'Iniciando $ServiceName...' -ForegroundColor Green; dotnet run"
    Start-Sleep -Seconds 2
}

# Construir solución primero
Write-Host "Compilando solución..." -ForegroundColor Magenta
dotnet build "$basePath\HotelMicroservices.sln" --configuration Debug

if ($LASTEXITCODE -ne 0) {
    Write-Host "Error al compilar. Abortando." -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "Iniciando servicios..." -ForegroundColor Green
Write-Host ""

# Iniciar servicios
Start-Service -ServiceName "CatalogosService (REST)" -ServicePath "$basePath\CatalogosService" -Port "5001"
Start-Service -ServiceName "HabitacionesService (GraphQL)" -ServicePath "$basePath\HabitacionesService" -Port "5002"
Start-Service -ServiceName "ReservasService (gRPC)" -ServicePath "$basePath\ReservasService" -Port "5003"
Start-Service -ServiceName "UsuariosPagosService (REST)" -ServicePath "$basePath\UsuariosPagosService" -Port "5004"

# Esperar un poco más antes del Gateway
Start-Sleep -Seconds 3
Start-Service -ServiceName "ApiGateway" -ServicePath "$basePath\ApiGateway" -Port "5000"

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Todos los servicios iniciados!       " -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "URLs disponibles:" -ForegroundColor White
Write-Host "  - API Gateway:      http://localhost:5000/swagger" -ForegroundColor Green
Write-Host "  - Catálogos:        http://localhost:5001/swagger" -ForegroundColor Green
Write-Host "  - Habitaciones:     http://localhost:5002/graphql-ui" -ForegroundColor Green
Write-Host "  - Reservas (gRPC):  http://localhost:5003" -ForegroundColor Green
Write-Host "  - Usuarios/Pagos:   http://localhost:5004/swagger" -ForegroundColor Green
Write-Host ""
Write-Host "Para obtener un token JWT:" -ForegroundColor Yellow
Write-Host '  curl -X POST http://localhost:5000/api/auth/token -H "Content-Type: application/json" -d "{\"username\":\"admin\",\"password\":\"admin123\"}"' -ForegroundColor Gray
Write-Host ""
