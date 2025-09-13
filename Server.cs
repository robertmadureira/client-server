using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Server
{
    static TcpListener listener;
    static ConcurrentDictionary<string, TcpClient> clientes = new();
    static ConcurrentDictionary<string, ConcurrentBag<string>> grupos = new(); // grupo -> membros

    static void Main()
    {
        listener = new TcpListener(IPAddress.Any, 5000);
        listener.Start();
        Console.WriteLine("Servidor iniciado...");

        while (true)
        {
            TcpClient cliente = listener.AcceptTcpClient();
            Thread t = new Thread(HandleClient);
            t.Start(cliente);
        }
    }

    static void HandleClient(object obj)
    {
        TcpClient cliente = (TcpClient)obj;
        NetworkStream stream = cliente.GetStream();
        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
        StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

        string username = reader.ReadLine();
        clientes.TryAdd(username, cliente);
        Console.WriteLine($"{username} conectado.");

        try
        {
            string linha;
            while ((linha = reader.ReadLine()) != null)
            {
                // Comando para criar grupo: criargrupo:nomegrupo:membro1,membro2,...
                if (linha.StartsWith("/criargrupo:"))
                {
                    var partes = linha.Split(':', 3);
                    if (partes.Length < 3)
                    {
                        writer.WriteLine("Uso: /criargrupo:nomegrupo:membro1,membro2,...");
                        continue;
                    }
                    string nomeGrupo = partes[1];
                    var membros = partes[2].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    var bag = new ConcurrentBag<string>(membros);
                    if (!bag.Contains(username)) bag.Add(username); // adiciona o criador
                    grupos[nomeGrupo] = bag;
                    writer.WriteLine($"Grupo '{nomeGrupo}' criado com membros: {string.Join(", ", bag)}");
                    continue;
                }

                // Mensagem para grupo: grupo:nomegrupo:mensagem
                if (linha.StartsWith("grupo:"))
                {
                    var partes = linha.Split(':', 3);
                    if (partes.Length < 3)
                    {
                        writer.WriteLine("Uso: grupo:nomegrupo:mensagem");
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
                                    destinoWriter.WriteLine($"[Grupo {nomeGrupo}] {username}: {mensagem}");
                                }
                                catch { }
                            }
                        }
                        writer.WriteLine($"Mensagem enviada ao grupo '{nomeGrupo}'.");
                    }
                    else
                    {
                        writer.WriteLine($"Grupo '{nomeGrupo}' não existe.");
                    }
                    continue;
                }

                // Mensagem privada: paraUsuario:mensagem
                var partesPriv = linha.Split(':', 2);
                if (partesPriv.Length < 2) continue;
                string destino = partesPriv[0];
                string mensagemPriv = partesPriv[1];

                if (clientes.TryGetValue(destino, out TcpClient destinoClientePriv))
                {
                    StreamWriter destinoWriter = new StreamWriter(destinoClientePriv.GetStream(), Encoding.UTF8) { AutoFlush = true };
                    destinoWriter.WriteLine($"{username}: {mensagemPriv}");
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
        Console.WriteLine($"{username} desconectado.");
    }
}