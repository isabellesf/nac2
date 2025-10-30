# Sistema de Gerenciamento de Estoque (Adaptado)

Este projeto foi adaptado do repositório original de "gerenciamento de veículos" para implementar um sistema de **Gerenciamento de Estoque** em C# e ASP.NET Core, conforme os requisitos de modelagem, regras de negócio e validações.

## 1. Modelagem de Entidades (Etapa 1)

O projeto utiliza o **Entity Framework Core** com o provedor **In-Memory** (equivalente ao H2 em Java) para a persistência.

### Diagrama de Entidades (Representação Textual)

**Produto**
- `CodigoSKU` (PK, Chave Primária)
- `Nome`
- `Categoria` (PERECIVEL/NAO_PERECIVEL)
- `PrecoUnitario`
- `QuantidadeMinimaEstoque`
- `QuantidadeEmEstoque` (Saldo Atual)

**Movimentação de Estoque**
- `Id` (PK, Chave Primária)
- `Tipo` (ENTRADA/SAIDA)
- `Quantidade`
- `DataMovimentacao`
- `Lote` (Para Perecíveis)
- `DataValidade` (Para Perecíveis)
- `ProdutoCodigoSKU` (FK, Chave Estrangeira para Produto)

**Relação:** Um Produto pode ter muitas Movimentações de Estoque (1:N).

### Regras de Negócio Implementadas (Etapas 2 e 3)

| Serviço | Regra de Negócio | Status |
| :--- | :--- | :--- |
| **Produto** | Cadastro completo de produtos (SKU é chave única). | **Implementado** (`ValidacaoException` se SKU duplicado). |
| **Produto** | Alerta de produtos abaixo do estoque mínimo. | **Implementado** (Método `ListarProdutosAbaixoEstoqueMinimoAsync`). |
| **Movimentação** | Não é permitido entrada/saída de quantidade negativa. | **Implementado** (`ValidacaoException`). |
| **Movimentação** | Produtos perecíveis devem ter lote e data de validade (na entrada). | **Implementado** (`ValidacaoException`). |
| **Movimentação** | Verificar estoque suficiente para saídas. | **Implementado** (`EstoqueInsuficienteException`). |
| **Movimentação** | Atualizar saldo do produto automaticamente. | **Implementado** (Saldo atualizado no `MovimentacaoService`). |
| **Movimentação** | Produto perecível não pode ter movimentação após data de validade (na saída). | **Implementado** (`ValidacaoException`). |
| **Relatórios** | Calcular valor total do estoque (`quantidade * preço`). | **Implementado**. |
| **Relatórios** | Listar produtos que vencerão em até 7 dias. | **Implementado**. |

## 2. Como Executar o Projeto

O projeto utiliza o SDK do .NET 8.0.

### Pré-requisitos

*   SDK do .NET 8.0
*   Visual Studio 2022 ou Visual Studio Code

### Passos

1.  **Restaurar Dependências e Adicionar Projetos à Solução:**
    ```bash
    # Na pasta raiz do projeto (onde está o slnPerformance.sln)
    dotnet restore
    dotnet sln add Persistence/Persistence.csproj
    ```

2.  **Executar a Aplicação:**
    ```bash
    # Navegue para a pasta do projeto principal
    cd performance-cache
    
    # Execute a aplicação
    dotnet run
    ```
    A aplicação será iniciada e o Swagger estará disponível em `https://localhost:7001/swagger` (a porta pode variar).

## 3. Exemplos de Requisições API

A API está exposta nos Controllers `Produto`, `Movimentacao` e `Relatorio`.

### 1. Cadastrar Produto (POST /api/Produto)

**Produto Não Perecível:**
```json
{
  "codigoSKU": "P001",
  "nome": "Caderno Espiral",
  "categoria": 1, 
  "precoUnitario": 15.50,
  "quantidadeMinimaEstoque": 5
}
```

**Produto Perecível:**
```json
{
  "codigoSKU": "P002",
  "nome": "Iogurte Natural",
  "categoria": 0, 
  "precoUnitario": 3.00,
  "quantidadeMinimaEstoque": 10
}
```

### 2. Registrar Entrada (POST /api/Movimentacao/entrada/{sku})

**Entrada de Perecível (SKU: P002):**
```json
{
  "quantidade": 100,
  "lote": "LOTE-20250520",
  "dataValidade": "2025-05-20T00:00:00"
}
```

**Exemplo de Erro (Entrada sem Lote/Validade para Perecível):**
```json
{
  "quantidade": 10,
  "lote": null,
  "dataValidade": null
}
// Retorna 400 Bad Request: "Produtos perecíveis requerem Lote e Data de Validade."
```

### 3. Registrar Saída (POST /api/Movimentacao/saida/{sku})

**Saída de Não Perecível (SKU: P001):**
```json
{
  "quantidade": 2
}
```

**Exemplo de Erro (Estoque Insuficiente):**
```json
{
  "quantidade": 500
}
// Retorna 400 Bad Request: "Estoque insuficiente para o produto P001. Disponível: X, Solicitado: 500."
```

### 4. Relatórios (GET /api/Relatorio)

*   **Valor Total do Estoque:** `GET /api/Relatorio/valor-total-estoque`
*   **Produtos Abaixo do Mínimo:** `GET /api/Relatorio/abaixo-minimo`
*   **Produtos Vencendo em 7 Dias:** `GET /api/Relatorio/vencendo-7-dias`
