using Servidor.Service;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        try
        {
            var chatService = new ChatHandlerService();
            var listener = new TcpListener(IPAddress.Any, 5000);
            listener.Start();
            Console.WriteLine("Servidor iniciado...");

            while (true)
            {
                TcpClient cliente = await listener.AcceptTcpClientAsync();
                _ = chatService.HandleClientAsync(cliente); // chama o método da service
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
        
    }
}
