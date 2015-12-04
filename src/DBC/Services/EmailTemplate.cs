using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNet.Http.Internal;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Infrastructure;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.Mvc.ViewEngines;
using Microsoft.AspNet.Mvc.ViewFeatures;

namespace DBC.Services
{
    public class EmailTemplate : IEmailTemplate
    {
        private readonly ActionContext _actionContext;
        private readonly ICompositeViewEngine _compositeViewEngine;

        public EmailTemplate(ICompositeViewEngine compositeViewEngine, IActionContextAccessor actionContextAccessor)
        {
            _compositeViewEngine = compositeViewEngine;
            _actionContext = actionContextAccessor.ActionContext; //needed because razorview depend on it (No service for type 'Microsoft.AspNet.Mvc.IUrlHelper' has been registered.)
        }

        public async Task<string> RenderViewToString<TModel>(string view, TModel model)
        {
            var viewEngineResult = _compositeViewEngine.FindView(_actionContext, view);
            var viewData = new ViewDataDictionary<TModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = model
            };
            using (var sw = new StringWriter())
            {
                var viewContext = new ViewContext(
                    _actionContext,
                    viewEngineResult.View,
                    viewData,
                    new TempDataDictionary(new HttpContextAccessor(), new SessionStateTempDataProvider()),
                    sw,
                    new HtmlHelperOptions());
                await viewEngineResult.View.RenderAsync(viewContext);
                return sw.ToString();
            }
        }
    }
}