using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servidor.Interface
{
    internal interface IGroupService
    {
        bool CreateGroup(string groupName, IEnumerable<string> members);
        bool AddMember(string groupName, string username);
        bool RemoveMember(string groupName, string username);
        IEnumerable<string> GetMembers(string groupName);
        bool GroupExists(string groupName);
    }
}
