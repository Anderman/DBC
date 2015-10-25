using System.Threading.Tasks;

namespace DBC.Services
{
    public interface IEmailTemplate
    {
        Task<string> RenderViewToString<TModel>(string view, TModel model);
    }
}