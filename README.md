# Company Sales | DDD + Clean Architecture

Projeto de demonstração técnica para evidenciar modelagem de domínio rica com **DDD (Domain-Driven Design)** e organização orientada a **Clean Architecture**.

## Objetivo

Demonstrar construção de um domínio com foco em:

- invariantes e regras de negócio explícitas
- entidades e value objects com responsabilidades claras
- eventos de domínio para sinalizar mudanças relevantes
- testes automatizados como rede de segurança para evolução

## Status atual

A camada **Domain** está concluída para esta demonstração, organizada em **3 bounded contexts**:

- `Catalog`
- `Customers`
- `Orders`

## Estrutura da solução

```text
Company.Sales.slnx
src/
	Company.Sales.Domain/
		Common/
		Catalog/
		Customers/
		Orders/
test/
	Company.Sales.Domain.Test/
```

## Diagrama de classes (Domain)

- Arquivo Mermaid: [docs/diagrams/domain-class-diagram.mmd](docs/diagrams/domain-class-diagram.mmd)

## Core compartilhado (`Common`)

- `Entity`: identidade, auditoria (`CreatedAt`, `UpdatedAt`) e controle de Domain Events
- `AggregateRoot`: base para agregados
- `ValueObject`: igualdade por componentes
- `DomainEventBase` + `IDomainEvent`: contrato e implementação base para eventos
- `DomainException`: exceção de regra de negócio
- `Guard`: validações reutilizáveis para proteção de invariantes

## Bounded Contexts

### Catalog

- Entidades: `Category`, `Product`
- Value Objects: `ProductName`, `ProductCode`, `ProductPrice`, `ProductImage`
- Enum: `ProductStatus`
- Eventos:
	- `CategoryActivatedEvent`
	- `CategoryDeactivatedEvent`
	- `ProductActivatedEvent`
	- `ProductDeactivatedEvent`
	- `ProductPriceChangedEvent`
	- `StockAdjustedEvent`
	- `ImageAddedEvent`

### Customers

- Entidades: `Customer`, `Address`
- Value Objects: `FullName`, `Cpf`, `Email`, `Phone`
- Enums: `CustomerStatus`, `Gender`, `MaritalStatus`
- Eventos:
	- `CustomerRegisteredEvent`
	- `CustomerBlockedEvent`
	- `PrimaryAddressChangedEvent`

### Orders

- Entidades: `Order`, `OrderItem`, `Payment`
- Value Objects: `DeliveryAddress`, `CancellationReason`
- Enums: `OrderStatus`, `PaymentMethod`, `PaymentStatus`
- Eventos:
	- `OrderShippedEvent`
	- `OrderDeliveredEvent`
	- `OrderCancelledEvent`
	- `PaymentApprovedEvent`
	- `PaymentRejectedEvent`

## Testes automatizados

Cobertura atual da camada de domínio com testes por contexto:

- `Catalog`: `CategoryTest`, `ProductTest`
- `Customers`: `AddressTest`, `CustomerTests`
- `Orders`: `OrderTest`, `OrderItemTest`, `PaymentTest`, `DeliveryAddressTest`

Ultima execucao (`dotnet test` em 2026-03-06):

- Total: `121`
- Sucesso: `121`
- Falhas: `0`
- Ignorados: `0`

## Como executar

```bash
dotnet build
dotnet test
```

## Próximos passos

- iniciar camada de `Application` com casos de uso
- projetar contratos da camada `Infrastructure` (persistência e integrações)
- evoluir testes para cenários de integração entre camadas
