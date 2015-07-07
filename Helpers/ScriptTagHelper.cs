using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Razor.Runtime.TagHelpers;

namespace DBC.Helpers
{
    [TargetElement("script", Attributes = "asp-fallback-src")]
    public class ScriptTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-fallback-src")]
        public string FallbackSrc { get; set; }

        [HtmlAttributeName("src")]
        public string Src { get; set; }

        protected internal IHostingEnvironment HostingEnvironment { get; set; }
        public ScriptTagHelper(IHostingEnvironment hostingEnvironment, IHttpContextAccessor context)
        {
            HostingEnvironment = hostingEnvironment;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (HostingEnvironment.EnvironmentName == "Development")
            {
                var path = HostingEnvironment.MapPath(FallbackSrc.TrimStart('/'));
                if (!File.Exists(path))
                {
                    using (var webClient = new WebClient())
                    {
                        var file = await webClient.DownloadStringTaskAsync(new Uri(Src));
                        Directory.CreateDirectory(Path.GetDirectoryName(path));
                        File.WriteAllText(path, file);
                    }
                }
            }
        }

    }
}