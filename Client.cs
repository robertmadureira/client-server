using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Client
{
    static void Main()
    {
        Console.Write("Digite o IP do Servidor: "); 
        string ipServidor = Console.ReadLine();    

        Console.Write("Seu nome: "); string nome = Console.ReadLine();

        TcpClient cliente = new TcpClient(ipServidor, 5000);
        NetworkStream stream = cliente.GetStream();
        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
        StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

        writer.WriteLine(nome); // envia nome ao servidor

        // Thread para ouvir mensagens recebidas
        new Thread(() =>
        {
            string msg;
            while ((msg = reader.ReadLine()) != null)
            {
                Console.WriteLine(msg);
            }
        }).Start();

        // Envio de mensagens
        while (true)
        {
            Console.Write("Para (nome): ");
            string destino = Console.ReadLine();
            Console.Write("Mensagem: ");
            string mensagem = Console.ReadLine();

            writer.WriteLine($"{destino}:{mensagem}");
        }
    }
}