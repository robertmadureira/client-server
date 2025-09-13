using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Client
{
    static void Main()
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

        TcpClient cliente = null;
        try
        {
            cliente = new TcpClient(ipServidor, 5000);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao conectar: {ex.Message}");
            return;
        }

        NetworkStream stream = cliente.GetStream();
        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
        StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

        writer.WriteLine(nome); // autenticação simples

        // Thread para ouvir mensagens recebidas
        new Thread(() =>
        {
            try
            {
                string msg;
                while ((msg = reader.ReadLine()) != null)
                {
                    Console.WriteLine("\n[Recebido] " + msg);
                    Console.Write("\n> ");
                }
            }
            catch { }
            Console.WriteLine("\nConexão encerrada pelo servidor.");
            Environment.Exit(0);
        }) { IsBackground = true }.Start();

        Console.WriteLine("\nComandos:");
        Console.WriteLine("/privado <usuario> <mensagem>  - Envia mensagem privada");
        Console.WriteLine("/criargrupo <nomegrupo> <membro1,membro2,...>  - Cria um grupo");
        Console.WriteLine("/grupo <nomegrupo> <mensagem>  - Envia mensagem para o grupo");
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
                writer.WriteLine($"{destino}:{mensagem}");
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
                writer.WriteLine($"/criargrupo:{nomeGrupo}:{membros}");
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
                writer.WriteLine($"grupo:{nomeGrupo}:{mensagem}");
            }
            else
            {
                Console.WriteLine("Comando não reconhecido.");
            }
        }

        try { cliente.Close(); } catch { }
    }
}
