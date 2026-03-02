# Company Sales | DDD + Clean Architecture

Projeto de portfólio para demonstrar modelagem de domínio, aplicação de regras de negócio e organização arquitetural com foco em **DDD (Domain-Driven Design)** e **Clean Architecture**.

## 🎯 Objetivo do projeto

Evidenciar minha abordagem de engenharia de software para construção de domínios ricos, com:

- regras explícitas e protegidas por invariantes
- entidades e value objects com responsabilidades claras
- eventos de domínio para mudanças relevantes de estado
- testes automatizados para segurança de evolução

## 🧱 Escopo atual

Atualmente o foco está na camada **Domain**, já com implementação funcional e coberta por testes.

### Core do domínio

- `Entity` com identidade, auditoria (`CreatedAt`, `UpdatedAt`) e suporte a Domain Events
- `ValueObject` com igualdade por componentes
- `DomainException` para violações de negócio
- `Guard` para validações reutilizáveis e invariantes

### Tipos de domínio (Enums)

- `OrderStatus`
- `PaymentMethod`
- `PaymentStatus`
- `Gender`
- `MaritalStatus`

### Entidades implementadas

#### `OrderItem`

- validações na criação
- cálculo de total do item
- aplicação de desconto com limite máximo
- adição/remoção de unidades com proteção contra estado inválido
- atualização de preço unitário

#### `Payment`

- inicialização com status `Pending`
- validação de método e valor
- geração local de código transacional (`GenerateLocalTransactionCode`)
- confirmação e rejeição com transição de estado controlada
- emissão de eventos em aprovação/rejeição

### Value Object implementado

#### `DeliveryAddress`

- factory method (`Create`)
- validação de obrigatoriedade dos campos
- validação de CEP no padrão `12345-678`
- igualdade por valor
- formatação amigável para exibição (`FormatAddress`)

### Eventos de domínio

- `PaymentApprovedEvent`
- `PaymentRejectedEvent`

## ✅ Qualidade e testes

Suíte de testes de domínio com:

- `OrderItemTest`
- `PaymentTest`
- `DeliveryAddressTest`

Resultado atual da execução:

- **33 testes passando**
- **0 falhas**

## ▶️ Como executar

```bash
dotnet test
```

## 🗺️ Próximos passos

- evoluir agregados e regras de consistência entre entidades
- iniciar camada de Application (casos de uso)
- adicionar infraestrutura de persistência e integrações
- ampliar testes para cenários de integração entre camadas
