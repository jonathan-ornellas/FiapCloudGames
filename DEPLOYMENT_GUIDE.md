# 🚀 Guia Completo de Deployment - FiapCloudGames

## 📋 Índice

1. [Visão Geral](#visão-geral)
2. [Pré-requisitos](#pré-requisitos)
3. [Configuração AWS](#configuração-aws)
4. [Pipeline CI/CD](#pipeline-cicd)
5. [Deploy Manual](#deploy-manual)
6. [Monitoramento](#monitoramento)
7. [Troubleshooting](#troubleshooting)

---

## 🎯 Visão Geral

Este guia descreve como fazer o deployment automático da aplicação **FiapCloudGames** na AWS EC2 (Windows) usando GitHub Actions e Docker.

### Arquitetura de Deployment

```
GitHub Repository (stage branch)
         ↓
GitHub Actions (CI/CD Pipeline)
         ↓
    Build & Test
         ↓
    Docker Build & Push (Docker Hub)
         ↓
AWS Systems Manager (EC2 Command Execution)
         ↓
EC2 Windows Instance (Docker Container)
         ↓
FiapCloudGames API (Port 8080)
```

---

## 📦 Pré-requisitos

### 1. Conta AWS
- [x] EC2 Instance (Windows) criada
- [x] IAM User com credenciais (Access Key ID + Secret Access Key)
- [x] IAM Role com permissão `AmazonSSMManagedInstanceCore` (para Systems Manager)
- [x] Security Group com RDP (3389) aberto

### 2. Docker Hub
- [x] Conta Docker Hub criada
- [x] Personal Access Token gerado
- [x] Username: `jonathanornellas`

### 3. GitHub
- [x] Repositório com branch `stage`
- [x] Secrets configurados (veja próxima seção)

### 4. Instância Windows EC2
- [x] Docker Desktop instalado
- [x] PowerShell 5.0+ disponível
- [x] AWS Systems Manager Agent rodando (pré-instalado em AMIs recentes)

---

## ⚙️ Configuração AWS

### 1. Configurar IAM Role para EC2

A instância EC2 precisa de uma IAM Role para usar AWS Systems Manager.

**Passos:**

1. Vá para **AWS Console → IAM → Roles**
2. Clique em **Create role**
3. Selecione **AWS service** → **EC2**
4. Procure e selecione **AmazonSSMManagedInstanceCore**
5. Clique em **Create role**
6. Vá para **EC2 → Instances**
7. Clique na sua instância
8. **Instance State → Manage IAM role**
9. Selecione a role criada

### 2. Verificar Systems Manager Agent

**Na instância Windows:**

```powershell
# Verificar status do SSM Agent
Get-Service -Name AmazonSSMAgent

# Se não estiver rodando, iniciar:
Start-Service -Name AmazonSSMAgent

# Verificar logs
Get-Content "C:\ProgramData\Amazon\SSM\Logs\amazon-ssm-agent.log" -Tail 20
```

### 3. Verificar Conectividade

**Na AWS Console:**

1. Vá para **Systems Manager → Session Manager**
2. Clique em **Start session**
3. Selecione sua instância EC2
4. Clique em **Start session**

Se conectar com sucesso, tudo está configurado!

---

## 🔐 Configurar GitHub Secrets

A pipeline CI/CD precisa de secrets para acessar AWS e Docker Hub.

### Adicionar Secrets no GitHub

1. Vá para **Settings → Secrets and variables → Actions**
2. Clique em **New repository secret**
3. Adicione os seguintes secrets:

| Nome | Valor |
|------|-------|
| `AWS_ACCESS_KEY_ID` | Seu AWS Access Key ID |
| `AWS_SECRET_ACCESS_KEY` | Seu AWS Secret Access Key |
| `AWS_REGION` | `us-east-1` (ou sua região) |
| `EC2_INSTANCE_ID` | `i-0e4db4afd1231fce8c` |
| `DOCKERHUB_USERNAME` | `jonathanornellas` |
| `DOCKERHUB_TOKEN` | Seu Docker Hub Personal Access Token |

**Como gerar Docker Hub Token:**

1. Vá para [Docker Hub Settings → Security](https://hub.docker.com/settings/security)
2. Clique em **New Access Token**
3. Dê um nome (ex: "GitHub Actions")
4. Selecione **Read & Write**
5. Clique em **Generate**
6. Copie o token

---

## 🔄 Pipeline CI/CD

### Estrutura da Pipeline

O arquivo `.github/workflows/ci-cd-deploy.yml` contém 4 jobs:

#### 1️⃣ **Build & Test** (CI)
- Executa em: PRs e pushes
- Ações:
  - Checkout do código
  - Setup .NET 8
  - Restore de dependências
  - Build da aplicação
  - Execução de testes
  - Upload de resultados

#### 2️⃣ **Build & Push Docker** (CD)
- Executa em: Pushes na branch `stage`
- Ações:
  - Login no Docker Hub
  - Build da imagem Docker
  - Push para Docker Hub com tags:
    - `jonathanornellas/fiapcloudgames:latest`
    - `jonathanornellas/fiapcloudgames:<commit-sha>`

#### 3️⃣ **Deploy to AWS** (CD)
- Executa em: Pushes na branch `stage`
- Ações:
  - Configura credenciais AWS
  - Usa AWS Systems Manager para executar PowerShell na EC2
  - Script de deploy:
    - Para container anterior
    - Faz pull da nova imagem
    - Inicia novo container
    - Verifica saúde

#### 4️⃣ **Notify** (Notificação)
- Executa em: Sempre (mesmo se falhar)
- Ações:
  - Exibe resumo da pipeline
  - Status de cada job

### Disparadores da Pipeline

A pipeline é disparada automaticamente quando:

- ✅ Push na branch `stage`
- ✅ Push na branch `main`
- ✅ Push na branch `master`
- ✅ Pull Request para essas branches
- ✅ Manual via GitHub Actions UI

### Monitorar Execução

1. Vá para **GitHub → Actions**
2. Clique no workflow mais recente
3. Veja o status de cada job
4. Clique em um job para ver detalhes

---

## 🚀 Deploy Manual

Se preferir fazer deploy manualmente, use o script PowerShell fornecido.

### Opção 1: Executar via RDP

1. Conecte via RDP na instância Windows
2. Abra PowerShell como Administrator
3. Execute:

```powershell
# Navegar para o diretório do script
cd C:\path\to\scripts

# Executar o script de deploy
.\deploy-windows.ps1

# Ou com parâmetros customizados
.\deploy-windows.ps1 -HostPort 8080 -DeployPath "C:\inetpub\fiapGames"
```

### Opção 2: Executar via AWS Systems Manager

1. Vá para **AWS Console → Systems Manager → Session Manager**
2. Clique em **Start session**
3. Selecione sua instância
4. Execute:

```powershell
# Fazer pull da imagem
docker pull jonathanornellas/fiapcloudgames:latest

# Parar container anterior
docker stop fiapgames-app -ErrorAction SilentlyContinue
docker rm fiapgames-app -ErrorAction SilentlyContinue

# Iniciar novo container
docker run -d `
    --name fiapgames-app `
    -p 8080:80 `
    -e ASPNETCORE_ENVIRONMENT=Production `
    -e ASPNETCORE_URLS=http://+:80 `
    -v C:\inetpub\fiapGames:/app/data `
    --restart unless-stopped `
    jonathanornellas/fiapcloudgames:latest

# Verificar status
docker ps
```

### Parâmetros do Script

```powershell
.\deploy-windows.ps1 `
    -ImageName "jonathanornellas/fiapcloudgames:latest" `
    -ContainerName "fiapgames-app" `
    -HostPort 8080 `
    -ContainerPort 80 `
    -DeployPath "C:\inetpub\fiapGames"
```

---

## 📊 Monitoramento

### Usar Docker Compose com Monitoramento

O arquivo `docker-compose.monitoring.yml` inclui:

- **Prometheus** - Coleta de métricas
- **Grafana** - Visualização de métricas
- **Alertmanager** - Gerenciamento de alertas
- **Node Exporter** - Métricas do sistema
- **cAdvisor** - Métricas de containers

### Iniciar Stack de Monitoramento

```bash
# Na máquina local (desenvolvimento)
docker-compose -f docker-compose.monitoring.yml up -d
```

### Acessar Ferramentas

| Ferramenta | URL | Credenciais |
|-----------|-----|------------|
| Grafana | http://localhost:3000 | admin / admin123 |
| Prometheus | http://localhost:9090 | - |
| Alertmanager | http://localhost:9093 | - |
| API | http://localhost:8080/swagger | - |

### Dashboard Grafana

Um dashboard pré-configurado está disponível em:
- **Pasta:** FiapCloudGames
- **Nome:** FiapCloudGames - Monitoramento

**Métricas incluídas:**

- Taxa de requisições HTTP
- Uso de CPU
- Uso de memória
- Tráfego de rede
- Latência da aplicação (P95)
- Taxa de erros

### Regras de Alerta

Alertas configurados em `monitoring/alert_rules.yml`:

| Alerta | Condição | Severidade |
|--------|----------|-----------|
| HighCPUUsage | CPU > 80% por 5 min | ⚠️ Warning |
| CriticalCPUUsage | CPU > 95% por 2 min | 🔴 Critical |
| HighMemoryUsage | Memória > 80% por 5 min | ⚠️ Warning |
| CriticalMemoryUsage | Memória > 95% por 2 min | 🔴 Critical |
| HighDiskUsage | Disco > 80% por 5 min | ⚠️ Warning |
| CriticalDiskUsage | Disco > 95% por 2 min | 🔴 Critical |
| ContainerDown | Container não respondendo | 🔴 Critical |
| HighErrorRate | Taxa de erros 5xx > 5% | ⚠️ Warning |
| SlowResponseTime | P95 latência > 2s | ⚠️ Warning |

### Integrar com Slack (Opcional)

1. Crie um Incoming Webhook no Slack
2. Adicione a URL em `monitoring/alertmanager.yml`:

```yaml
receivers:
  - name: 'critical'
    slack_configs:
      - api_url: 'https://hooks.slack.com/services/YOUR/WEBHOOK/URL'
        channel: '#alerts'
```

---

## 🔧 Troubleshooting

### Pipeline Falha no Build

**Problema:** Job "Build & Test" falha

**Soluções:**

1. Verifique se o código está correto:
   ```bash
   dotnet build
   dotnet test
   ```

2. Verifique a versão do .NET:
   ```bash
   dotnet --version
   ```

3. Verifique dependências:
   ```bash
   dotnet restore
   ```

### Docker Push Falha

**Problema:** "Unauthorized" ao fazer push

**Soluções:**

1. Verifique credenciais no GitHub Secrets
2. Regenere o Docker Hub Token
3. Verifique permissões na conta Docker Hub

### Deploy Não Executa na EC2

**Problema:** Job "Deploy to AWS" falha

**Soluções:**

1. Verifique se a IAM Role está corretamente configurada:
   ```powershell
   # Na EC2, via Systems Manager
   Get-Service -Name AmazonSSMAgent
   ```

2. Verifique se o Systems Manager Agent está rodando:
   ```powershell
   Start-Service -Name AmazonSSMAgent
   ```

3. Verifique logs do Systems Manager:
   ```powershell
   Get-Content "C:\ProgramData\Amazon\SSM\Logs\amazon-ssm-agent.log" -Tail 50
   ```

4. Teste manualmente:
   ```bash
   aws ssm send-command \
     --instance-ids "i-0e4db4afd1231fce8c" \
     --document-name "AWS-RunPowerShellScript" \
     --parameters 'commands=["Get-Date"]' \
     --region us-east-1
   ```

### Container Não Inicia

**Problema:** Container para logo após iniciar

**Soluções:**

1. Verifique logs:
   ```powershell
   docker logs fiapgames-app
   ```

2. Verifique se a porta está disponível:
   ```powershell
   netstat -ano | findstr :8080
   ```

3. Verifique se a imagem existe:
   ```powershell
   docker images | findstr fiapcloudgames
   ```

4. Teste a imagem localmente:
   ```powershell
   docker run -it jonathanornellas/fiapcloudgames:latest
   ```

### Aplicação Não Responde

**Problema:** Acesso a http://localhost:8080/swagger retorna erro

**Soluções:**

1. Verifique se o container está rodando:
   ```powershell
   docker ps
   ```

2. Verifique logs:
   ```powershell
   docker logs -f fiapgames-app
   ```

3. Teste conectividade:
   ```powershell
   curl http://localhost:8080/swagger
   ```

4. Verifique firewall:
   ```powershell
   # Abrir porta 8080
   netsh advfirewall firewall add rule name="Allow 8080" dir=in action=allow protocol=tcp localport=8080
   ```

---

## 📚 Referências

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [AWS Systems Manager Documentation](https://docs.aws.amazon.com/systems-manager/)
- [Docker Documentation](https://docs.docker.com/)
- [Prometheus Documentation](https://prometheus.io/docs/)
- [Grafana Documentation](https://grafana.com/docs/)

---

## 📞 Suporte

Para problemas ou dúvidas:

1. Verifique os logs da pipeline no GitHub Actions
2. Verifique os logs da EC2 via Systems Manager
3. Verifique os logs do container: `docker logs fiapgames-app`
4. Consulte a documentação oficial das ferramentas

---

**Desenvolvido por:** Jonathan Ornellas  
**Última atualização:** Janeiro 2026
