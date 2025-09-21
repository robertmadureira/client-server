using Servidor.Interface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servidor.Service
{
    internal class GroupService : IGroupService
    {
        private ConcurrentDictionary<string, ConcurrentBag<string>> _groups = new();

        public bool CreateGroup(string groupName, IEnumerable<string> members)
        {
            var bag = new ConcurrentBag<string>(members);
            return _groups.TryAdd(groupName, bag);
        }

        public bool AddMember(string groupName, string username)
        {
            if (_groups.TryGetValue(groupName, out var bag))
            {
                bag.Add(username);
                return true;
            }
            return false;
        }

        public bool RemoveMember(string groupName, string username)
        {
            if (_groups.TryGetValue(groupName, out var bag))
            {
                var temp = new ConcurrentBag<string>();
                foreach (var member in bag)
                    if (member != username) temp.Add(member);
                _groups[groupName] = temp;
                return true;
            }
            return false;
        }

        public IEnumerable<string> GetMembers(string groupName)
        {
            return _groups.TryGetValue(groupName, out var bag) ? bag : Enumerable.Empty<string>();
        }

        public bool GroupExists(string groupName) => _groups.ContainsKey(groupName);
    }
}

