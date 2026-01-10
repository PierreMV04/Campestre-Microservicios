# Script para inicializar y subir a GitHub
# Ejecutar desde PowerShell: .\init-github.ps1

param(
    [Parameter(Mandatory=$true)]
    [string]$RepoUrl
)

Write-Host "==================================" -ForegroundColor Cyan
Write-Host "  GITHUB INITIALIZATION" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan
Write-Host ""

# Verificar si Git está instalado
Write-Host "?? Verificando Git..." -ForegroundColor Yellow
$gitVersion = git --version
if ($LASTEXITCODE -ne 0) {
    Write-Host "? Git no encontrado. Instala Git desde:" -ForegroundColor Red
    Write-Host "   https://git-scm.com/download/win" -ForegroundColor Red
    exit 1
}
Write-Host "? $gitVersion" -ForegroundColor Green
Write-Host ""

# Verificar si ya hay un repositorio Git
if (Test-Path ".git") {
    Write-Host "??  Ya existe un repositorio Git" -ForegroundColor Yellow
    $response = Read-Host "¿Deseas reinicializarlo? (s/n)"
    if ($response -eq "s") {
        Remove-Item -Recurse -Force .git
        Write-Host "? Repositorio anterior eliminado" -ForegroundColor Green
    } else {
        Write-Host "? Operación cancelada" -ForegroundColor Red
        exit 0
    }
}

# Inicializar Git
Write-Host "?? Inicializando Git..." -ForegroundColor Yellow
git init
git branch -M main
Write-Host "? Git inicializado" -ForegroundColor Green
Write-Host ""

# Agregar archivos
Write-Host "?? Agregando archivos..." -ForegroundColor Yellow
git add .
Write-Host "? Archivos agregados" -ForegroundColor Green
Write-Host ""

# Commit inicial
Write-Host "?? Creando commit inicial..." -ForegroundColor Yellow
git commit -m "Initial commit - Hotel Microservices Architecture"
if ($LASTEXITCODE -ne 0) {
    Write-Host "? Error al crear commit" -ForegroundColor Red
    exit 1
}
Write-Host "? Commit creado" -ForegroundColor Green
Write-Host ""

# Agregar remote
Write-Host "?? Conectando con GitHub..." -ForegroundColor Yellow
git remote add origin $RepoUrl
Write-Host "? Remote agregado: $RepoUrl" -ForegroundColor Green
Write-Host ""

# Push a GitHub
Write-Host "?? Subiendo a GitHub..." -ForegroundColor Yellow
git push -u origin main
if ($LASTEXITCODE -ne 0) {
    Write-Host "? Error al subir a GitHub" -ForegroundColor Red
    Write-Host ""
    Write-Host "?? Asegúrate de que:" -ForegroundColor Yellow
    Write-Host "   1. Has creado el repositorio en GitHub" -ForegroundColor White
    Write-Host "   2. La URL es correcta" -ForegroundColor White
    Write-Host "   3. Tienes permisos de escritura" -ForegroundColor White
    Write-Host "   4. Has configurado Git con tu usuario:" -ForegroundColor White
    Write-Host "      git config --global user.name 'Tu Nombre'" -ForegroundColor Cyan
    Write-Host "      git config --global user.email 'tu@email.com'" -ForegroundColor Cyan
    exit 1
}
Write-Host "? Código subido a GitHub" -ForegroundColor Green
Write-Host ""

Write-Host "==================================" -ForegroundColor Green
Write-Host "  ? GITHUB SETUP COMPLETADO" -ForegroundColor Green
Write-Host "==================================" -ForegroundColor Green
Write-Host ""

Write-Host "?? Próximos pasos:" -ForegroundColor Yellow
Write-Host "   1. Ve a https://railway.app" -ForegroundColor White
Write-Host "   2. New Project ? Deploy from GitHub repo" -ForegroundColor White
Write-Host "   3. Selecciona tu repositorio" -ForegroundColor White
Write-Host "   4. Lee README_DEPLOYMENT.md para más detalles" -ForegroundColor White
Write-Host ""

Write-Host "?? URL del repositorio:" -ForegroundColor Yellow
Write-Host "   $RepoUrl" -ForegroundColor Cyan
Write-Host ""
