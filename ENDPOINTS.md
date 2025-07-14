# Documentação da API - Sistema de Ordem de Serviço

Este documento descreve todos os endpoints disponíveis na API, como utilizá-los, os dados necessários e os retornos esperados.

**URL Base da API:** `http://localhost:3000` (em desenvolvimento)

**Autenticação:** A maioria dos endpoints requer um token JWT, que deve ser enviado no cabeçalho da requisição da seguinte forma:
`Authorization: Bearer <seu_token_jwt>`

---
a
## 1. Autenticação

### 1.1. Login de Usuário

*   **Endpoint:** `POST /api/auth/login`
*   **Descrição:** Autentica um usuário com email e senha.
*   **Autenticação:** Não requer.
*   **Corpo da Requisição (JSON):**
    ```json
    {
      "email": "admin@example.com",
      "password": "password123"
    }
    ```
*   **Resposta de Sucesso (200 OK):**
    ```json
    {
      "token": "ey...",
      "user": {
        "id": "...",
        "name": "Administrador",
        "email": "admin@example.com",
        "roleId": "...",
        "roleName": "Admin",
        "isActive": true
      }
    }
    ```
*   **Resposta de Erro (401 Unauthorized):**
    *   Credenciais inválidas.

---

## 2. Usuários (`/api/usuarios`)

*   **Autenticação:** Todos os endpoints de usuários requerem autenticação.

### 2.1. Listar Usuários
*   **Endpoint:** `GET /api/usuarios`

### 2.2. Obter Usuário por ID
*   **Endpoint:** `GET /api/usuarios/{id}`

### 2.3. Criar Usuário
*   **Endpoint:** `POST /api/usuarios`
*   **Corpo da Requisição (JSON):**
    ```json
    {
      "name": "Novo Usuário",
      "email": "user@example.com",
      "password": "password123",
      "roleId": "guid-do-cargo",
      "isActive": true
    }
    ```

### 2.4. Atualizar Usuário
*   **Endpoint:** `PUT /api/usuarios/{id}`
*   **Corpo da Requisição (JSON) (envie apenas os campos a serem alterados):**
    ```json
    {
      "name": "Nome Atualizado",
      "email": "email.atualizado@example.com",
      "password": "novaSenhaSegura",
      "roleId": "guid-do-novo-cargo",
      "isActive": false
    }
    ```

### 2.5. Deletar Usuário
*   **Endpoint:** `DELETE /api/usuarios/{id}`

---

## 3. Clientes (`/api/clientes`)

*   **Autenticação:** Todos os endpoints de clientes requerem autenticação.

### 3.1. Listar Clientes
*   **Endpoint:** `GET /api/clientes`

### 3.2. Obter Cliente por ID
*   **Endpoint:** `GET /api/clientes/{id}`

### 3.3. Criar Cliente
*   **Endpoint:** `POST /api/clientes`
*   **Corpo da Requisição (JSON):**
    ```json
    {
      "name": "Nome do Cliente",
      "email": "cliente@email.com",
      "phone": "11999998888",
      "document": "123.456.789-00"
    }
    ```

### 3.4. Atualizar Cliente
*   **Endpoint:** `PUT /api/clientes/{id}`
*   **Corpo da Requisição (JSON):**
    ```json
    {
      "name": "Nome Atualizado do Cliente",
      "email": "clienteatualizado@email.com",
      "phone": "11988887777",
      "document": "123.456.789-00"
    }
    ```

### 3.5. Deletar Cliente
*   **Endpoint:** `DELETE /api/clientes/{id}`

---

## 4. Ordens de Serviço (`/api/os`)

*   **Autenticação:** Todos os endpoints de OS requerem autenticação.

### 4.1. Listar Ordens de Serviço
*   **Endpoint:** `GET /api/os`
*   **Filtros (Query Params):**
    *   `statusId` (guid)
    *   `clientId` (guid)
    *   `startDate` (formato: `YYYY-MM-DD`)
    *   `endDate` (formato: `YYYY-MM-DD`)
*   **Exemplo:** `GET /api/os?statusId=guid-do-status&startDate=2024-01-01`

### 4.2. Obter Ordem de Serviço por ID
*   **Endpoint:** `GET /api/os/{id}`

### 4.3. Criar Ordem de Serviço
*   **Endpoint:** `POST /api/os`
*   **Corpo da Requisição (JSON):**
    ```json
    {
      "clientId": "guid-do-cliente",
      "equipment": "Notebook",
      "brand": "MarcaX",
      "model": "ModeloY",
      "serialNumber": "SN12345",
      "problemDescription": "Não liga."
    }
    ```

### 4.4. Atualizar Ordem de Serviço
*   **Endpoint:** `PUT /api/os/{id}`
*   **Corpo da Requisição (JSON):**
    ```json
    {
      "equipment": "Notebook",
      "brand": "MarcaX",
      "model": "ModeloZ",
      "serialNumber": "SN123456",
      "problemDescription": "Não liga e a tela está quebrada."
    }
    ```

### 4.5. Atualizar Status da OS
*   **Endpoint:** `PATCH /api/os/{id}/status`
*   **Corpo da Requisição (JSON):**
    ```json
    {
      "statusId": "guid-do-novo-status",
      "notes": "Cliente notificado sobre o orçamento."
    }
    ```

### 4.6. Preencher Solução Técnica da OS
*   **Endpoint:** `PATCH /api/os/{id}/solution`
*   **Corpo da Requisição (JSON):**
    ```json
    {
      "technicalSolution": "A fonte de alimentação foi trocada."
    }
    ```

---

## 5. Status (`/api/status`)

*   **Autenticação:** Todos os endpoints de status requerem autenticação.

### 5.1. Listar Status
*   **Endpoint:** `GET /api/status`

### 5.2. Criar Status
*   **Endpoint:** `POST /api/status`
*   **Corpo da Requisição (JSON):**
    ```json
    {
      "name": "Aguardando Aprovação",
      "color": "#FFC107",
      "icon": "icon-timer",
      "triggersEmail": true
    }
    ```

### 5.3. Atualizar Status
*   **Endpoint:** `PUT /api/status/{id}`
*   **Corpo da Requisição (JSON):**
    ```json
    {
      "name": "Aguardando Peças",
      "color": "#00BCD4"
    }
    ```

### 5.4. Deletar Status
*   **Endpoint:** `DELETE /api/status/{id}`
*   **Observação:** Retornará erro (409 Conflict) se o status estiver em uso por alguma Ordem de Serviço.

---

## 6. Cargos (`/api/cargos`)

*   **Autenticação:** Todos os endpoints de cargos requerem autenticação.

### 6.1. Listar Cargos
*   **Endpoint:** `GET /api/cargos`

### 6.2. Criar Cargo
*   **Endpoint:** `POST /api/cargos`
*   **Corpo da Requisição (JSON):**
    ```json
    {
      "name": "Técnico",
      "permissions": "{ "os": ["create", "read", "update"] }"
    }
    ```

---

## 7. Serviços (`/api/servicos`)

*   **Autenticação:** Todos os endpoints de serviços requerem autenticação.

### 7.1. Listar Serviços
*   **Endpoint:** `GET /api/servicos`

### 7.2. Criar Serviço
*   **Endpoint:** `POST /api/servicos`
*   **Corpo da Requisição (JSON):**
    ```json
    {
      "name": "Troca de Tela",
      "description": "Substituição da tela de notebook.",
      "defaultPrice": 450.00
    }
    ```
