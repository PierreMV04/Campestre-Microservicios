# Script de despliegue local para desarrollo
# Ejecutar desde PowerShell: .\deploy-local.ps1

Write-Host "==================================" -ForegroundColor Cyan
Write-Host "  HOTEL MICROSERVICES - DEPLOY" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan
Write-Host ""

# Verificar si .NET 8 SDK está instalado
Write-Host "?? Verificando .NET 8 SDK..." -ForegroundColor Yellow
$dotnetVersion = dotnet --version
if ($LASTEXITCODE -ne 0) {
    Write-Host "? .NET SDK no encontrado. Instala .NET 8 SDK desde:" -ForegroundColor Red
    Write-Host "   https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor Red
    exit 1
}
Write-Host "? .NET SDK $dotnetVersion encontrado" -ForegroundColor Green
Write-Host ""

# Limpiar builds anteriores
Write-Host "?? Limpiando builds anteriores..." -ForegroundColor Yellow
Get-ChildItem -Path . -Include bin,obj -Recurse -Directory | Remove-Item -Recurse -Force
Write-Host "? Limpieza completa" -ForegroundColor Green
Write-Host ""

# Restaurar dependencias
Write-Host "?? Restaurando dependencias..." -ForegroundColor Yellow
dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "? Error al restaurar dependencias" -ForegroundColor Red
    exit 1
}
Write-Host "? Dependencias restauradas" -ForegroundColor Green
Write-Host ""

# Compilar solución
Write-Host "?? Compilando solución..." -ForegroundColor Yellow
dotnet build --configuration Release --no-restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "? Error en la compilación" -ForegroundColor Red
    exit 1
}
Write-Host "? Compilación exitosa" -ForegroundColor Green
Write-Host ""

# Publicar cada servicio
$services = @(
    "ApiGateway",
    "CatalogosService",
    "HabitacionesService",
    "ReservasService",
    "UsuariosPagosService"
)

Write-Host "?? Publicando servicios..." -ForegroundColor Yellow
foreach ($service in $services) {
    Write-Host "  ? Publicando $service..." -ForegroundColor Cyan
    dotnet publish "$service/$service.csproj" `
        --configuration Release `
        --output "publish/$service" `
        --no-build
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "  ? Error al publicar $service" -ForegroundColor Red
        exit 1
    }
    Write-Host "  ? $service publicado" -ForegroundColor Green
}
Write-Host ""

Write-Host "==================================" -ForegroundColor Green
Write-Host "  ? DEPLOY LOCAL COMPLETADO" -ForegroundColor Green
Write-Host "==================================" -ForegroundColor Green
Write-Host ""

Write-Host "?? Los archivos publicados están en:" -ForegroundColor Yellow
Write-Host "   .\publish\" -ForegroundColor White
Write-Host ""

Write-Host "?? Para ejecutar los servicios:" -ForegroundColor Yellow
Write-Host "   cd publish\ApiGateway" -ForegroundColor White
Write-Host "   dotnet ApiGateway.dll" -ForegroundColor White
Write-Host ""

Write-Host "?? Para usar Docker:" -ForegroundColor Yellow
Write-Host "   docker-compose up --build" -ForegroundColor White
Write-Host ""

Write-Host "??  Para desplegar en Railway:" -ForegroundColor Yellow
Write-Host "   1. Sube el código a GitHub" -ForegroundColor White
Write-Host "   2. Conecta Railway con tu repositorio" -ForegroundColor White
Write-Host "   3. Lee README_DEPLOYMENT.md para más detalles" -ForegroundColor White
Write-Host ""
