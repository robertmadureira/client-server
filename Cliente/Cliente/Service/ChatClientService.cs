using Cliente.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Cliente.Service
{
    internal class ChatClientService : IChatClientService
    {
        private TcpClient _cliente;
        private StreamReader _reader;
        private StreamWriter _writer;
        private Thread _listenThread;

        public async Task ConnectAsync(string ipServidor, string nomeUsuario)
        {
            _cliente = new TcpClient();
            await _cliente.ConnectAsync(ipServidor, 5000);
            var stream = _cliente.GetStream();
            _reader = new StreamReader(stream, Encoding.UTF8);
            _writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
            await _writer.WriteLineAsync(nomeUsuario);
        }

        public async Task SendAsync(string mensagem)
        {
            if (_writer != null)
                await _writer.WriteLineAsync(mensagem);
        }

        public void Listen()
        {
            _listenThread = new Thread(() =>
            {
                try
                {
                    string msg;
                    while ((msg = _reader.ReadLine()) != null)
                    {
                        Console.WriteLine("\n[Recebido] " + msg);
                        Console.Write("\n> ");
                    }
                }
                catch { }
                Console.WriteLine("\nConexão encerrada pelo servidor.");
                Environment.Exit(0);
            });
            _listenThread.IsBackground = true;
            _listenThread.Start();
        }

        public void Disconnect()
        {
            try { _cliente?.Close(); } catch { }
        }
    }
}