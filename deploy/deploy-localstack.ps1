<#
Deploy da API no LocalStack (ECR local) + SQL Server, com smoke test em /health.
Uso: ./deploy/deploy-localstack.ps1 -ImageName "seuusuario/fiapcloudgames" -Tag "latest"
#>

param(
    [string]$ImageName = "fiapcloudgames",
    [string]$Tag       = "latest"
)

# Continue: stderr de aws/docker nao derruba o script (checamos $LASTEXITCODE).
$ErrorActionPreference = "Continue"

$Region          = "us-east-1"
$EcrRepo         = "fiapcloudgames"
$LocalStackUrl   = "http://localhost:4566"
$EcrRegistry     = "localhost:4566"
$EcrImage        = "$EcrRegistry/$EcrRepo`:$Tag"
$Network         = "fiap-net"
$SqlContainer    = "fiap-sqlserver"
$ApiContainer    = "fiap-api"
$SaPassword      = "Your_password123"
$HostPort        = 8080
$ContainerPort   = 8080

# Credenciais fake exigidas pela AWS CLI (LocalStack aceita qualquer valor).
$env:AWS_ACCESS_KEY_ID     = "test"
$env:AWS_SECRET_ACCESS_KEY = "test"
$env:AWS_DEFAULT_REGION    = $Region

function Write-Step($msg) { Write-Host "`n=== $msg ===" -ForegroundColor Cyan }

# awslocal se existir, senao aws --endpoint-url. 2>&1 evita erro fatal no stderr.
function Invoke-Aws {
    param([Parameter(ValueFromRemainingArguments=$true)]$Args)
    if (Get-Command awslocal -ErrorAction SilentlyContinue) {
        & awslocal @Args 2>&1
    } else {
        & aws --endpoint-url $LocalStackUrl @Args 2>&1
    }
}

Write-Step "1/7 Verificando o LocalStack"
try {
    Invoke-RestMethod -Uri "$LocalStackUrl/_localstack/health" -TimeoutSec 10 | Out-Null
    Write-Host "LocalStack OK." -ForegroundColor Green
} catch {
    Write-Error "LocalStack nao respondeu em $LocalStackUrl. Suba com 'docker compose -f deploy/docker-compose.localstack.yml up -d'."
    exit 1
}

Write-Step "2/7 Baixando a imagem $ImageName`:$Tag"
docker pull "$ImageName`:$Tag"
if ($LASTEXITCODE -ne 0) {
    Write-Host "Nao baixou do Docker Hub; tentando imagem local." -ForegroundColor Yellow
}

Write-Step "3/7 Criando repositorio ECR '$EcrRepo'"
Invoke-Aws ecr create-repository --repository-name $EcrRepo | Out-Null
if ($LASTEXITCODE -eq 0) {
    Write-Host "Repositorio ECR criado." -ForegroundColor Green
} else {
    Write-Host "Repositorio ECR ja existe (ok)." -ForegroundColor Yellow
}

Write-Step "4/7 Publicando a imagem no ECR ($EcrImage)"
docker tag "$ImageName`:$Tag" $EcrImage
docker push $EcrImage
if ($LASTEXITCODE -ne 0) { Write-Error "Falha no push para o ECR do LocalStack."; exit 1 }
Write-Host "Imagem publicada no ECR do LocalStack." -ForegroundColor Green

Write-Step "5/7 Baixando a imagem de volta do ECR"
docker pull $EcrImage
Invoke-Aws ecr describe-images --repository-name $EcrRepo

Write-Step "6/7 Subindo os containers (SQL Server + API)"
docker network create $Network 2>$null | Out-Null

docker rm -f $SqlContainer 2>$null | Out-Null
docker run -d --name $SqlContainer --network $Network `
    -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=$SaPassword" `
    -p 1433:1433 `
    mcr.microsoft.com/mssql/server:2022-latest | Out-Null

Write-Host "Aguardando o SQL Server..." -ForegroundColor Yellow
$ready = $false
for ($i = 1; $i -le 30; $i++) {
    docker exec $SqlContainer /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P $SaPassword -C -Q "SELECT 1" 2>$null | Out-Null
    if ($LASTEXITCODE -eq 0) { $ready = $true; break }
    Start-Sleep -Seconds 3
}
if (-not $ready) { Write-Error "SQL Server nao ficou pronto a tempo."; exit 1 }
Write-Host "SQL Server pronto." -ForegroundColor Green

docker rm -f $ApiContainer 2>$null | Out-Null
$connStr = "Server=$SqlContainer,1433;Database=FiapGameDb;User Id=sa;Password=$SaPassword;TrustServerCertificate=True;MultipleActiveResultSets=true"
docker run -d --name $ApiContainer --network $Network `
    -p "$HostPort`:$ContainerPort" `
    -e "ASPNETCORE_URLS=http://+:$ContainerPort" `
    -e "ConnectionStrings__Default=$connStr" `
    --health-cmd "curl -f http://localhost:$ContainerPort/health || exit 1" `
    --health-interval=10s --health-retries=5 --health-start-period=20s `
    $EcrImage | Out-Null

Write-Step "7/7 Smoke test em http://localhost:$HostPort/health"
$ok = $false
for ($i = 1; $i -le 20; $i++) {
    Start-Sleep -Seconds 3
    try {
        $resp = Invoke-RestMethod -Uri "http://localhost:$HostPort/health" -TimeoutSec 5
        if ($resp.status -eq "Healthy") { $ok = $true; break }
    } catch { }
}

if ($ok) {
    Write-Host "`nDEPLOY OK! API em http://localhost:$HostPort" -ForegroundColor Green
    Write-Host "Swagger: http://localhost:$HostPort/swagger" -ForegroundColor Green
    Write-Host "Health:  http://localhost:$HostPort/health" -ForegroundColor Green
    exit 0
} else {
    Write-Host "`nA API nao respondeu no /health. Logs:" -ForegroundColor Red
    docker logs --tail 40 $ApiContainer
    exit 1
}
