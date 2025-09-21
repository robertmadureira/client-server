using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Servidor.Interface
{
    internal interface IUserService
    {
        bool AddUser(string username, TcpClient client);
        bool RemoveUser(string username);
        TcpClient GetUser(string username);
        IEnumerable<string> GetAllUsers();
    }
}
