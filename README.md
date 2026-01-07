# FIAP Cloud Games

**Projeto de Estudo - Tarefa 3 FIAP**

Sistema de gerenciamento de jogos desenvolvido em .NET 8 com arquitetura de microsserviços, Elasticsearch, Event Sourcing e RabbitMQ.

> ⚠️ Este é um projeto educacional desenvolvido como parte da Tarefa 3 do Tech Challenge FIAP. Não deve ser utilizado em produção sem as devidas adaptações de segurança.

## 🚀 Execução Rápida

### Docker Compose (Recomendado)
```bash
docker-compose -f docker-compose.microservices.yml up -d
```

Acesse:
- **Users API:** http://localhost:5001/swagger
- **Games API:** http://localhost:5002/swagger
- **Payments API:** http://localhost:5003/swagger
- **RabbitMQ Management:** http://localhost:15672 (guest/guest)
- **Elasticsearch:** http://localhost:9200
- **Grafana:** http://localhost:3000 (admin/admin)

## 📋 Pré-requisitos

- Docker e Docker Compose
- .NET 8 SDK (para desenvolvimento local)
- Git
- SQL Server 2022 (local ou Docker)

## 🏗️ Arquitetura

### Microsserviços (3)

| Serviço | Porta | Responsabilidade |
|---------|-------|------------------|
| **Users API** | 5001 | Autenticação, cadastro, perfis |
| **Games API** | 5002 | Listagem, busca, recomendações |
| **Payments API** | 5003 | Processamento de pagamentos |

### Componentes Principais

**Elasticsearch (9200)**
- Indexação de jogos
- Busca avançada
- Recomendações baseadas em rating

**RabbitMQ (5672)**
- Fila de mensagens
- Comunicação assíncrona entre microsserviços
- Retry automático com backoff exponencial
- Dead Letter Queue para mensagens com falha

**Event Sourcing**
- Auditoria completa de eventos
- Rastreamento de transações distribuídas
- Recuperação de estado

**AWS Lambda**
- Processamento assíncrono
- Notificações de pagamento
- Geração de recomendações

### Infraestrutura

**SQL Server (1433)**
- 3 bancos de dados separados (Users, Games, Payments)
- Event Store para auditoria
- Conexão: `Trusted_Connection=true;MultipleActiveResultSets=true`

**Prometheus + Grafana**
- Monitoramento de métricas
- Alertas automáticos

## 📊 Endpoints

### Users API (5001)
```bash
POST   /api/auth/register      # Registrar usuário
POST   /api/auth/login         # Fazer login
```

### Games API (5002)
```bash
POST   /api/games              # Criar jogo
GET    /api/games              # Listar todos
GET    /api/games/{id}         # Obter jogo
GET    /api/games/search       # Buscar (Elasticsearch)
GET    /api/games/recommendations/{userId}  # Recomendações
PUT    /api/games/{id}         # Atualizar
DELETE /api/games/{id}         # Deletar
```

### Payments API (5003)
```bash
POST   /api/payments           # Processar pagamento
GET    /api/payments/{id}      # Obter pagamento
GET    /api/payments/user/{userId}  # Pagamentos do usuário
PUT    /api/payments/{id}/status    # Atualizar status
```

## 🔄 Fluxo de Pagamento (Com RabbitMQ)

```
1. Cliente → Payments API: POST /api/payments
2. Payments API → SQL Server: Cria pagamento (status: Pending)
3. Payments API → Event Store: Registra PaymentProcessedEvent
4. Payments API → RabbitMQ: Publica evento
5. Games API (Consumer) ← RabbitMQ: Consome evento
6. Games API → SQL Server: Adiciona jogo à biblioteca do usuário
7. Games API → RabbitMQ: Publica GameAddedToLibraryEvent
8. Payments API (Consumer) ← RabbitMQ: Confirma sucesso
9. Payments API → SQL Server: Atualiza status para Completed
```

## 🛡️ Resiliência

**Retry Automático**
- 3 tentativas com backoff exponencial
- Intervalo inicial: 1 segundo
- Multiplicador: 2x a cada tentativa

**Dead Letter Queue**
- Mensagens que falham após 3 tentativas
- Monitoramento manual via RabbitMQ Management

**Transações Distribuídas**
- Event Sourcing garante auditoria
- Idempotência em operações críticas

## 🧪 Testando Localmente

### 1. Iniciar containers
```bash
docker-compose -f docker-compose.microservices.yml up -d
```

### 2. Registrar usuário
```bash
curl -X POST http://localhost:5001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "teste@example.com",
    "password": "Senha123!",
    "fullName": "Teste User"
  }'
```

### 3. Fazer login
```bash
curl -X POST http://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "teste@example.com",
    "password": "Senha123!"
  }'
```

### 4. Criar jogo
```bash
curl -X POST http://localhost:5002/api/games \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer SEU_TOKEN" \
  -d '{
    "title": "Elden Ring",
    "description": "Action RPG",
    "genre": "RPG",
    "price": 299.90,
    "rating": 9.5
  }'
```

### 5. Processar pagamento
```bash
curl -X POST http://localhost:5003/api/payments \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "user-id",
    "gameId": "game-id",
    "amount": 299.90,
    "paymentMethod": "credit_card"
  }'
```

## 📦 Estrutura do Projeto

```
FiapCloudGames/
├── FiapCloudGames.Users.Api/          # Microsserviço de Usuários
├── FiapCloudGames.Games.Api/          # Microsserviço de Jogos
├── FiapCloudGames.Payments.Api/       # Microsserviço de Pagamentos
├── FiapCloudGames.Shared/             # Código compartilhado
├── FiapCloudGames.EventSourcing/      # Event Store
├── FiapCloudGames.Lambda/             # Funções serverless
├── FiapCloudGames.Microservices.sln   # Solução principal
├── docker-compose.microservices.yml   # Orquestração
├── azure-pipelines.yml                # CI/CD
├── README.md                          # Este arquivo
├── CHECKUP.md                         # Validação de requisitos
└── LAMBDA.md                          # Documentação Lambda
```

## 🛠️ Tecnologias

| Tecnologia | Versão | Uso |
|------------|--------|-----|
| .NET | 8.0 | Framework principal |
| Entity Framework Core | 9.0 | ORM |
| SQL Server | 2022 | Banco de dados |
| Elasticsearch | 8.10 | Busca e recomendações |
| RabbitMQ | 3.13 | Mensageria |
| JWT | - | Autenticação |
| Docker | - | Containerização |
| Prometheus | - | Monitoramento |
| Grafana | - | Visualização |
| AWS Lambda | - | Serverless |
| AWS SES | - | Email |

## 🚀 CI/CD Pipeline

**Stages:**
1. **Build** - Compila FiapCloudGames.Microservices.sln
2. **Docker** - Build e push de 3 imagens (Users, Games, Payments)
3. **Development** - Deploy em ambiente de desenvolvimento
4. **Staging** - Deploy em EC2 via AWS SSM
5. **Production** - Deploy em produção

**Imagens Docker:**
- `jonathanornellas/fiapcloudgames-users:latest`
- `jonathanornellas/fiapcloudgames-games:latest`
- `jonathanornellas/fiapcloudgames-payments:latest`

## 📊 Monitoramento

**Prometheus (9090)**
- Métricas de requisições HTTP
- Latência de banco de dados
- Taxa de erro

**Grafana (3000)**
- Dashboards customizados
- Alertas em tempo real

**RabbitMQ Management (15672)**
- Monitoramento de filas
- Análise de mensagens
- Configuração de exchanges

## 🔐 Segurança

- JWT Bearer Token para autenticação
- Senhas com hash BCrypt
- Validação de entrada com FluentValidation
- CORS configurado
- HTTPS em produção
- Secret Key configurável via appsettings

## 📝 Variáveis de Ambiente

```bash
# Banco de dados (local com Trusted Connection)
ConnectionStrings__DefaultConnection=Server=localhost;Database=FiapGameUsers;Trusted_Connection=true;MultipleActiveResultSets=true;Encrypt=false;

# JWT Secret Key
Jwt__Key=fiap-cloud-games-secret-key-2024-production-secure-key-minimum-32-chars
Jwt__Issuer=fiap-cloud-games
Jwt__Audience=fiap-cloud-games-users

# RabbitMQ
RabbitMq__Host=localhost
RabbitMq__Username=guest
RabbitMq__Password=guest

# Elasticsearch
Elasticsearch__Url=http://localhost:9200

# Email (Lambda)
Email__SenderEmail=noreply@fiapcloudgames.com
Email__RecipientEmail=jonathan.nnt@hotmail.com
Email__AwsRegion=us-east-1
```

## 🤝 Contribuindo

1. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
2. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
3. Push para a branch (`git push origin feature/AmazingFeature`)
4. Abra um Pull Request

## 📄 Licença

Este projeto é parte da FIAP - Pós-graduação em Arquitetura de Software.

---

**Desenvolvido por:** Jonathan Ornellas  
**FIAP - Pós-graduação em Arquitetura de Software**  
**Janeiro 2026**
