# 📊 Guia de Monitoramento - FiapCloudGames

## 🎯 Visão Geral

Este guia descreve como usar a stack de monitoramento (Prometheus + Grafana + Alertmanager) para monitorar a aplicação FiapCloudGames.

---

## 🏗️ Arquitetura de Monitoramento

```
┌─────────────────────────────────────────────────────────────┐
│                    FiapCloudGames API                       │
│                    (Port 8080)                              │
└────────────────────┬────────────────────────────────────────┘
                     │
        ┌────────────┼────────────┐
        ▼            ▼            ▼
    ┌────────┐  ┌──────────┐  ┌─────────────┐
    │ Metrics│  │Node      │  │cAdvisor     │
    │Endpoint│  │Exporter  │  │(Containers) │
    └────────┘  └──────────┘  └─────────────┘
        │            │            │
        └────────────┼────────────┘
                     │
                     ▼
            ┌─────────────────┐
            │  Prometheus     │
            │  (Coleta)       │
            │  Port: 9090     │
            └────────┬────────┘
                     │
        ┌────────────┼────────────┐
        ▼            ▼            ▼
    ┌────────┐  ┌──────────┐  ┌─────────────┐
    │Grafana │  │Alert     │  │Rules        │
    │(Visual)│  │Manager   │  │(Evaluation) │
    │Port:   │  │Port:9093 │  │             │
    │3000    │  └──────────┘  └─────────────┘
    └────────┘
```

---

## 🚀 Iniciar Stack de Monitoramento

### Opção 1: Docker Compose (Recomendado)

```bash
# Navegar para o diretório do projeto
cd /path/to/FiapCloudGames

# Iniciar a stack completa
docker-compose -f docker-compose.monitoring.yml up -d

# Verificar status
docker-compose -f docker-compose.monitoring.yml ps

# Ver logs
docker-compose -f docker-compose.monitoring.yml logs -f
```

### Opção 2: Iniciar Serviços Individuais

```bash
# Prometheus
docker run -d \
  --name prometheus \
  -p 9090:9090 \
  -v $(pwd)/monitoring/prometheus.yml:/etc/prometheus/prometheus.yml \
  prom/prometheus

# Grafana
docker run -d \
  --name grafana \
  -p 3000:3000 \
  -e GF_SECURITY_ADMIN_PASSWORD=admin123 \
  grafana/grafana

# Alertmanager
docker run -d \
  --name alertmanager \
  -p 9093:9093 \
  -v $(pwd)/monitoring/alertmanager.yml:/etc/alertmanager/alertmanager.yml \
  prom/alertmanager
```

---

## 🌐 Acessar Ferramentas

### Prometheus

**URL:** http://localhost:9090

**Funcionalidades:**
- Visualizar métricas coletadas
- Executar queries PromQL
- Ver status dos targets
- Verificar regras de alerta

**Exemplo de Query:**
```promql
# Taxa de requisições por segundo
rate(http_requests_total[5m])

# Uso de CPU
100 - (avg(rate(node_cpu_seconds_total{mode="idle"}[5m])) * 100)

# Uso de memória
(1 - (node_memory_MemAvailable_bytes / node_memory_MemTotal_bytes)) * 100
```

### Grafana

**URL:** http://localhost:3000

**Credenciais padrão:**
- Username: `admin`
- Password: `admin123`

**Dashboards pré-configurados:**
- FiapCloudGames - Monitoramento (com 6 painéis)

**Painéis inclusos:**
1. Taxa de Requisições HTTP
2. Uso de CPU
3. Uso de Memória
4. Tráfego de Rede
5. Latência da Aplicação (P95)
6. Taxa de Erros

### Alertmanager

**URL:** http://localhost:9093

**Funcionalidades:**
- Ver alertas ativos
- Silenciar alertas
- Gerenciar grupos de alertas
- Configurar integrações (Slack, Email, etc)

---

## 📈 Métricas Coletadas

### Métricas da Aplicação

| Métrica | Descrição | Tipo |
|---------|-----------|------|
| `http_requests_total` | Total de requisições HTTP | Counter |
| `http_request_duration_seconds` | Duração das requisições | Histogram |
| `http_requests_total{status}` | Requisições por status | Counter |

### Métricas do Sistema (Node Exporter)

| Métrica | Descrição |
|---------|-----------|
| `node_cpu_seconds_total` | Tempo de CPU por modo |
| `node_memory_MemTotal_bytes` | Memória total |
| `node_memory_MemAvailable_bytes` | Memória disponível |
| `node_filesystem_size_bytes` | Tamanho do filesystem |
| `node_filesystem_avail_bytes` | Espaço disponível |
| `node_network_receive_bytes_total` | Bytes recebidos |
| `node_network_transmit_bytes_total` | Bytes transmitidos |

### Métricas de Container (cAdvisor)

| Métrica | Descrição |
|---------|-----------|
| `container_memory_usage_bytes` | Memória usada pelo container |
| `container_cpu_usage_seconds_total` | CPU usada pelo container |
| `container_network_receive_bytes_total` | Bytes recebidos |
| `container_network_transmit_bytes_total` | Bytes transmitidos |

---

## 🚨 Regras de Alerta

### Alertas Configurados

#### CPU

| Alerta | Condição | Severidade | Ação |
|--------|----------|-----------|------|
| HighCPUUsage | CPU > 80% por 5 min | ⚠️ Warning | Investigar processos |
| CriticalCPUUsage | CPU > 95% por 2 min | 🔴 Critical | Escalar imediatamente |

#### Memória

| Alerta | Condição | Severidade | Ação |
|--------|----------|-----------|------|
| HighMemoryUsage | Memória > 80% por 5 min | ⚠️ Warning | Verificar vazamentos |
| CriticalMemoryUsage | Memória > 95% por 2 min | 🔴 Critical | Reiniciar serviço |

#### Disco

| Alerta | Condição | Severidade | Ação |
|--------|----------|-----------|------|
| HighDiskUsage | Disco > 80% por 5 min | ⚠️ Warning | Limpar espaço |
| CriticalDiskUsage | Disco > 95% por 2 min | 🔴 Critical | Adicionar espaço urgente |

#### Container

| Alerta | Condição | Severidade | Ação |
|--------|----------|-----------|------|
| ContainerDown | Container não respondendo | 🔴 Critical | Reiniciar container |
| HighContainerMemory | Memória > 80% alocada | ⚠️ Warning | Aumentar limite |

#### Aplicação

| Alerta | Condição | Severidade | Ação |
|--------|----------|-----------|------|
| HighErrorRate | Taxa de erros 5xx > 5% | ⚠️ Warning | Verificar logs |
| SlowResponseTime | P95 latência > 2s | ⚠️ Warning | Otimizar queries |

---

## 🔔 Integrar com Slack

### 1. Criar Webhook no Slack

1. Vá para [Slack API](https://api.slack.com/apps)
2. Clique em **Create New App**
3. Selecione **From scratch**
4. Dê um nome (ex: "AlertManager")
5. Selecione seu workspace
6. Vá para **Incoming Webhooks**
7. Ative **Incoming Webhooks**
8. Clique em **Add New Webhook to Workspace**
9. Selecione o canal (ex: #alerts)
10. Copie a URL do webhook

### 2. Configurar Alertmanager

Edite `monitoring/alertmanager.yml`:

```yaml
global:
  slack_api_url: 'https://hooks.slack.com/services/YOUR/WEBHOOK/URL'

route:
  receiver: 'slack'

receivers:
  - name: 'slack'
    slack_configs:
      - channel: '#alerts'
        title: 'Alerta: {{ .GroupLabels.alertname }}'
        text: '{{ range .Alerts }}{{ .Annotations.description }}{{ end }}'
        send_resolved: true
```

### 3. Reiniciar Alertmanager

```bash
docker-compose -f docker-compose.monitoring.yml restart alertmanager
```

---

## 📊 Criar Dashboard Customizado

### No Grafana

1. Clique em **Dashboards** → **New Dashboard**
2. Clique em **Add a new panel**
3. Selecione **Prometheus** como datasource
4. Escreva uma query PromQL
5. Configure visualização
6. Salve o dashboard

### Exemplo: Dashboard de Performance

```promql
# Query 1: Taxa de requisições
rate(http_requests_total[5m])

# Query 2: Latência P95
histogram_quantile(0.95, rate(http_request_duration_seconds_bucket[5m]))

# Query 3: Taxa de erros
rate(http_requests_total{status=~"5.."}[5m])

# Query 4: Uso de CPU
100 - (avg(rate(node_cpu_seconds_total{mode="idle"}[5m])) * 100)
```

---

## 🔍 Troubleshooting

### Prometheus não coleta métricas

**Problema:** Nenhuma métrica aparece no Prometheus

**Soluções:**

1. Verifique se os targets estão up:
   - Acesse http://localhost:9090/targets
   - Verifique status de cada target

2. Verifique configuração do Prometheus:
   ```bash
   docker logs prometheus
   ```

3. Verifique se a aplicação expõe métricas:
   ```bash
   curl http://localhost:8080/metrics
   ```

### Alertas não disparam

**Problema:** Alertas não são acionados mesmo com condições atendidas

**Soluções:**

1. Verifique regras de alerta:
   - Acesse http://localhost:9090/alerts
   - Verifique status das regras

2. Verifique sintaxe do YAML:
   ```bash
   docker logs prometheus
   ```

3. Teste a query manualmente:
   - Vá para http://localhost:9090
   - Execute a query do alerta

### Grafana não conecta ao Prometheus

**Problema:** "No data" nos painéis

**Soluções:**

1. Verifique datasource:
   - **Configuration** → **Data Sources**
   - Clique em **Prometheus**
   - Verifique URL: `http://prometheus:9090`
   - Clique em **Test**

2. Reinicie Grafana:
   ```bash
   docker-compose -f docker-compose.monitoring.yml restart grafana
   ```

---

## 📈 Melhores Práticas

### 1. Monitoramento Proativo

- Defina alertas para métricas críticas
- Configure notificações em tempo real
- Revise alertas regularmente

### 2. Retenção de Dados

Configure retenção no Prometheus (padrão: 15 dias):

```yaml
# prometheus.yml
global:
  scrape_interval: 15s
  retention: 30d  # 30 dias
```

### 3. Escalabilidade

Para ambientes de produção:

- Use Prometheus com storage remoto
- Implemente Prometheus Federation
- Use Thanos para retenção de longo prazo

### 4. Segurança

- Proteja Grafana com senha forte
- Use HTTPS em produção
- Restrinja acesso ao Prometheus

---

## 📚 Referências

- [Prometheus Documentation](https://prometheus.io/docs/)
- [Grafana Documentation](https://grafana.com/docs/)
- [Alertmanager Documentation](https://prometheus.io/docs/alerting/latest/overview/)
- [PromQL Query Language](https://prometheus.io/docs/prometheus/latest/querying/basics/)

---

**Desenvolvido por:** Jonathan Ornellas  
**Última atualização:** Janeiro 2026
