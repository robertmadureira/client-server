using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Server
{
    static TcpListener listener; static ConcurrentDictionary<string, TcpClient> clientes = new();

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
                // Ex: "paraUsuario:mensagem"
                var partes = linha.Split(':', 2);
                if (partes.Length < 2) continue;

                string destino = partes[0];
                string mensagem = partes[1];

                if (clientes.TryGetValue(destino, out TcpClient destinoCliente))
                {
                    StreamWriter destinoWriter = new StreamWriter(destinoCliente.GetStream(), Encoding.UTF8) { AutoFlush = true };
                    destinoWriter.WriteLine($"{username}: {mensagem}");
                }
            }
        }
        catch { }

        clientes.TryRemove(username, out _);
        cliente.Close();
        Console.WriteLine($"{username} desconectado.");
    }
}