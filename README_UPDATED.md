# FIAP Cloud Games - Sistema de Jogos

Sistema completo de gerenciamento de jogos desenvolvido em .NET 8 com arquitetura DDD, CI/CD automático e monitoramento em tempo real.

## 🚀 Quick Start

### Opção 1: Docker (Recomendado para demonstração)

```bash
docker-compose up -d
```

Acesse: http://localhost:8080/swagger

### Opção 2: Docker com Monitoramento

```bash
docker-compose -f docker-compose.monitoring.yml up -d
```

Acesse:
- API: http://localhost:8080/swagger
- Grafana: http://localhost:3000 (admin/admin123)
- Prometheus: http://localhost:9090

### Opção 3: Visual Studio (Desenvolvimento)

1. Abrir `Fiap.Game.sln` no Visual Studio
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
- **CI/CD Automático** - GitHub Actions com deploy na AWS
- **Monitoramento** - Prometheus, Grafana, Alertmanager
- **Containerização** - Docker multistage build

### 🔒 Níveis de Acesso

- **Usuário**: Visualizar jogos e biblioteca
- **Administrador**: Gerenciar jogos (criar, ativar, desativar)

## 🛠️ Tecnologias

### Backend
- **.NET 8** - Framework principal
- **Entity Framework Core** - ORM com SQL Server
- **JWT** - Autenticação e autorização
- **AutoMapper** - Mapeamento de objetos
- **FluentValidation** - Validação de entrada
- **BCrypt** - Hash seguro de senhas

### DevOps & Infraestrutura
- **Docker** - Containerização
- **Docker Compose** - Orquestração local
- **GitHub Actions** - CI/CD automático
- **AWS EC2** - Hospedagem em nuvem
- **AWS Systems Manager** - Deploy remoto

### Monitoramento
- **Prometheus** - Coleta de métricas
- **Grafana** - Visualização de dados
- **Alertmanager** - Gerenciamento de alertas
- **Node Exporter** - Métricas do sistema
- **cAdvisor** - Métricas de containers

### Documentação
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
FiapCloudGames/
├── .github/
│   └── workflows/
│       └── ci-cd-deploy.yml          # Pipeline CI/CD automática
├── Fiap.Game.Api/                    # Controllers, DTOs, Middleware
├── Fiap.Game.Domain/                 # Entidades, Value Objects, Interfaces
├── Fiap.Game.Business/               # Services, Regras de Negócio
├── Fiap.Game.Infra.Data/             # Entity Framework, Repositories
├── Fiap.Game.Infra.CrossCutting/     # JWT, Hash, DI
├── Fiap.Game.Tests/                  # Testes Unitários e Integração
├── monitoring/                        # Configurações de monitoramento
│   ├── prometheus.yml                # Configuração Prometheus
│   ├── alertmanager.yml              # Configuração Alertmanager
│   ├── alert_rules.yml               # Regras de alerta
│   └── grafana-provisioning/         # Dashboards e datasources
├── scripts/
│   └── deploy-windows.ps1            # Script de deploy Windows
├── Dockerfile                        # Build multistage
├── docker-compose.yml                # Composição local
├── docker-compose.monitoring.yml     # Composição com monitoramento
├── DEPLOYMENT_GUIDE.md               # Guia de deployment
├── MONITORING_GUIDE.md               # Guia de monitoramento
└── README.md                         # Este arquivo
```

## 🧪 Testes

### Cobertura de Testes

- **Domain Layer**: 100% (Value Objects, Entities)
- **Business Layer**: 95% (Services, Validações)
- **Integration Tests**: Endpoints principais
- **Total**: 95%+ de cobertura

### Executar Testes

```bash
cd Fiap.Game
dotnet test
```

### Testes com Cobertura

```bash
dotnet test --collect:"XPlat Code Coverage"
```

## 🚀 CI/CD Pipeline

### Fluxo Automático

```
Push na branch stage
        ↓
GitHub Actions Triggered
        ↓
Build & Test (CI)
        ↓
Docker Build & Push (Docker Hub)
        ↓
Deploy na AWS EC2 (via Systems Manager)
        ↓
Aplicação rodando em produção
```

### Configuração

1. **GitHub Secrets** necessários:
   - `AWS_ACCESS_KEY_ID`
   - `AWS_SECRET_ACCESS_KEY`
   - `AWS_REGION`
   - `EC2_INSTANCE_ID`
   - `DOCKERHUB_USERNAME`
   - `DOCKERHUB_TOKEN`

2. **Arquivo de pipeline**: `.github/workflows/ci-cd-deploy.yml`

3. **Triggers**:
   - Push na branch `stage`, `main` ou `master`
   - Pull Requests
   - Manual via GitHub Actions UI

Veja [DEPLOYMENT_GUIDE.md](DEPLOYMENT_GUIDE.md) para detalhes completos.

## 📊 Monitoramento em Tempo Real

### Stack de Monitoramento

- **Prometheus**: Coleta de métricas (port 9090)
- **Grafana**: Visualização (port 3000)
- **Alertmanager**: Gerenciamento de alertas (port 9093)
- **Node Exporter**: Métricas do sistema
- **cAdvisor**: Métricas de containers

### Iniciar Monitoramento

```bash
docker-compose -f docker-compose.monitoring.yml up -d
```

### Acessar Dashboards

- **Grafana**: http://localhost:3000
- **Prometheus**: http://localhost:9090
- **Alertmanager**: http://localhost:9093

### Alertas Configurados

- CPU > 80% (warning) / > 95% (critical)
- Memória > 80% (warning) / > 95% (critical)
- Disco > 80% (warning) / > 95% (critical)
- Container down (critical)
- Taxa de erros 5xx > 5% (warning)
- Latência P95 > 2s (warning)

Veja [MONITORING_GUIDE.md](MONITORING_GUIDE.md) para detalhes completos.

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

### Deploy Manual (Windows)

```powershell
# Executar script de deploy
.\scripts\deploy-windows.ps1

# Ou com parâmetros
.\scripts\deploy-windows.ps1 -HostPort 8080 -DeployPath "C:\inetpub\fiapGames"
```

## 📚 Documentação Adicional

- **[Guia de Deployment](DEPLOYMENT_GUIDE.md)** - Setup AWS, GitHub Actions, deploy automático
- **[Guia de Monitoramento](MONITORING_GUIDE.md)** - Prometheus, Grafana, alertas
- **[Guia de Execução Detalhado](GUIA_EXECUCAO.md)** - Instruções completas de execução

## 🌐 Endpoints Principais

### Autenticação

- `POST /api/auth/register` - Registrar novo usuário
- `POST /api/auth/login` - Fazer login
- `GET /api/auth/profile` - Obter perfil do usuário

### Jogos

- `GET /api/games` - Listar todos os jogos
- `GET /api/games/{id}` - Obter detalhes do jogo
- `POST /api/games` - Criar novo jogo (admin)
- `PUT /api/games/{id}` - Atualizar jogo (admin)
- `DELETE /api/games/{id}` - Deletar jogo (admin)

### Biblioteca

- `GET /api/library` - Listar jogos do usuário
- `POST /api/library/{gameId}` - Adicionar jogo à biblioteca
- `DELETE /api/library/{gameId}` - Remover jogo da biblioteca

### Health Check

- `GET /health` - Status da aplicação
- `GET /swagger` - Documentação interativa

## 🐛 Troubleshooting

### Aplicação não inicia

```bash
# Verificar logs
docker-compose logs api

# Verificar banco de dados
docker-compose logs db
```

### Testes falham

```bash
# Restaurar dependências
dotnet restore

# Executar testes com output detalhado
dotnet test --verbosity detailed
```

### Deploy falha

Veja [DEPLOYMENT_GUIDE.md - Troubleshooting](DEPLOYMENT_GUIDE.md#-troubleshooting)

## 🤝 Contribuindo

1. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
2. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
3. Push para a branch (`git push origin feature/AmazingFeature`)
4. Abra um Pull Request

## 📝 Licença

Este projeto é parte do programa de Pós-graduação em Arquitetura de Software da FIAP.

---

**Desenvolvido por:** Jonathan Ornellas  
**Discord:** jhonjonees  
**FIAP - Pós-graduação em Arquitetura de Software**  
**Última atualização:** Janeiro 2026

## 🎯 Próximos Passos

- [ ] Implementar cache distribuído (Redis)
- [ ] Adicionar rate limiting
- [ ] Implementar logging centralizado (ELK Stack)
- [ ] Adicionar testes de carga
- [ ] Implementar API Gateway
- [ ] Adicionar autoscaling na AWS
- [ ] Implementar disaster recovery
