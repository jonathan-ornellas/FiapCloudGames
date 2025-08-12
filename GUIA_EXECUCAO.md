# 🚀 Guia de Execução - FIAP Cloud Games

## 📋 Pré-requisitos

### Para Execução Local (Visual Studio)
- Visual Studio 2022 ou superior
- .NET 8 SDK
- SQL Server (LocalDB ou SQL Server Express)
- SQL Server Management Studio (opcional)

### Para Execução com Docker
- Docker Desktop
- Docker Compose

## 🏠 Execução Local (Desenvolvimento)

### 1. Configuração do Banco de Dados

O projeto está configurado para usar **SQL Server LocalDB** por padrão.

**Connection String padrão:**
```
Server=(localdb)\\mssqllocaldb;Database=FiapGameDb;Trusted_Connection=true;MultipleActiveResultSets=true
```

### 2. Executar Migrations

No **Package Manager Console** do Visual Studio:

```powershell
# Selecionar projeto padrão: Fiap.Game.Infra.Data
Add-Migration InitialCreate -StartupProject Fiap.Game.Api
Update-Database -StartupProject Fiap.Game.Api
```

Ou via **linha de comando**:

```bash
cd src/Fiap.Game
dotnet ef migrations add InitialCreate --project Fiap.Game.Infra.Data --startup-project Fiap.Game.Api
dotnet ef database update --project Fiap.Game.Infra.Data --startup-project Fiap.Game.Api
```

### 3. Executar a Aplicação

**No Visual Studio:**
- Definir `Fiap.Game.Api` como projeto de inicialização
- Pressionar `F5` ou `Ctrl + F5`

**Via linha de comando:**
```bash
cd src/Fiap.Game
dotnet run --project Fiap.Game.Api
```

### 4. Acessar a Aplicação

- **API**: https://localhost:7000 ou http://localhost:5000
- **Swagger**: https://localhost:7000/swagger

## 🐳 Execução com Docker

### 1. Executar Docker Compose

```bash
# Na raiz do projeto
docker-compose up -d
```

### 2. Aguardar Inicialização

A primeira execução pode levar alguns minutos para:
- Baixar imagens do SQL Server e .NET
- Compilar a aplicação
- Executar migrations
- Inicializar os serviços

### 3. Acessar a Aplicação

- **API**: http://localhost:8080
- **Swagger**: http://localhost:8080/swagger

### 4. Comandos Úteis

```bash
# Ver logs da API
docker-compose logs -f api

# Ver logs do SQL Server
docker-compose logs -f sqlserver

# Parar os serviços
docker-compose down

# Reconstruir e executar
docker-compose up --build

# Remover volumes (reset completo)
docker-compose down -v
```

## 👤 Usuário Administrador Padrão

**Email:** admin@fcg.local  
**Senha:** Admin@123

## 🧪 Executar Testes

### Testes Unitários e de Integração

```bash
cd src/Fiap.Game
dotnet test
```

### Testes com Cobertura

```bash
cd src/Fiap.Game
dotnet test --collect:"XPlat Code Coverage"
```

## 📊 Endpoints Disponíveis

### Autenticação
- `POST /api/auth/register` - Registrar usuário
- `POST /api/auth/login` - Login
- `GET /api/auth/me` - Perfil do usuário

### Jogos
- `GET /api/games` - Listar jogos
- `POST /api/games` - Criar jogo (Admin)
- `PUT /api/games/{id}/activate` - Ativar jogo (Admin)
- `PUT /api/games/{id}/deactivate` - Desativar jogo (Admin)

### Biblioteca
- `GET /api/library` - Biblioteca do usuário

## 🔧 Configurações Personalizadas

### Alterar Connection String

**Para desenvolvimento local**, edite `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "Default": "SUA_CONNECTION_STRING_AQUI"
  }
}
```

**Para Docker**, edite `docker-compose.yml`:

```yaml
environment:
  - ConnectionStrings__Default=SUA_CONNECTION_STRING_AQUI
```

### Configurar JWT

Edite as configurações JWT nos arquivos `appsettings.json`:

```json
{
  "Jwt": {
    "Key": "SUA_CHAVE_SECRETA_AQUI",
    "Issuer": "SeuIssuer",
    "Audience": "SeuAudience"
  }
}
```

## 🚨 Solução de Problemas

### Erro de Connection String
- Verifique se o SQL Server está rodando
- Confirme a connection string no appsettings.json
- Execute as migrations

### Erro no Docker
- Verifique se o Docker está rodando
- Execute `docker-compose down -v` e tente novamente
- Verifique os logs com `docker-compose logs`

### Erro de Migrations
- Delete a pasta `Migrations` se existir
- Execute `Add-Migration InitialCreate` novamente
- Execute `Update-Database`

---

**Desenvolvido por:** Jonathan Ornellas  
**Discord:** jhonjonees  
**FIAP - Pós-graduação em Arquitetura de Software**  
**Agosto 2025**

