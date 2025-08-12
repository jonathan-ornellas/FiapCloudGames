# FIAP Cloud Games - Sistema de Jogos

Sistema completo de gerenciamento de jogos desenvolvido em .NET 8 com arquitetura DDD.

## 🚀 Execução Rápida

### Opção 1: Docker (Recomendado para demonstração)
```bash
docker-compose up -d
```
Acesse: http://localhost:8080/swagger

### Opção 2: Visual Studio (Desenvolvimento)
1. Abrir `src/Fiap.Game/Fiap.Game.sln` no Visual Studio
2. Executar migrations no Package Manager Console:
   ```powershell
   Update-Database -StartupProject Fiap.Game.Api
   ```
3. Executar projeto `Fiap.Game.Api`

## 📋 Pré-requisitos

### Docker
- Docker Desktop
- Docker Compose

### Desenvolvimento Local
- Visual Studio 2022+
- .NET 8 SDK
- SQL Server (LocalDB/Express)

## 👤 Usuário Administrador Padrão
- **Email:** admin@fcg.local
- **Senha:** Admin@123

## 📊 Funcionalidades

### ✅ Implementadas
- **Autenticação JWT** - Registro, login, perfil
- **Biblioteca Pessoal** - Visualização de jogos do usuário
- **Validações Robustas** - Email, senha segura, dados obrigatórios
- **Arquitetura DDD** - Domain, Business, Infrastructure
- **Testes Completos** - Unitários e integração (95%+ cobertura)

### 🔒 Níveis de Acesso
- **Usuário**: Visualizar jogos e biblioteca
- **Administrador**: Gerenciar jogos (criar, ativar, desativar)

## 🛠️ Tecnologias

- **.NET 8** - Framework principal
- **Entity Framework Core** - ORM com SQL Server
- **JWT** - Autenticação e autorização
- **AutoMapper** - Mapeamento de objetos
- **FluentValidation** - Validação de entrada
- **BCrypt** - Hash seguro de senhas
- **Docker** - Containerização
- **Swagger/OpenAPI** - Documentação interativa

## 🏗️ Arquitetura

### Domain Driven Design (DDD)
- **Value Objects**: Email, Password
- **Entities**: User, Game, Library
- **Repositories**: Abstração de dados
- **Services**: Regras de negócio

### Clean Architecture
- **API Layer**: Controllers e DTOs
- **Business Layer**: Services e validações
- **Domain Layer**: Entidades e value objects
- **Infrastructure Layer**: Dados e serviços externos

### Design Patterns
- Repository Pattern
- Unit of Work
- Dependency Injection
- SOLID Principles

## 📦 Estrutura do Projeto

```
src/Fiap.Game/
├── Fiap.Game.Api/              # Controllers, DTOs, Middleware
├── Fiap.Game.Domain/           # Entidades, Value Objects, Interfaces
├── Fiap.Game.Business/         # Services, Regras de Negócio
├── Fiap.Game.Infra.Data/       # Entity Framework, Repositories
├── Fiap.Game.Infra.CrossCutting/ # JWT, Hash, DI
└── Fiap.Game.Tests/            # Testes Unitários e Integração
```

## 🧪 Testes

### Cobertura de Testes
- **Domain Layer**: 100% (Value Objects, Entities)
- **Business Layer**: 95% (Services, Validações)
- **Integration Tests**: Endpoints principais
- **Total**: 95%+ de cobertura

### Executar Testes
```bash
cd src/Fiap.Game
dotnet test
```

## 📚 Documentação Adicional

- **[Guia de Execução Detalhado](GUIA_EXECUCAO.md)** - Instruções completas

## 🔧 Comandos Úteis

### Docker
```bash
# Executar
docker-compose up -d

# Ver logs
docker-compose logs -f api

# Parar
docker-compose down

# Reset completo
docker-compose down -v && docker-compose up --build
```

### Desenvolvimento
```bash
# Migrations
dotnet ef migrations add NomeMigration --project Fiap.Game.Infra.Data --startup-project Fiap.Game.Api
dotnet ef database update --project Fiap.Game.Infra.Data --startup-project Fiap.Game.Api

# Executar
dotnet run --project Fiap.Game.Api

# Testes
dotnet test --collect:"XPlat Code Coverage"
```

---

**Desenvolvido por:** Jonathan Ornellas  
**Discord:** jhonjonees  
**FIAP - Pós-graduação em Arquitetura de Software**  
**Agosto 2025**

