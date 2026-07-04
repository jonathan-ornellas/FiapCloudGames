#!/usr/bin/env bash
# Deploy da API no LocalStack: publica o artefato (imagem) no S3 e sobe o
# container + SQL Server, com smoke test em /health.
# Uso: ./deploy/deploy-localstack.sh <imagem> <tag>
set -euo pipefail

IMAGE_NAME="${1:-fiapcloudgames}"
TAG="${2:-latest}"
IMAGE="${IMAGE_NAME}:${TAG}"

REGION="us-east-1"
BUCKET="fiapcloudgames-artifacts"
ARTIFACT_FILE="fiapcloudgames-${TAG}.tar"
LOCALSTACK_URL="http://localhost:4566"
NETWORK="fiap-net"
SQL_CONTAINER="fiap-sqlserver"
API_CONTAINER="fiap-api"
SA_PASSWORD="Your_password123"
HOST_PORT=8080
CONTAINER_PORT=8080

# Credenciais fake exigidas pela AWS CLI (LocalStack aceita qualquer valor).
export AWS_ACCESS_KEY_ID=test
export AWS_SECRET_ACCESS_KEY=test
export AWS_DEFAULT_REGION="$REGION"

step() { echo -e "\n=== $1 ==="; }

aws_cmd() {
  if command -v awslocal >/dev/null 2>&1; then awslocal "$@";
  else aws --endpoint-url "$LOCALSTACK_URL" "$@"; fi
}

step "1/6 Verificando o LocalStack"
if ! curl -fs "${LOCALSTACK_URL}/_localstack/health" >/dev/null; then
  echo "LocalStack nao respondeu em ${LOCALSTACK_URL}. Suba com 'docker compose -f deploy/docker-compose.localstack.yml up -d'."; exit 1
fi
echo "LocalStack OK."

step "2/6 Baixando a imagem ${IMAGE}"
docker pull "$IMAGE" || echo "Nao baixou do Docker Hub; tentando imagem local."

step "3/6 Publicando o artefato no S3 (bucket '${BUCKET}')"
aws_cmd s3 mb "s3://${BUCKET}" 2>/dev/null || true
docker save "$IMAGE" -o "$ARTIFACT_FILE"
aws_cmd s3 cp "$ARTIFACT_FILE" "s3://${BUCKET}/${ARTIFACT_FILE}"
rm -f "$ARTIFACT_FILE"
echo "Artefato publicado em s3://${BUCKET}/${ARTIFACT_FILE}"

step "4/6 Conferindo o artefato no S3"
aws_cmd s3 ls "s3://${BUCKET}/"

step "5/6 Subindo os containers (SQL Server + API)"
docker network create "$NETWORK" 2>/dev/null || true

docker rm -f "$SQL_CONTAINER" 2>/dev/null || true
docker run -d --name "$SQL_CONTAINER" --network "$NETWORK" \
  -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=${SA_PASSWORD}" \
  -p 1433:1433 \
  mcr.microsoft.com/mssql/server:2022-latest >/dev/null

echo "Aguardando o SQL Server..."
ready=false
for i in $(seq 1 30); do
  if docker exec "$SQL_CONTAINER" /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -C -Q "SELECT 1" >/dev/null 2>&1; then
    ready=true; break
  fi
  sleep 3
done
[ "$ready" = true ] || { echo "SQL Server nao ficou pronto."; exit 1; }
echo "SQL Server pronto."

docker rm -f "$API_CONTAINER" 2>/dev/null || true
CONN="Server=${SQL_CONTAINER},1433;Database=FiapGameDb;User Id=sa;Password=${SA_PASSWORD};TrustServerCertificate=True;MultipleActiveResultSets=true"
docker run -d --name "$API_CONTAINER" --network "$NETWORK" \
  -p "${HOST_PORT}:${CONTAINER_PORT}" \
  -e "ASPNETCORE_URLS=http://+:${CONTAINER_PORT}" \
  -e "ConnectionStrings__Default=${CONN}" \
  --health-cmd "curl -f http://localhost:${CONTAINER_PORT}/health || exit 1" \
  --health-interval=10s --health-retries=5 --health-start-period=20s \
  "$IMAGE" >/dev/null

step "6/6 Smoke test em http://localhost:${HOST_PORT}/health"
ok=false
for i in $(seq 1 20); do
  sleep 3
  if curl -fs "http://localhost:${HOST_PORT}/health" | grep -q "Healthy"; then ok=true; break; fi
done

if [ "$ok" = true ]; then
  echo -e "\nDEPLOY OK! API em http://localhost:${HOST_PORT}"
  echo "Swagger: http://localhost:${HOST_PORT}/swagger"
else
  echo -e "\nA API nao respondeu. Logs:"; docker logs --tail 40 "$API_CONTAINER"; exit 1
fi
