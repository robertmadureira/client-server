using Servidor.Interface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Servidor.Service
{
    internal class ChatHandlerService : IChatHandlerService
    {
        private ConcurrentDictionary<string, TcpClient> clientes = new();
        private ConcurrentDictionary<string, ConcurrentBag<string>> grupos = new();

        public async Task HandleClientAsync(TcpClient cliente)
        {
            NetworkStream stream = cliente.GetStream();
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

            string username = await reader.ReadLineAsync();
            clientes.TryAdd(username, cliente);
            Console.WriteLine($"{username} conectado.");

            try
            {
                string linha;
                while ((linha = await reader.ReadLineAsync()) != null)
                {
                    // Criar grupo
                    if (linha.StartsWith("/criargrupo:"))
                    {
                        var partes = linha.Split(':', 3);
                        if (partes.Length < 3)
                        {
                            await writer.WriteLineAsync("Uso: /criargrupo:nomegrupo:membro1,membro2,...");
                            continue;
                        }
                        string nomeGrupo = partes[1];
                        var membros = partes[2].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                        var bag = new ConcurrentBag<string>(membros);
                        if (!bag.Contains(username)) bag.Add(username);
                        grupos[nomeGrupo] = bag;
                        await writer.WriteLineAsync($"Grupo '{nomeGrupo}' criado com membros: {string.Join(", ", bag)}");
                        continue;
                    }

                    // Mensagem para grupo
                    if (linha.StartsWith("grupo:"))
                    {
                        var partes = linha.Split(':', 3);
                        if (partes.Length < 3)
                        {
                            await writer.WriteLineAsync("Uso: grupo:nomegrupo:mensagem");
                            continue;
                        }
                        string nomeGrupo = partes[1];
                        string mensagem = partes[2];
                        if (grupos.TryGetValue(nomeGrupo, out var membros))
                        {
                            foreach (var membro in membros)
                            {
                                if (clientes.TryGetValue(membro, out TcpClient destinoCliente) && membro != username)
                                {
                                    try
                                    {
                                        StreamWriter destinoWriter = new StreamWriter(destinoCliente.GetStream(), Encoding.UTF8) { AutoFlush = true };
                                        await destinoWriter.WriteLineAsync($"[Grupo {nomeGrupo}] {username}: {mensagem}");
                                    }
                                    catch { }
                                }
                            }
                            await writer.WriteLineAsync($"Mensagem enviada ao grupo '{nomeGrupo}'.");
                        }
                        else
                        {
                            await writer.WriteLineAsync($"Grupo '{nomeGrupo}' não existe.");
                        }
                        continue;
                    }

                    // Mensagem privada
                    var partesPriv = linha.Split(':', 2);
                    if (partesPriv.Length < 2) continue;
                    string destino = partesPriv[0];
                    string mensagemPriv = partesPriv[1];

                    if (clientes.TryGetValue(destino, out TcpClient destinoClientePriv))
                    {
                        StreamWriter destinoWriter = new StreamWriter(destinoClientePriv.GetStream(), Encoding.UTF8) { AutoFlush = true };
                        await destinoWriter.WriteLineAsync($"{username}: {mensagemPriv}");
                    }

                    if (linha == "/usuarios")
                    {
                        var online = string.Join(", ", clientes.Keys);
                        await writer.WriteLineAsync($"Usuários online: {online}");
                        continue;
                    }
                }
            }
            catch { }

            clientes.TryRemove(username, out _);
            // Remover usuário de todos os grupos
            foreach (var grupo in grupos)
            {
                while (grupo.Value.TryTake(out string membro))
                {
                    if (membro != username)
                        grupo.Value.Add(membro);
                }
            }
            cliente.Close();
            Console.WriteLine($"Usuário {username} desconectado.");
        }
    }
}