using System.Threading.Tasks;

namespace DBC.Services
{
    public interface IEmailTemplate
    {
        Task<string> RenderViewToString(string controller, string view, object model);
    }
}