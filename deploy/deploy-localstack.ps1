<#
Deploy da API no LocalStack: publica o artefato (imagem) no S3 e sobe o
container + SQL Server, com smoke test em /health.
Uso: ./deploy/deploy-localstack.ps1 -ImageName "seuusuario/fiapcloudgames" -Tag "latest"
#>

param(
    [string]$ImageName = "fiapcloudgames",
    [string]$Tag       = "latest"
)

# Continue: stderr de aws/docker nao derruba o script (checamos $LASTEXITCODE).
$ErrorActionPreference = "Continue"

$Region        = "us-east-1"
$Bucket        = "fiapcloudgames-artifacts"
$ArtifactFile  = "fiapcloudgames-$Tag.tar"
$LocalStackUrl = "http://localhost:4566"
$Network       = "fiap-net"
$SqlContainer  = "fiap-sqlserver"
$ApiContainer  = "fiap-api"
$SaPassword    = "Your_password123"
$HostPort      = 8080
$ContainerPort = 8080
$Image         = "$ImageName`:$Tag"

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

Write-Step "1/6 Verificando o LocalStack"
try {
    Invoke-RestMethod -Uri "$LocalStackUrl/_localstack/health" -TimeoutSec 10 | Out-Null
    Write-Host "LocalStack OK." -ForegroundColor Green
} catch {
    Write-Error "LocalStack nao respondeu em $LocalStackUrl. Suba com 'docker compose -f deploy/docker-compose.localstack.yml up -d'."
    exit 1
}

Write-Step "2/6 Baixando a imagem $Image"
docker pull $Image
if ($LASTEXITCODE -ne 0) {
    Write-Host "Nao baixou do Docker Hub; tentando imagem local." -ForegroundColor Yellow
}

Write-Step "3/6 Publicando o artefato no S3 (bucket '$Bucket')"
Invoke-Aws s3 mb "s3://$Bucket" | Out-Null
docker save $Image -o $ArtifactFile
if ($LASTEXITCODE -ne 0) { Write-Error "Falha ao exportar a imagem (docker save)."; exit 1 }
Invoke-Aws s3 cp $ArtifactFile "s3://$Bucket/$ArtifactFile"
if ($LASTEXITCODE -ne 0) { Write-Error "Falha ao enviar o artefato para o S3."; exit 1 }
Remove-Item $ArtifactFile -Force -ErrorAction SilentlyContinue
Write-Host "Artefato publicado em s3://$Bucket/$ArtifactFile" -ForegroundColor Green

Write-Step "4/6 Conferindo o artefato no S3"
Invoke-Aws s3 ls "s3://$Bucket/"

Write-Step "5/6 Subindo os containers (SQL Server + API)"
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
    $Image | Out-Null

Write-Step "6/6 Smoke test em http://localhost:$HostPort/health"
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
