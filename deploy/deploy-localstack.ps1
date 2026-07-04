<#
.SYNOPSIS
    Faz o "deploy" da API Fiap Cloud Games no LocalStack (ECR local) e sobe o
    container junto com um SQL Server, fazendo um smoke test no /health.

.DESCRIPTION
    Passos executados:
      1. Verifica se o LocalStack está no ar (http://localhost:4566).
      2. Baixa a imagem do Docker Hub (ou usa uma local).
      3. Cria um repositório ECR dentro do LocalStack.
      4. Faz push da imagem para o ECR do LocalStack.
      5. Baixa a imagem de volta do ECR (prova que está publicada).
      6. Sobe um SQL Server e a API em containers, na mesma rede Docker.
      7. Faz um smoke test no endpoint /health.

.EXAMPLE
    ./deploy/deploy-localstack.ps1 -ImageName "seuusuario/fiapcloudgames" -Tag "latest"
#>

param(
    [string]$ImageName = "fiapcloudgames",
    [string]$Tag       = "latest"
)

$ErrorActionPreference = "Stop"

# ----------------------------------------------------------------------------
# Configurações
# ----------------------------------------------------------------------------
$Region          = "us-east-1"
$EcrRepo         = "fiapcloudgames"
$LocalStackUrl   = "http://localhost:4566"
$EcrRegistry     = "localhost:4566"          # registry ECR do LocalStack
$EcrImage        = "$EcrRegistry/$EcrRepo`:$Tag"
$Network         = "fiap-net"
$SqlContainer    = "fiap-sqlserver"
$ApiContainer    = "fiap-api"
$SaPassword      = "Your_password123"
$HostPort        = 8080
$ContainerPort   = 8080

# Credenciais "fake" exigidas pela AWS CLI (o LocalStack aceita qualquer valor)
$env:AWS_ACCESS_KEY_ID     = "test"
$env:AWS_SECRET_ACCESS_KEY = "test"
$env:AWS_DEFAULT_REGION    = $Region

function Write-Step($msg) { Write-Host "`n=== $msg ===" -ForegroundColor Cyan }

# Usa 'awslocal' se existir, senão cai para 'aws --endpoint-url'
function Invoke-Aws {
    param([Parameter(ValueFromRemainingArguments=$true)]$Args)
    if (Get-Command awslocal -ErrorAction SilentlyContinue) {
        & awslocal @Args
    } else {
        & aws --endpoint-url $LocalStackUrl @Args
    }
}

# ----------------------------------------------------------------------------
# 1) LocalStack está rodando?
# ----------------------------------------------------------------------------
Write-Step "1/7 Verificando o LocalStack"
try {
    $health = Invoke-RestMethod -Uri "$LocalStackUrl/_localstack/health" -TimeoutSec 10
    Write-Host "LocalStack OK." -ForegroundColor Green
} catch {
    Write-Error "LocalStack nao respondeu em $LocalStackUrl. Inicie com 'localstack start -d' ou 'docker compose -f deploy/docker-compose.localstack.yml up -d' e tente novamente."
    exit 1
}

# ----------------------------------------------------------------------------
# 2) Baixar a imagem (Docker Hub) -- se falhar, tenta usar imagem local
# ----------------------------------------------------------------------------
Write-Step "2/7 Baixando a imagem $ImageName`:$Tag"
docker pull "$ImageName`:$Tag"
if ($LASTEXITCODE -ne 0) {
    Write-Host "Nao consegui baixar do Docker Hub. Vou tentar usar uma imagem local com esse nome." -ForegroundColor Yellow
}

# ----------------------------------------------------------------------------
# 3) Criar repositório ECR no LocalStack
# ----------------------------------------------------------------------------
Write-Step "3/7 Criando repositorio ECR '$EcrRepo' no LocalStack"
Invoke-Aws ecr create-repository --repository-name $EcrRepo 2>$null
if ($LASTEXITCODE -eq 0) {
    Write-Host "Repositorio ECR criado." -ForegroundColor Green
} else {
    Write-Host "Repositorio ECR ja existe (ok)." -ForegroundColor Yellow
}

# ----------------------------------------------------------------------------
# 4) Tag + push da imagem para o ECR do LocalStack
# ----------------------------------------------------------------------------
Write-Step "4/7 Publicando a imagem no ECR do LocalStack ($EcrImage)"
docker tag "$ImageName`:$Tag" $EcrImage
docker push $EcrImage
if ($LASTEXITCODE -ne 0) { Write-Error "Falha no push para o ECR do LocalStack."; exit 1 }
Write-Host "Imagem publicada no ECR do LocalStack." -ForegroundColor Green

# ----------------------------------------------------------------------------
# 5) Pull de volta do ECR (prova que o artefato esta publicado)
# ----------------------------------------------------------------------------
Write-Step "5/7 Baixando a imagem de volta do ECR do LocalStack"
docker pull $EcrImage
Invoke-Aws ecr describe-images --repository-name $EcrRepo

# ----------------------------------------------------------------------------
# 6) Subir SQL Server + API
# ----------------------------------------------------------------------------
Write-Step "6/7 Subindo os containers (SQL Server + API)"

docker network create $Network 2>$null | Out-Null

# (Re)cria o SQL Server
docker rm -f $SqlContainer 2>$null | Out-Null
docker run -d --name $SqlContainer --network $Network `
    -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=$SaPassword" `
    -p 1433:1433 `
    mcr.microsoft.com/mssql/server:2022-latest | Out-Null

Write-Host "Aguardando o SQL Server ficar pronto..." -ForegroundColor Yellow
$ready = $false
for ($i = 1; $i -le 30; $i++) {
    docker exec $SqlContainer /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P $SaPassword -C -Q "SELECT 1" 2>$null | Out-Null
    if ($LASTEXITCODE -eq 0) { $ready = $true; break }
    Start-Sleep -Seconds 3
}
if (-not $ready) { Write-Error "SQL Server nao ficou pronto a tempo."; exit 1 }
Write-Host "SQL Server pronto." -ForegroundColor Green

# (Re)cria a API a partir da imagem do ECR do LocalStack
docker rm -f $ApiContainer 2>$null | Out-Null
$connStr = "Server=$SqlContainer,1433;Database=FiapGameDb;User Id=sa;Password=$SaPassword;TrustServerCertificate=True;MultipleActiveResultSets=true"
docker run -d --name $ApiContainer --network $Network `
    -p "$HostPort`:$ContainerPort" `
    -e "ASPNETCORE_URLS=http://+:$ContainerPort" `
    -e "ConnectionStrings__Default=$connStr" `
    --health-cmd "curl -f http://localhost:$ContainerPort/health || exit 1" `
    --health-interval=10s --health-retries=5 --health-start-period=20s `
    $EcrImage | Out-Null

# ----------------------------------------------------------------------------
# 7) Smoke test no /health
# ----------------------------------------------------------------------------
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
    Write-Host "`nDEPLOY OK! API respondendo em http://localhost:$HostPort" -ForegroundColor Green
    Write-Host "Swagger:  http://localhost:$HostPort/swagger" -ForegroundColor Green
    Write-Host "Health:   http://localhost:$HostPort/health" -ForegroundColor Green
    exit 0
} else {
    Write-Host "`nA API nao respondeu no /health. Veja os logs:" -ForegroundColor Red
    docker logs --tail 40 $ApiContainer
    exit 1
}
