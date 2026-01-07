# FiapCloudGames - Tarefa 3: Arquitetura de Microsserviços

> **Projeto de Estudo** - FIAP Tech Challenge - Tarefa 3

Sistema completo de gerenciamento de jogos desenvolvido em .NET 8 com arquitetura de microsserviços, Elasticsearch, Event Sourcing, RabbitMQ e sistema completo de autenticação e autorização.

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
- SQL Server (LocalDB ou Express)

## 🏗️ Arquitetura

### Microsserviços (3)

| Serviço | Porta | Responsabilidade |
|---------|-------|------------------|
| **Users API** | 5001 | Autenticação, cadastro, gerenciamento de perfis e roles |
| **Games API** | 5002 | Catálogo, compras, biblioteca pessoal e recomendações |
| **Payments API** | 5003 | Processamento e histórico de pagamentos |

### Componentes de Infraestrutura

| Componente | Porta | Descrição |
|-----------|-------|-----------|
| **SQL Server** | 1433 | 3 bancos de dados separados (Users, Games, Payments) |
| **Elasticsearch** | 9200 | Busca e indexação de jogos |
| **RabbitMQ** | 5672 | Fila de mensagens assíncrona |
| **Prometheus** | 9090 | Coleta de métricas |
| **Grafana** | 3000 | Visualização de métricas |
| **AlertManager** | 9093 | Gerenciamento de alertas |

## 📊 Endpoints Completos

### 🔐 Users API (5001)

#### Autenticação
```
POST   /api/auth/register           - Registrar novo usuário (role: User)
POST   /api/auth/login              - Fazer login e obter JWT token
```

#### Perfil de Usuário
```
GET    /api/users/profile           - Obter perfil próprio          [Auth: User]
PUT    /api/users/profile           - Atualizar perfil próprio      [Auth: User]
```

#### Administração (Admin apenas)
```
GET    /api/users                   - Listar todos os usuários      [Auth: Admin]
PUT    /api/users/{id}/role         - Alterar role de usuário       [Auth: Admin]
```

### 🎮 Games API (5002)

#### Catálogo Público
```
GET    /api/games                   - Listar todos os jogos disponíveis
GET    /api/games/{id}              - Obter detalhes de um jogo
GET    /api/games/search            - Buscar jogos (Elasticsearch)
```

#### Administração (Admin apenas)
```
POST   /api/games                   - Cadastrar novo jogo           [Auth: Admin]
PUT    /api/games/{id}              - Atualizar jogo                [Auth: Admin]
DELETE /api/games/{id}              - Deletar jogo                  [Auth: Admin]
```

#### Biblioteca e Recomendações (Usuários autenticados)
```
POST   /api/games/purchase          - Comprar um jogo               [Auth: User]
GET    /api/games/library           - Ver biblioteca pessoal        [Auth: User]
GET    /api/games/recommendations   - Obter recomendações personalizadas [Auth: User]
```

### 💳 Payments API (5003)

#### Processamento
```
POST   /api/payments                - Processar pagamento           [Auth: User]
```

#### Consulta
```
GET    /api/payments/{id}           - Consultar pagamento específico [Auth: User/Admin]
GET    /api/payments/user           - Histórico de pagamentos       [Auth: User]
```

#### Administração (Admin apenas)
```
PUT    /api/payments/{id}/status    - Atualizar status do pagamento [Auth: Admin]
```

## 🔐 Sistema de Autenticação e Autorização

### Níveis de Acesso

#### 👤 Usuário (User)
- ✅ Cadastro e login
- ✅ Visualizar e atualizar perfil próprio
- ✅ Navegar pelo catálogo de jogos
- ✅ Comprar jogos
- ✅ Acessar biblioteca pessoal
- ✅ Ver recomendações personalizadas
- ✅ Processar pagamentos
- ✅ Consultar histórico de pagamentos próprio

#### 👨‍💼 Administrador (Admin)
- ✅ Todas as permissões de usuário
- ✅ Cadastrar, atualizar e deletar jogos
- ✅ Listar todos os usuários do sistema
- ✅ Alterar roles de usuários
- ✅ Consultar todos os pagamentos
- ✅ Atualizar status de pagamentos

### Segurança JWT

Cada microsserviço valida tokens JWT com:
- **Claims incluídas:**
  - `sub` (User ID)
  - `email` (E-mail do usuário)
  - `name` (Nome do usuário)
  - `role` (User ou Admin)

```json
{
  "Jwt": {
    "Key": "sua-chave-secreta-aqui",
    "Issuer": "fiap-cloud-games",
    "Audience": "fiap-cloud-games-api"
  }
}
```

### Validações de Segurança

#### E-mail
- ✅ Formato válido (regex)
- ✅ Normalização (lowercase, trim)
- ✅ Limite de 180 caracteres
- ✅ Unicidade no sistema

#### Senha
- ✅ Mínimo 8 caracteres
- ✅ Pelo menos 1 letra maiúscula
- ✅ Pelo menos 1 letra minúscula
- ✅ Pelo menos 1 número
- ✅ Pelo menos 1 caractere especial
- ✅ Hash com BCrypt

## 🔄 Fluxo de Compra de Jogo

```
1. Usuário seleciona jogo
   ↓
2. POST /api/games/purchase (Games API)
   ↓
3. Validação: jogo existe? já comprado?
   ↓
4. Adiciona à biblioteca (UserLibrary)
   ↓
5. POST /api/payments (Payments API)
   ↓
6. Processa pagamento
   ↓
7. Publica evento PaymentProcessed (RabbitMQ)
   ↓
8. Lambda envia notificação por email
   ↓
9. Jogo disponível na biblioteca
```

## 🎯 Sistema de Recomendações

### Algoritmo
1. Busca jogos **NÃO** comprados pelo usuário
2. Ordena por **rating** (maior para menor)
3. Retorna top N jogos (padrão: 10)

### Exemplo
```bash
GET /api/games/recommendations?limit=5
Authorization: Bearer {seu-token}
```

**Resposta:**
```json
[
  {
    "id": 15,
    "title": "God of War",
    "description": "Aventura épica...",
    "genre": "Action-Adventure",
    "rating": 9.8,
    "price": 199.90
  }
]
```

## 🛡️ Resiliência e Confiabilidade

### Retry Automático (RabbitMQ)
- ✅ 3 tentativas com backoff exponencial
- ✅ Intervalo inicial: 1 segundo
- ✅ Multiplicador: 2x a cada tentativa

### Dead Letter Queue
- ✅ Mensagens que falham após 3 tentativas
- ✅ Monitoramento via RabbitMQ Management

### Event Sourcing
- ✅ Auditoria completa de todas as operações
- ✅ Histórico imutável de eventos
- ✅ Capacidade de replay

### Transações Distribuídas
- ✅ Idempotência em operações críticas
- ✅ Rollback via compensação

## 🧪 Testes

### Testes Unitários (153 testes - 100% sucesso)

```bash
dotnet test tests/FiapCloudGames.Tests.Unit
```

**Cobertura:**
- ✅ **Services** (AuthService, GameService, PaymentService)
- ✅ **ValueObjects** (Email, Money)
- ✅ **Validators** (RegisterUserDto, CreateGameDto, ProcessPaymentDto)

### Testes de Integração

```bash
dotnet test tests/FiapCloudGames.Tests.Integration
```

**Cobertura:**
- ✅ DTOs de todas as APIs
- ✅ Validação de propriedades
- ✅ Criação de objetos

### Executar Todos os Testes

```bash
dotnet test
```

### Executar no Visual Studio

1. Abra **Test Explorer** (`Ctrl + E, T`)
2. Clique em **"Run All Tests"**
3. Visualize os resultados em tempo real

## 📦 Estrutura do Projeto

```
FiapCloudGames/
├── src/
│   ├── Services/
│   │   ├── Users/
│   │   │   ├── FiapCloudGames.Users.Api/
│   │   │   └── FiapCloudGames.Users.Business/
│   │   ├── Games/
│   │   │   ├── FiapCloudGames.Games.Api/
│   │   │   └── FiapCloudGames.Games.Business/
│   │   └── Payments/
│   │       ├── FiapCloudGames.Payments.Api/
│   │       ├── FiapCloudGames.Payments.Business/
│   │       └── FiapCloudGames.Lambda/
│   └── Shared/
│       ├── FiapCloudGames.Domain/
│       ├── FiapCloudGames.EventSourcing/
│       └── FiapCloudGames.Shared/
├── tests/
│   ├── FiapCloudGames.Tests.Unit/
│   └── FiapCloudGames.Tests.Integration/
├── monitoring/
├── docker-compose.microservices.yml
├── azure-pipelines.yml
├── README.md
├── ANALISE_REQUISITOS.md
└── IMPLEMENTACOES_CONCLUIDAS.md
```

## 🗄️ Modelo de Dados

### Users Database
- **Users** - Informações de usuários e roles

### Games Database
- **Games** - Catálogo de jogos
- **UserLibraries** - Biblioteca pessoal de cada usuário

### Payments Database
- **Payments** - Histórico de transações

### Event Sourcing Database
- **Events** - Registro de todos os eventos do sistema

## 🚀 AWS Lambda

### Funções Publicadas

#### 1. NotificationFunction
- **Trigger:** RabbitMQ (PaymentProcessedEvent)
- **Ação:** Envia email via AWS SES
- **Destinatário:** Configurável via variável de ambiente
- **Template:** Confirmação de pagamento

#### 2. RecommendationFunction
- **Trigger:** HTTP API Gateway
- **Ação:** Consulta Elasticsearch
- **Retorno:** Top N jogos recomendados
- **Critérios:** Rating e histórico de compras

## 📊 CI/CD Pipeline (Azure DevOps)

### Stages

1. **Build**
   - Compila todos os projetos
   - Restaura pacotes NuGet
   - Valida code style

2. **Tests**
   - Executa testes unitários
   - Executa testes de integração
   - Gera relatório de cobertura

3. **Docker**
   - Build de 3 imagens Docker
   - Push para Docker Hub/ACR
   - Tag com número da build

4. **Deploy Development**
   - Deploy automático em dev
   - Smoke tests

5. **Deploy Staging**
   - Deploy com aprovação manual
   - Testes de aceitação

6. **Deploy Production**
   - Deploy com aprovação manual
   - Rollback automático se falhar

### Gatilhos
- ✅ Pull Request → Build + Tests
- ✅ Push em `dev` → Deploy em Development
- ✅ Push em `stage` → Deploy em Staging
- ✅ Push em `main` → Deploy em Production

## 🛠️ Tecnologias e Versões

| Tecnologia | Versão | Uso |
|------------|--------|-----|
| **.NET** | 8.0 | Framework principal |
| **C#** | 12.0 | Linguagem |
| **Entity Framework Core** | 9.0 | ORM |
| **SQL Server** | 2022 | Banco de dados relacional |
| **Elasticsearch** | 8.10 | Busca full-text e recomendações |
| **RabbitMQ** | 3.13 | Message broker |
| **FluentValidation** | 12.0 | Validação de dados |
| **JWT Bearer** | 8.0 | Autenticação |
| **Serilog** | 4.1 | Logging estruturado |
| **xUnit** | 2.4 | Framework de testes |
| **Moq** | 4.20 | Mocking |
| **Prometheus** | Latest | Métricas |
| **Grafana** | Latest | Visualização |
| **AWS Lambda** | .NET 8 | Serverless functions |
| **Docker** | Latest | Containerização |

## 📝 Variáveis de Ambiente

### Users API
```bash
ConnectionStrings__DefaultConnection=Server=localhost;Database=FiapGamesUsers;...
Jwt__Key=your-secret-key-here
Jwt__Issuer=fiap-cloud-games
Jwt__Audience=fiap-cloud-games-api
```

### Games API
```bash
ConnectionStrings__DefaultConnection=Server=localhost;Database=FiapGamesGames;...
Jwt__Key=your-secret-key-here
Elasticsearch__Url=http://localhost:9200
RabbitMq__Host=localhost
RabbitMq__Username=guest
RabbitMq__Password=guest
```

### Payments API
```bash
ConnectionStrings__DefaultConnection=Server=localhost;Database=FiapGamesPayments;...
Jwt__Key=your-secret-key-here
RabbitMq__Host=localhost
RabbitMq__Username=guest
RabbitMq__Password=guest
```

## 🎓 Documentação Adicional

- 📄 **[ANALISE_REQUISITOS.md](ANALISE_REQUISITOS.md)** - Análise completa dos requisitos
- 📄 **[IMPLEMENTACOES_CONCLUIDAS.md](IMPLEMENTACOES_CONCLUIDAS.md)** - Detalhes de implementação
- 📄 **[GUIA_EXECUCAO.md](GUIA_EXECUCAO.md)** - Guia passo a passo de execução

## 🤝 Contribuindo

1. Fork o projeto
2. Crie uma branch (`git checkout -b feature/NovaFuncionalidade`)
3. Commit suas mudanças (`git commit -m 'Adiciona NovaFuncionalidade'`)
4. Push para a branch (`git push origin feature/NovaFuncionalidade`)
5. Abra um Pull Request

### Padrões de Código
- ✅ Seguir convenções C#
- ✅ Usar async/await
- ✅ Escrever testes unitários
- ✅ Documentar APIs com Swagger
- ✅ Não usar comentários desnecessários

## 📄 Licença

Projeto acadêmico desenvolvido para o Tech Challenge da FIAP.

## 👤 Autor

**Jonathan Ornellas**
- GitHub: [@jonathan-ornellas](https://github.com/jonathan-ornellas)
- LinkedIn: [Jonathan Ornellas](https://linkedin.com/in/jonathan-ornellas)

## 🏆 Conquistas do Projeto

- ✅ 100% dos requisitos obrigatórios implementados
- ✅ 153 testes unitários (100% de sucesso)
- ✅ Arquitetura de microsserviços completa
- ✅ Sistema de autenticação e autorização robusto
- ✅ Event Sourcing e mensageria assíncrona
- ✅ CI/CD pipeline configurado
- ✅ Monitoramento com Prometheus e Grafana
- ✅ Documentação completa

---

**FIAP - Pós-graduação em Arquitetura de Software**  
**Tech Challenge - Tarefa 3**  
**Janeiro 2025**
