using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.Routing;

namespace DBC.Services
{
    public class EmailTemplate : IEmailTemplate
    {
        private readonly ICompositeViewEngine _compositeViewEngine;
        private readonly HttpContext _httpContext;

        public EmailTemplate(ICompositeViewEngine compositeViewEngine, IHttpContextAccessor httpContextAccessor)
        {
            _compositeViewEngine = compositeViewEngine;
            _httpContext = httpContextAccessor.HttpContext;
        }

        public async Task<string> RenderViewToString(string controller, string view, object model)
        {
            var routeData = new RouteData {Values = {["Controller"] = controller}};
            var actionDescriptor = new ActionDescriptor {RouteConstraints = new List<RouteDataActionConstraint>()};
            var actionContext = new ActionContext(_httpContext, routeData, actionDescriptor);
            var viewEngineResult = _compositeViewEngine.FindView(actionContext, view);
            var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = model
            };
            using (var sw = new StringWriter())
            {
                var viewContext = new ViewContext(actionContext, viewEngineResult.View, viewData, null, sw,null);
                await viewEngineResult.View.RenderAsync(viewContext);
                sw.Flush();
                return sw.ToString();
            }
        }
    }
}