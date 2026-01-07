# AWS Lambda - FIAP Cloud Games

## Visão Geral

O projeto utiliza **AWS Lambda** para processamento serverless de:
- **Notificações de Pagamento** - Enviar emails quando um pagamento é processado
- **Recomendações de Jogos** - Gerar recomendações baseadas em Elasticsearch

---

## Funções Lambda

### 1. NotificationFunction

**Objetivo:** Enviar notificações de pagamento por email

**Handler:** `FiapCloudGames.Lambda::FiapCloudGames.Lambda.Functions.NotificationFunction::HandlePaymentNotificationAsync`

**Entrada:**
```json
{
  "PaymentId": "123e4567-e89b-12d3-a456-426614174000",
  "UserId": "user-123",
  "GameId": "game-456",
  "Amount": 299.90,
  "Status": "Completed"
}
```

**Saída:**
```json
{
  "success": true,
  "message": "Payment notification sent successfully",
  "paymentId": "123e4567-e89b-12d3-a456-426614174000",
  "recipientEmail": "jonathan.nnt@hotmail.com",
  "timestamp": "2026-01-06T10:30:00Z"
}
```

**Configuração:**
- **Timeout:** 60 segundos
- **Memory:** 512 MB
- **Runtime:** .NET 8
- **Variáveis de Ambiente:** Nenhuma

**Integração:**
- Acionada por RabbitMQ quando um pagamento é processado
- Envia email via AWS SES
- Email para: `jonathan.nnt@hotmail.com`

---

### 2. RecommendationFunction

**Objetivo:** Gerar recomendações de jogos baseadas em Elasticsearch

**Handler:** `FiapCloudGames.Lambda::FiapCloudGames.Lambda.Functions.RecommendationFunction::GenerateRecommendationsAsync`

**Entrada:**
```json
{
  "userId": "123e4567-e89b-12d3-a456-426614174000",
  "limit": 10
}
```

**Saída:**
```json
{
  "success": true,
  "data": {
    "userId": "123e4567-e89b-12d3-a456-426614174000",
    "recommendedGames": [
      {
        "id": "game-1",
        "title": "Elden Ring",
        "description": "Action RPG",
        "price": 299.90,
        "genre": "RPG",
        "rating": 9.5
      }
    ],
    "popularGames": [...],
    "generatedAt": "2026-01-06T10:30:00Z",
    "totalRecommendations": 10
  },
  "timestamp": "2026-01-06T10:30:00Z"
}
```

**Configuração:**
- **Timeout:** 60 segundos
- **Memory:** 512 MB
- **Runtime:** .NET 8
- **Variáveis de Ambiente:**
  - `ELASTICSEARCH_URL`: URL do Elasticsearch (padrão: http://localhost:9200)

**Integração:**
- Consulta Elasticsearch para obter jogos com melhor rating
- Retorna jogos recomendados e populares
- Pode ser acionada via API Gateway ou EventBridge

---

## Arquitetura

```
Payments API
    ↓
RabbitMQ (payment.completed)
    ↓
Lambda NotificationFunction
    ↓
AWS SES
    ↓
Email (jonathan.nnt@hotmail.com)

Games API
    ↓
Elasticsearch
    ↓
Lambda RecommendationFunction
    ↓
Recomendações
```

---

## Deploy

### Pré-requisitos

1. AWS CLI configurado com credenciais
2. .NET 8 SDK instalado
3. IAM Role criada: `FiapCloudGamesLambdaRole`

### Opção 1: Script Automático

```bash
./deploy-lambda.sh
```

### Opção 2: Manual via AWS CLI

**Compilar:**
```bash
dotnet publish FiapCloudGames.Lambda/FiapCloudGames.Lambda.csproj -c Release -o ./lambda-publish
cd lambda-publish
zip -r ../lambda-deployment.zip .
cd ..
```

**Criar/Atualizar NotificationFunction:**
```bash
aws lambda create-function \
  --function-name FiapCloudGames-NotificationFunction \
  --runtime dotnet8 \
  --role arn:aws:iam::048249931892:role/FiapCloudGamesLambdaRole \
  --handler FiapCloudGames.Lambda::FiapCloudGames.Lambda.Functions.NotificationFunction::HandlePaymentNotificationAsync \
  --zip-file fileb://lambda-deployment.zip \
  --timeout 60 \
  --memory-size 512 \
  --region us-east-1
```

**Criar/Atualizar RecommendationFunction:**
```bash
aws lambda create-function \
  --function-name FiapCloudGames-RecommendationFunction \
  --runtime dotnet8 \
  --role arn:aws:iam::048249931892:role/FiapCloudGamesLambdaRole \
  --handler FiapCloudGames.Lambda::FiapCloudGames.Lambda.Functions.RecommendationFunction::GenerateRecommendationsAsync \
  --zip-file fileb://lambda-deployment.zip \
  --timeout 60 \
  --memory-size 512 \
  --environment Variables={ELASTICSEARCH_URL=http://localhost:9200} \
  --region us-east-1
```

---

## Testes

### Testar NotificationFunction

```bash
aws lambda invoke \
  --function-name FiapCloudGames-NotificationFunction \
  --payload '{"PaymentId":"123","UserId":"user-1","GameId":"game-1","Amount":299.90,"Status":"Completed"}' \
  --region us-east-1 \
  response.json

cat response.json
```

### Testar RecommendationFunction

```bash
aws lambda invoke \
  --function-name FiapCloudGames-RecommendationFunction \
  --payload '{"userId":"123e4567-e89b-12d3-a456-426614174000","limit":10}' \
  --region us-east-1 \
  response.json

cat response.json
```

---

## Monitoramento

### CloudWatch Logs

```bash
aws logs tail /aws/lambda/FiapCloudGames-NotificationFunction --follow
aws logs tail /aws/lambda/FiapCloudGames-RecommendationFunction --follow
```

### Métricas

- **Invocations:** Número de vezes que a função foi acionada
- **Duration:** Tempo de execução
- **Errors:** Número de erros
- **Throttles:** Quando Lambda não consegue processar

---

## Configuração de SES

Para enviar emails via AWS SES:

1. Verificar email no SES:
```bash
aws ses verify-email-identity \
  --email-address jonathan.nnt@hotmail.com \
  --region us-east-1
```

2. Sair do Sandbox (se necessário):
```bash
aws ses set-account-sending-enabled \
  --enabled \
  --region us-east-1
```

---

## Troubleshooting

### Erro: "Unable to locate credentials"
- Configure AWS CLI: `aws configure`
- Ou defina variáveis de ambiente:
  ```bash
  export AWS_ACCESS_KEY_ID=your-key
  export AWS_SECRET_ACCESS_KEY=your-secret
  export AWS_DEFAULT_REGION=us-east-1
  ```

### Erro: "Email address not verified in SES"
- Verifique o email no SES console
- Ou execute: `aws ses verify-email-identity --email-address jonathan.nnt@hotmail.com`

### Erro: "Elasticsearch connection refused"
- Certifique-se que Elasticsearch está rodando
- Verifique a URL em variáveis de ambiente

---

## Próximos Passos

1. Integrar com API Gateway para invocar via HTTP
2. Adicionar DLQ (Dead Letter Queue) para falhas
3. Implementar retry automático com backoff exponencial
4. Adicionar autoscaling baseado em métricas
5. Configurar alertas no CloudWatch

---

## Referências

- [AWS Lambda Documentation](https://docs.aws.amazon.com/lambda/)
- [AWS SES Documentation](https://docs.aws.amazon.com/ses/)
- [Elasticsearch .NET Client](https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/index.html)
