# FiapCloudGames - Tarefa 3: Arquitetura de Microsserviços

> **Projeto de Estudo** - FIAP Tech Challenge - Tarefa 3

Sistema de gerenciamento de jogos desenvolvido em .NET 8 com arquitetura de microsserviços, Elasticsearch, Event Sourcing e RabbitMQ.

## 🚀 Execução Rápida

### Docker Compose (Recomendado)
```bash
docker-compose -f docker-compose.microservices.yml up -d
```

Acesse:
- **Users API:** http://localhost:5001/swagger
- **Games API:** http://localhost:5002/swagger
- **Payments API:** http://localhost:5003/swagger
- **RabbitMQ:** http://localhost:15672 (guest/guest)
- **Elasticsearch:** http://localhost:9200
- **Grafana:** http://localhost:3000 (admin/admin)
- **Prometheus:** http://localhost:9090

### Multi-Projeto no Visual Studio

1. Abra `FiapCloudGames.Microservices.sln`
2. Clique com botão direito na solução
3. Selecione **"Set Startup Projects"**
4. Escolha **"Multiple startup projects"**
5. Marque os 3 microsserviços
6. Clique em **"Start"**

## 📋 Pré-requisitos

- .NET 8 SDK
- Docker e Docker Compose
- Visual Studio 2022 ou VS Code
- Git

## 🏗️ Arquitetura

### Microsserviços (3)

| Serviço | Porta | Responsabilidade |
|---------|-------|------------------|
| **Users API** | 5001 | Autenticação, cadastro, perfis |
| **Games API** | 5002 | Listagem, busca, recomendações |
| **Payments API** | 5003 | Processamento de pagamentos |

### Componentes de Infraestrutura

| Componente | Porta | Descrição |
|-----------|-------|-----------|
| **SQL Server** | 1433 | 3 bancos de dados separados |
| **Elasticsearch** | 9200 | Busca e indexação de jogos |
| **RabbitMQ** | 5672 | Fila de mensagens assíncrona |
| **Prometheus** | 9090 | Coleta de métricas |
| **Grafana** | 3000 | Visualização de métricas |
| **AlertManager** | 9093 | Gerenciamento de alertas |

## 📊 Endpoints Principais

### Users API (5001)
```
POST   /api/auth/register      - Registrar novo usuário
POST   /api/auth/login         - Fazer login
GET    /api/auth/profile       - Obter perfil do usuário
```

### Games API (5002)
```
GET    /api/games              - Listar todos os jogos
POST   /api/games              - Criar novo jogo
GET    /api/games/{id}         - Obter jogo por ID
GET    /api/games/search       - Buscar jogos (Elasticsearch)
GET    /api/games/recommendations/{userId} - Recomendações
PUT    /api/games/{id}         - Atualizar jogo
DELETE /api/games/{id}         - Deletar jogo
```

### Payments API (5003)
```
POST   /api/payments           - Processar pagamento
GET    /api/payments/{id}      - Obter pagamento
GET    /api/payments/user/{userId} - Pagamentos do usuário
PUT    /api/payments/{id}/status - Atualizar status
```

## 🔄 Fluxo de Dados

### Pagamento → Recomendação (Com RabbitMQ)

```
1. Usuário faz pagamento
   ↓
2. Payments API processa
   ↓
3. Publica PaymentProcessedEvent no RabbitMQ
   ↓
4. Games API consome evento
   ↓
5. Adiciona jogo à biblioteca do usuário
   ↓
6. Lambda envia notificação por email
```

## 🛡️ Resiliência

**Retry Automático**
- 3 tentativas com backoff exponencial
- Intervalo inicial: 1 segundo
- Multiplicador: 2x a cada tentativa

**Dead Letter Queue**
- Mensagens que falham após 3 tentativas
- Monitoramento via RabbitMQ Management

**Transações Distribuídas**
- Event Sourcing garante auditoria completa
- Idempotência em operações críticas

## 🧪 Testes

### Testes Unitários

```bash
dotnet test FiapCloudGames.Tests.Unit
```

**Cobertura:**
- ✅ AuthService (validações de email, senha)
- ✅ GameService (preço, rating, descontos)
- ✅ PaymentService (valor, status, impostos)

### Executar no Visual Studio

1. Abra **Test Explorer** (Test → Test Explorer)
2. Clique em **"Run All Tests"**
3. Visualize os resultados

## 📦 Estrutura do Projeto

```
FiapCloudGames/
├── FiapCloudGames.Users.Api/
├── FiapCloudGames.Games.Api/
├── FiapCloudGames.Payments.Api/
├── FiapCloudGames.Shared/
├── FiapCloudGames.EventSourcing/
├── FiapCloudGames.Lambda/
├── FiapCloudGames.Tests.Unit/
├── monitoring/
├── docker-compose.microservices.yml
├── azure-pipelines.yml
├── README.md
├── CHECKUP.md
└── LAMBDA.md
```

## 🔐 Segurança

### Autenticação JWT

Cada microsserviço tem sua própria chave JWT:

```json
{
  "Jwt": {
    "Key": "unique-guid-for-each-service",
    "Issuer": "fiap-cloud-games",
    "Audience": "fiap-cloud-games-api"
  }
}
```

### Banco de Dados Local

```
ConnectionStrings__DefaultConnection=Server=localhost;Database=FiapGames;Trusted_Connection=true;MultipleActiveResultSets=true;
```

## 🚀 AWS Lambda

### Funções Publicadas

1. **NotificationFunction**
   - Envia email via AWS SES
   - Destinatário: jonathan.nnt@hotmail.com
   - Acionada quando pagamento é concluído

2. **RecommendationFunction**
   - Consulta Elasticsearch
   - Retorna jogos recomendados
   - Baseado em rating e histórico

## 📊 CI/CD Pipeline

**Stages:**
1. **Build** - Compila FiapCloudGames.Microservices.sln
2. **Testes** - Executa testes unitários
3. **Docker** - Build e push de 3 imagens
4. **Development** - Deploy em dev
5. **Staging** - Deploy em staging
6. **Production** - Deploy em produção

**Gatilhos:**
- ✅ Pull Request
- ✅ Push em `dev`
- ✅ Push em `stage`
- ✅ Push em `prod`

## 🛠️ Tecnologias

| Tecnologia | Versão | Uso |
|------------|--------|-----|
| .NET | 8.0 | Framework principal |
| Entity Framework Core | 9.0 | ORM |
| SQL Server | 2022 | Banco de dados |
| Elasticsearch | 8.10 | Busca e recomendações |
| RabbitMQ | 3.13 | Mensageria |
| Serilog | - | Logging |
| Prometheus | - | Monitoramento |
| Grafana | - | Visualização |
| AWS Lambda | - | Serverless |

## 📝 Variáveis de Ambiente

```bash
ConnectionStrings__DefaultConnection=Server=localhost;Database=FiapGames;Trusted_Connection=true;MultipleActiveResultSets=true;
Jwt__Key=unique-guid-per-service
RabbitMq__Host=localhost
Elasticsearch__Url=http://localhost:9200
Email__RecipientEmail=jonathan.nnt@hotmail.com
```

## 🤝 Contribuindo

1. Crie uma branch (`git checkout -b feature/AmazingFeature`)
2. Commit suas mudanças (`git commit -m 'Add AmazingFeature'`)
3. Push para a branch (`git push origin feature/AmazingFeature`)
4. Abra um Pull Request

## 📄 Licença

Projeto acadêmico - FIAP Tech Challenge

---

**Desenvolvido por:** Jonathan Ornellas  
**FIAP - Pós-graduação em Arquitetura de Software**  
**Janeiro 2026**
