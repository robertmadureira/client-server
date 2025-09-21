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
    internal class UserService : IUserService
    {
        private ConcurrentDictionary<string, TcpClient> _users = new();

        public bool AddUser(string username, TcpClient client) => _users.TryAdd(username, client);
        public bool RemoveUser(string username) => _users.TryRemove(username, out _);
        public TcpClient GetUser(string username) => _users.TryGetValue(username, out var client) ? client : null;
        public IEnumerable<string> GetAllUsers() => _users.Keys;
    }
}