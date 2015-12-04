using System.Threading.Tasks;

namespace DBC.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}