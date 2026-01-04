# ============================================
# Script de Deploy - FiapCloudGames
# Plataforma: Windows
# ============================================

param(
    [string]$ImageName = "jonathanornellas/fiapcloudgames:latest",
    [string]$ContainerName = "fiapgames-app",
    [int]$HostPort = 8080,
    [int]$ContainerPort = 80,
    [string]$DeployPath = "C:\inetpub\fiapGames"
)

Write-Host "╔════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║     FiapCloudGames - Deploy Script (Windows)              ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host ""

Write-Host "📋 Configurações:" -ForegroundColor Yellow
Write-Host "  📦 Imagem: $ImageName"
Write-Host "  🏷️  Container: $ContainerName"
Write-Host "  📁 Caminho: $DeployPath"
Write-Host "  🔌 Porta: $HostPort"
Write-Host ""

# ============================================
# 1. Verificar Docker
# ============================================
Write-Host "🔍 Verificando Docker..." -ForegroundColor Yellow
try {
    $dockerVersion = docker --version
    Write-Host "✅ Docker encontrado: $dockerVersion" -ForegroundColor Green
} catch {
    Write-Host "❌ Docker não está instalado ou não está no PATH" -ForegroundColor Red
    Write-Host "   Instale Docker Desktop para Windows: https://www.docker.com/products/docker-desktop" -ForegroundColor Red
    exit 1
}

# ============================================
# 2. Criar diretório de deploy
# ============================================
Write-Host "`n📁 Criando diretório de deploy..." -ForegroundColor Yellow
if (-not (Test-Path $DeployPath)) {
    New-Item -ItemType Directory -Path $DeployPath -Force | Out-Null
    Write-Host "✅ Diretório criado: $DeployPath" -ForegroundColor Green
} else {
    Write-Host "✅ Diretório já existe: $DeployPath" -ForegroundColor Green
}

# ============================================
# 3. Parar container anterior
# ============================================
Write-Host "`n⏹️  Parando container anterior..." -ForegroundColor Yellow
$existingContainer = docker ps -a -q -f name=$ContainerName 2>$null
if ($existingContainer) {
    Write-Host "  Parando: $ContainerName" -ForegroundColor Cyan
    docker stop $ContainerName -ErrorAction SilentlyContinue | Out-Null
    Write-Host "  Removendo: $ContainerName" -ForegroundColor Cyan
    docker rm $ContainerName -ErrorAction SilentlyContinue | Out-Null
    Write-Host "✅ Container anterior removido" -ForegroundColor Green
} else {
    Write-Host "ℹ️  Nenhum container anterior encontrado" -ForegroundColor Cyan
}

# ============================================
# 4. Fazer pull da imagem
# ============================================
Write-Host "`n📥 Fazendo pull da imagem do Docker Hub..." -ForegroundColor Yellow
Write-Host "  Imagem: $ImageName" -ForegroundColor Cyan
docker pull $ImageName
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Erro ao fazer pull da imagem" -ForegroundColor Red
    exit 1
}
Write-Host "✅ Imagem baixada com sucesso" -ForegroundColor Green

# ============================================
# 5. Iniciar novo container
# ============================================
Write-Host "`n🚀 Iniciando novo container..." -ForegroundColor Yellow
Write-Host "  Nome: $ContainerName" -ForegroundColor Cyan
Write-Host "  Porta: $HostPort`:$ContainerPort" -ForegroundColor Cyan

docker run -d `
    --name $ContainerName `
    -p ${HostPort}:${ContainerPort} `
    -e ASPNETCORE_ENVIRONMENT=Production `
    -e ASPNETCORE_URLS=http://+:${ContainerPort} `
    -v ${DeployPath}:/app/data `
    --restart unless-stopped `
    $ImageName

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Erro ao iniciar container" -ForegroundColor Red
    exit 1
}
Write-Host "✅ Container iniciado com sucesso" -ForegroundColor Green

# ============================================
# 6. Aguardar container ficar pronto
# ============================================
Write-Host "`n⏳ Aguardando container ficar pronto..." -ForegroundColor Yellow
$maxAttempts = 30
$attempt = 0
$containerReady = $false

while ($attempt -lt $maxAttempts) {
    $attempt++
    
    # Verificar se container está rodando
    $isRunning = docker ps -q -f name=$ContainerName
    
    if ($isRunning) {
        Write-Host "✅ Container está rodando (tentativa $attempt/$maxAttempts)" -ForegroundColor Green
        $containerReady = $true
        break
    }
    
    Write-Host "  Tentativa $attempt/$maxAttempts..." -ForegroundColor Cyan
    Start-Sleep -Seconds 2
}

if (-not $containerReady) {
    Write-Host "❌ Container não ficou pronto no tempo limite" -ForegroundColor Red
    Write-Host "  Verifique os logs: docker logs $ContainerName" -ForegroundColor Yellow
    exit 1
}

# ============================================
# 7. Verificar status
# ============================================
Write-Host "`n📊 Status do container:" -ForegroundColor Yellow
docker ps -f name=$ContainerName

# ============================================
# 8. Exibir informações de acesso
# ============================================
Write-Host "`n╔════════════════════════════════════════════════════════════╗" -ForegroundColor Green
Write-Host "║              ✅ DEPLOY CONCLUÍDO COM SUCESSO!              ║" -ForegroundColor Green
Write-Host "╚════════════════════════════════════════════════════════════╝" -ForegroundColor Green
Write-Host ""
Write-Host "🌐 Acesse a aplicação:" -ForegroundColor Green
Write-Host "   URL: http://localhost:$HostPort/swagger" -ForegroundColor Cyan
Write-Host ""
Write-Host "📁 Dados persistidos em:" -ForegroundColor Green
Write-Host "   Caminho: $DeployPath" -ForegroundColor Cyan
Write-Host ""
Write-Host "🔧 Comandos úteis:" -ForegroundColor Green
Write-Host "   Ver logs:        docker logs -f $ContainerName" -ForegroundColor Cyan
Write-Host "   Parar:           docker stop $ContainerName" -ForegroundColor Cyan
Write-Host "   Iniciar:         docker start $ContainerName" -ForegroundColor Cyan
Write-Host "   Remover:         docker rm -f $ContainerName" -ForegroundColor Cyan
Write-Host ""
