# FiapCloudGames - User Service

Microsserviço responsável por **autenticação, cadastro e gerenciamento de perfis de usuários** da plataforma FiapCloudGames.

**Projeto de Estudo - FIAP Tech Challenge - Tarefa 3**

## 🚀 Execução Rápida

### Docker Compose (Recomendado)

```bash
docker-compose up -d
```

Acesse:
- **User API:** http://localhost:5001/swagger
- **SQL Server:** localhost:1433

### Multi-Projeto no Visual Studio

1. Abra `FiapCloudGames.Users.sln`
2. Clique com botão direito na solução
3. Selecione "Set Startup Projects"
4. Escolha "Multiple startup projects"
5. Marque `FiapCloudGames.Users.Api`
6. Clique em "Start"

## 📋 Pré-requisitos

- .NET 8 SDK
- Docker e Docker Compose
- Visual Studio 2022 ou VS Code
- Git
- SQL Server (LocalDB ou Express)

## 🏗️ Arquitetura

### Microsserviço User

| Componente | Porta | Descrição |
|-----------|-------|----------|
| **User API** | 5001 | Autenticação, cadastro, gerenciamento de perfis |
| **SQL Server** | 1433 | Banco de dados do User Service |

### Comunicação com Outros Microsserviços

Este microsserviço fornece autenticação JWT para:
- **Games Service** (Catálogo de jogos)
- **Payment Service** (Processamento de pagamentos)

## 📊 Endpoints da API

### Autenticação

| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| POST | `/api/auth/register` | Registrar novo usuário | ❌ |
| POST | `/api/auth/login` | Fazer login e obter JWT token | ❌ |

### Perfil de Usuário

| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/api/users/profile` | Obter perfil próprio | ✅ User |
| PUT | `/api/users/profile` | Atualizar perfil próprio | ✅ User |

### Administração

| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/api/users` | Listar todos os usuários | ✅ Admin |
| PUT | `/api/users/{id}/role` | Alterar role de usuário | ✅ Admin |

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

## 📊 Fluxo de Autenticação

```
1. Usuário envia credenciais (email, senha)
   ↓
2. POST /api/auth/register ou /api/auth/login
   ↓
3. Validação de credenciais
   ↓
4. Geração de JWT Token
   ↓
5. Retorno do token para o cliente
   ↓
6. Cliente inclui token em requisições autenticadas
   ↓
7. Validação do token em cada requisição
   ↓
8. Acesso concedido/negado conforme role
```

## 🔄 Fluxo de Comunicação com Outros Microsserviços

```
┌─────────────────────────────────────────────────────────┐
│                    User Service                         │
│  (Autenticação, Cadastro, Gerenciamento de Perfis)     │
└─────────────────────────────────────────────────────────┘
                            │
                            │ JWT Token
                            ↓
        ┌───────────────────┴───────────────────┐
        │                                       │
        ↓                                       ↓
┌──────────────────┐                  ┌──────────────────┐
│  Gamer Service   │                  │ Payment Service  │
│  (Catálogo de    │                  │ (Processamento   │
│   Jogos)         │                  │  de Pagamentos)  │
└──────────────────┘                  └──────────────────┘
```

## 🧪 Testes

### Testes Unitários

```bash
dotnet test
```

**Cobertura:**
- ✅ AuthService
- ✅ UserService
- ✅ Validators
- ✅ ValueObjects (Email)

### Testes de Integração

```bash
dotnet test --filter "Integration"
```

**Cobertura:**
- ✅ Endpoints da API
- ✅ Autenticação e Autorização
- ✅ Validação de dados

## 📦 Estrutura do Projeto

```
FiapCloudGames-User/
├── src/
│   ├── Services/
│   │   └── Users/
│   │       ├── FiapCloudGames.Users.Api/
│   │       │   ├── Controllers/
│   │       │   ├── DTOs/
│   │       │   ├── Validators/
│   │       │   ├── Services/
│   │       │   ├── Repositories/
│   │       │   ├── Data/
│   │       │   └── Program.cs
│   │       └── FiapCloudGames.Users.Business/
│   │           ├── Services/
│   │           ├── Interfaces/
│   │           └── Mappers/
│   └── Shared/
│       ├── FiapCloudGames.Domain/
│       ├── FiapCloudGames.Shared/
│       └── FiapCloudGames.EventSourcing/
├── tests/
├── Dockerfile
├── docker-compose.yml
├── azure-pipelines.yml
└── FiapCloudGames.Users.sln
```

### Organização Lógica (Visual Studio)

```
Solution 'FiapCloudGames.Users'
├── 📁 1 - Presentation
│   └── FiapCloudGames.Users.Api
├── 📁 2 - Application
│   └── FiapCloudGames.Users.Business
└── 📁 3 - Shared
    ├── FiapCloudGames.Domain
    ├── FiapCloudGames.Shared
    └── FiapCloudGames.EventSourcing
```

## 💾 Banco de Dados

### Tabelas Principais

**Users**
- UserId (PK)
- Email (Unique)
- PasswordHash
- FirstName
- LastName
- Role (User/Admin)
- CreatedAt
- UpdatedAt

## 📊 CI/CD Pipeline (Azure DevOps)

A pipeline está configurada para:

- ✅ Build automático em Pull Request
- ✅ Execução de testes unitários
- ✅ Execução de testes de integração
- ✅ Deploy em stage (apenas merge em stage)
- ✅ Análise de código
- ✅ Geração de artefatos

**Gatilhos:**
- Pull Request → Build + Tests
- Push em stage → Deploy em Staging

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

```bash
# Banco de Dados
ConnectionStrings__DefaultConnection=Server=localhost;Database=FiapGameUsers;User Id=sa;Password=YourPassword;Encrypt=false;

# JWT
Jwt__Key=sua-chave-secreta-aqui-com-minimo-32-caracteres
Jwt__Issuer=fiap-cloud-games
Jwt__Audience=fiap-cloud-games-api
Jwt__ExpirationMinutes=60

# Logging
Logging__LogLevel__Default=Information
```

## 📚 Documentação Adicional

- 📄 **[SETUP.md](./SETUP.md)** - Instruções detalhadas de setup
- 📄 **[azure-pipelines.yml](./azure-pipelines.yml)** - Configuração de CI/CD
- 📄 **[Dockerfile](./Dockerfile)** - Build da imagem Docker
- 📄 **[docker-compose.yml](./docker-compose.yml)** - Orquestração local

## 🤝 Contribuindo

1. Fork o projeto
2. Crie uma branch (`git checkout -b feature/NovaFuncionalidade`)
3. Commit suas mudanças (`git commit -m 'Adiciona NovaFuncionalidade'`)
4. Push para a branch (`git push origin feature/NovaFuncionalidade`)
5. Abra um Pull Request

### Padrões de Código
- ✅ Seguir convenções C#
- ✅ Usar async/await
- ✅ Adicionar testes para novas funcionalidades
- ✅ Documentar código complexo

---

## 📝 Licença

Este projeto é parte do FIAP Tech Challenge - Tarefa 3.

---

## 👤 Autor

**Jonathan Nogueira Ornellas**
- Discord: jhonjonees#2864

---

**Última atualização:** Janeiro de 2026
