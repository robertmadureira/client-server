using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Servidor.Interface
{
    internal interface IChatHandlerService
    {
        Task HandleClientAsync(TcpClient cliente);
    }
}
