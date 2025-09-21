using Cliente.Service;
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        try
        {
            var clientService = new ClientService();
            await clientService.RunChatClientAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
    }
}
