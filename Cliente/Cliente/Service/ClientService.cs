using Cliente.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cliente.Service
{
    internal class ClientService : IClientService
    {
        public async Task RunChatClientAsync()
        {
            Console.WriteLine("--- Cliente de Chat ---");
            Console.Write("Digite o IP do Servidor: ");
            string ipServidor = Console.ReadLine();

            string nome;
            while (true)
            {
                Console.Write("Seu nome de usuário: ");
                nome = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(nome)) break;
                Console.WriteLine("Nome de usuário não pode ser vazio.");
            }

            var chatService = new ChatClientService();
            try
            {
                await chatService.ConnectAsync(ipServidor, nome);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao conectar: {ex.Message}");
                return;
            }

            chatService.Listen();

            Console.WriteLine("\nComandos:");
            Console.WriteLine("/privado <usuario> <mensagem>  - Envia mensagem privada");
            Console.WriteLine("/criargrupo <nomegrupo> <membro1,membro2,...>  - Cria um grupo");
            Console.WriteLine("/grupo <nomegrupo> <mensagem>  - Envia mensagem para o grupo");
            Console.WriteLine("/usuarios  - Lista usuários online");
            Console.WriteLine("/sair  - Encerra o programa");

            while (true)
            {
                Console.Write("\n> ");
                string entrada = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(entrada)) continue;

                if (entrada.StartsWith("/sair"))
                {
                    Console.WriteLine("Saindo...");
                    break;
                }
                else if (entrada.StartsWith("/privado "))
                {
                    var partes = entrada.Split(' ', 3);
                    if (partes.Length < 3)
                    {
                        Console.WriteLine("Uso: /privado <usuario> <mensagem>");
                        continue;
                    }
                    string destino = partes[1];
                    string mensagem = partes[2];

                    if (mensagem.Contains("/arquivo "))
                    {
                        var msgPartes = mensagem.Split(new[] { "/arquivo " }, StringSplitOptions.None);
                        string texto = msgPartes[0].Trim();
                        string caminho = msgPartes[1].Trim();
                        if (!File.Exists(caminho))
                        {
                            Console.WriteLine("Arquivo não encontrado.");
                            continue;
                        }
                        byte[] bytes = File.ReadAllBytes(caminho);
                        string base64 = Convert.ToBase64String(bytes);
                        string nomeArquivo = Path.GetFileName(caminho);
                        await chatService.SendAsync($"{destino}:{texto}:/arquivo:{nomeArquivo}:{base64}");
                    }
                    else
                    {
                        await chatService.SendAsync($"{destino}:{mensagem}");
                    }
                }
                else if (entrada.StartsWith("/criargrupo "))
                {
                    var partes = entrada.Split(' ', 3);
                    if (partes.Length < 3)
                    {
                        Console.WriteLine("Uso: /criargrupo <nomegrupo> <membro1,membro2,...>");
                        continue;
                    }
                    string nomeGrupo = partes[1];
                    string membros = partes[2];
                    await chatService.SendAsync($"/criargrupo:{nomeGrupo}:{membros}");
                }
                else if (entrada.StartsWith("/grupo "))
                {
                    var partes = entrada.Split(' ', 3);
                    if (partes.Length < 3)
                    {
                        Console.WriteLine("Uso: /grupo <nomegrupo> <mensagem>");
                        continue;
                    }
                    string nomeGrupo = partes[1];
                    string mensagem = partes[2];
                    
                    if (mensagem.Contains("/arquivo "))
                    {
                        var msgPartes = mensagem.Split(new[] { "/arquivo " }, StringSplitOptions.None);
                        string texto = msgPartes[0].Trim();
                        string caminho = msgPartes[1].Trim();
                        if (!File.Exists(caminho))
                        {
                            Console.WriteLine("Arquivo não encontrado.");
                            continue;
                        }
                        byte[] bytes = File.ReadAllBytes(caminho);
                        string base64 = Convert.ToBase64String(bytes);
                        string nomeArquivo = Path.GetFileName(caminho);
                        await chatService.SendAsync($"grupo:{nomeGrupo}:{texto}:/arquivo:{nomeArquivo}:{base64}");
                    }
                    else
                    {
                        await chatService.SendAsync($"grupo:{nomeGrupo}:{mensagem}");
                    }
                }
                else if (entrada == "/usuarios")
                {
                    await chatService.SendAsync("/usuarios");
                    // A resposta será exibida pelo listener
                }
                else
                {
                    Console.WriteLine("Comando não reconhecido.");
                }
            }

            chatService.Disconnect();
        }
    }
}
