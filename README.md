
## Como rodar o servidor na AWS (Ubuntu)

### 1. Configuração da instância
- Crie uma instância EC2 Ubuntu.
- No grupo de segurança, libere a porta TCP 5000 para entrada.

### 2. Instalação de dependências
Conecte via terminal SSH e execute:

```
dotnet run --project Servidor/Servidor/Server.csproj
```

O servidor ficará aguardando conexões na porta 5000.

### 3. Clone o projeto

```

### 3. Execução do Cliente
Execute o cliente:


### 4. Publicação e execução

```
```
dotnet run --project Cliente/Cliente/Client.csproj
```

Siga as instruções no terminal para informar o IP do servidor e seu nome de usuário.

O servidor ficará aguardando conexões na porta 5000.

---

## Como rodar o cliente

Compile e execute o cliente localmente (Windows/Linux):

```

### linux

Siga as instruções no terminal para informar o IP público da instância AWS e seu nome de usuário.
dotnet publish -c Release -r linux-x64 --self-contained

## Comandos do Cliente

- **Mensagem privada:**
	```
	/privado <usuario> <mensagem>
	```
	Exemplo: `/privado robert Olá!`

- **Criar grupo:**
	```
	/criargrupo <nomegrupo> <membro1,membro2,...>
	```
	Exemplo: `/criargrupo turma2025 robert,gabriel`

- **Mensagem para grupo:**
	```
	/grupo <nomegrupo> <mensagem>
	```
	Exemplo: `/grupo turma2025 Bom dia, grupo!`

- **Sair:**
	```
	/sair
	```

## Observações

- O usuário que cria um grupo é automaticamente incluído nele.
- Apenas usuários conectados recebem mensagens.
- O sistema não implementa autenticação forte nem criptografia.


## Estrutura do Projeto

```
client-server/
├── Cliente/
│   └── Cliente/
│       ├── Program.cs
│       ├── Service/
│       │   └── ClientService.cs
│       └── Interface/
│           └── IClientService.cs
├── Servidor/
│   └── Servidor/
│       ├── Program.cs
│       ├── Service/
│       │   └── ChatHandlerService.cs
│       └── Interface/
│           └── IChatHandlerService.cs
└── README.md
```

## Requisitos
- .NET 8.0 ou superior

---
Desenvolvido para fins didáticos.
