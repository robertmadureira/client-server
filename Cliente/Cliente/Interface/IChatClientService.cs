using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cliente.Interface
{
    internal interface IChatClientService
    {
        Task ConnectAsync(string ipServidor, string nomeUsuario);
        Task SendAsync(string mensagem);
        void Listen();
        void Disconnect();
    }
}
