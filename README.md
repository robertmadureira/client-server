 
# Simple Client-Server Chat with Groups

Este projeto implementa um sistema de chat simples em C# utilizando sockets TCP, com suporte a mensagens privadas e grupos.

## Funcionalidades

- **Conexão Cliente-Servidor**: O servidor gerencia conexões simultâneas de múltiplos clientes.
- **Autenticação Simples**: O usuário informa um nome de usuário ao conectar.
- **Mensagens Privadas**: Envie mensagens diretamente para outro usuário conectado.
- **Mensagens em Grupo**: Crie grupos e envie mensagens para todos os membros do grupo.
- **Concorrência**: O servidor utiliza async/await para gerenciar múltiplas conexões de forma eficiente.

## Como Usar

### 1. Compilação
Compile os projetos `Servidor` e `Cliente` separadamente usando o Visual Studio ou o comando `dotnet build`.

### 2. Execução do Servidor
Execute o servidor:

```
dotnet run --project Servidor/Servidor/Server.csproj
```

O servidor ficará aguardando conexões na porta 5000.

### 3. Execução do Cliente
Execute o cliente:

```
dotnet run --project Cliente/Cliente/Client.csproj
```

Siga as instruções no terminal para informar o IP do servidor e seu nome de usuário.

### linux
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
│       └── Client.cs
├── Servidor/
│   └── Servidor/
│       └── Server.cs
└── README.md
```

## Requisitos
- .NET 8.0 ou superior

---
Desenvolvido para fins didáticos.
