# Payments API – Fiap Cloud Games

Um microsserviço de pagamentos assíncrono para a plataforma Fiap Cloud Games.
O serviço recebe eventos de pedidos realizados, cria o pagamento correspondente e expõe um endpoint para consulta do status do pagamento.

---

## 🚀 Visão geral

- `FiapCloudGamesPayments.Api`: API HTTP minimalista com endpoint REST e integração com mensageria.
- `FiapCloudGamesPayments.Application`: lógica de negócio, serviços, DTOs e tratamento de eventos de domínio.
- `FiapCloudGamesPayments.Domain`: modelo de domínio, entidades, agregados, eventos e exceções.
- `FiapCloudGamesPayments.Infra.Data`: persistência com Entity Framework Core e repositórios.
- `FiapCloudGamesPayments.Test`: testes unitários para o serviço de pagamento.
- `FiapCloudGames.Contracts`: contrato de eventos de integração compartilhado entre microserviços.

---

## 🔧 Funcionalidades principais

- Processamento de pagamentos a partir de eventos `OrderPlacedIntegrationEvent`.
- Persistência de `OrderPayment` no MySQL.
- Consulta de pagamento por `orderId` via endpoint REST.
- Autenticação JWT com verificação de roles `Admin` e `User`.
- Envio de mensagens para Azure Service Bus após criação de pagamento.
- Logs estruturados e tracing para Elasticsearch/Elastic APM.
- Tratamento global de exceções.

---

## 📦 Arquitetura de fluxo

1. Um evento `OrderPlacedIntegrationEvent` chega ao serviço via MassTransit + Azure Service Bus.
2. `ProcessPaymentConsumer` consome o evento e chama `OrderPaymentService.ProcessPayment`.
3. O serviço valida idempotência e grava o novo `OrderPayment` no banco.
4. A criação dispara um `OrderPaymentCreatedDomainEvent`.
5. `OrderPaymentCreatedEventHandler` publica uma mensagem na fila configurada do Azure Service Bus.
6. A API também permite consultar o pagamento pelo `orderId` com segurança baseada em token JWT.

---

## 📌 Endpoint disponível

### GET `/orders/{orderId}/payments`

- Retorna o pagamento associado ao pedido.
- Requer autenticação JWT.
- Roles permitidos: `Admin`, `User`.
- O serviço valida que o `User` só acesse seus próprios pagamentos;
  o `Admin` pode consultar qualquer pagamento.

Resposta de exemplo:

```json
{
  "id": "<payment-id>",
  "orderId": "<order-id>",
  "userId": "<user-id>",
  "status": "Processing",
  "price": 99.90,
  "dateCreated": "2026-05-20T12:34:56Z",
  "dateUpdated": null
}
```

---

## 🧩 Dependências externas

O serviço depende de:

- MySQL
- Azure Service Bus
- Elasticsearch / Elastic APM (opcional para observabilidade)

> Nota: o código atual usa Azure Service Bus via `AzureServiceBus:ConnectionString` e `AzureServiceBus:QueueName`, não RabbitMQ.

---

## ⚙️ Configuração

As configurações são carregadas a partir de `appsettings.json` e podem ser sobrescritas por variáveis de ambiente.
Use os nomes abaixo para configurar o ambiente:

- `ConnectionStrings__MySQL`
- `Authentication__Key`
- `Authentication__Issuer`
- `Authentication__Audience`
- `ElasticSearch__Uri`
- `ElasticSearch__IndexName`
- `ElasticSearch__ApiKey`
- `AzureServiceBus__ConnectionString`
- `AzureServiceBus__QueueName`
- `ElasticApm__ServerUrl`
- `ElasticApm__SecretToken`

---

## 🧪 Executando localmente

1. Garanta que MySQL, Azure Service Bus e Elasticsearch estejam acessíveis.
2. Configure as variáveis de ambiente ou atualize `appsettings.json` com os valores reais.
3. Execute a migração do banco de dados:

```powershell
dotnet run --project FiapCloudGamesPayments.Api -- migrate
```

4. Inicie a API:

```powershell
dotnet run --project FiapCloudGamesPayments.Api
```

5. No ambiente de desenvolvimento, o Swagger estará disponível em:

```
http://localhost:5000/swagger
```

---

## 🐳 Docker

O projeto inclui um `Dockerfile` para criar a imagem do serviço.

```powershell
docker build -f FiapCloudGamesPayments.Api/Dockerfile -t fiap-payments-api .
docker run -p 8080:8080 --env-file .env fiap-payments-api
```

---

## 🧠 Observabilidade

- `Serilog` grava logs no console e no Elasticsearch.
- `Elastic APM` é configurado em `ElasticApm` para capturar tracing e corpo de requisições.
- Filtros de MassTransit (`TracingSendFilter`, `TracingPublishFilter`, `TracingConsumeFilter`) enriquecem o rastreamento de mensagens.

---

## ✅ Testes

Execute a suíte de testes unitários com:

```powershell
dotnet test FiapCloudGamesPayments.Test/FiapCloudGamesPayments.Test.csproj
```

---

## 📁 Estrutura de pastas

- `FiapCloudGamesPayments.Api`: Web API, endpoints, consumidores, autenticação e configurações.
- `FiapCloudGamesPayments.Application`: casos de uso, serviços, DTOs, eventos e middlewares.
- `FiapCloudGamesPayments.Domain`: entidades, agregados, eventos de domínio e exceções.
- `FiapCloudGamesPayments.Infra.Data`: EF Core, contexto de banco e repositórios.
- `FiapCloudGamesPayments.Test`: testes unitários.
- `FiapCloudGames.Contracts`: contratos de eventos de integração usados pelo consumidor.

---

## 💡 Observações

- A validação de idempotência bloqueia pagamentos duplicados para o mesmo pedido.
- A integração com eventos usa `OrderPlacedIntegrationEvent` com `orderId`, `userId`, `price` e `gameIds`.
- Ao criar `OrderPayment`, o serviço publica um evento de pagamento na fila configurada.

Se quiser, posso também adicionar um exemplo de `docker-compose.yml` para rodar a solução localmente com dependências.  
---
