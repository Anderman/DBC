using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.TagHelpers;
using Microsoft.AspNet.Razor.Runtime.TagHelpers;
using Microsoft.Framework.OptionsModel;

namespace DBC.Helpers
{
    [TargetElement("script", Attributes = "asp-fallback-src")]
    [TargetElement("script", Attributes = "asp-copy-src-to-fallback")]
    public class ScriptTagHelper : TagHelper
    {
        private readonly string preTest = @"<SCRIPT>({0}==null||alert('{1}'));</SCRIPT>";
        private readonly string postTest = @"<SCRIPT>({0}!=null||alert('{1}'));</SCRIPT>";
        [HtmlAttributeName("asp-copy-src-to-fallback")]
        public string CopySrcToFallback { get; set; }

        [HtmlAttributeName("asp-warn-if-test-is-invalid")]
        public string WarnIfTestIsInvalid { get; set; }

        [HtmlAttributeName("asp-fallback-src")]
        public string FallbackSrc { get; set; }

        [HtmlAttributeName("asp-fallback-test")]
        public string FallbackTest { get; set; }

        [HtmlAttributeName("src")]
        public string RemotePath { get; set; }

        protected internal IHostingEnvironment HostingEnvironment { get; set; }
        protected internal IHttpContextAccessor Context { get; set; }
        protected internal IOptions<AppSettings> AppSettings { get; set; }
        public ScriptTagHelper(IHostingEnvironment hostingEnvironment, IHttpContextAccessor context, IOptions<AppSettings> appSettings)
        {
            HostingEnvironment = hostingEnvironment;
            Context = context;
            AppSettings = appSettings;
        }
        //
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (WarnIfTestIsInvalid?.Contains(HostingEnvironment.EnvironmentName, StringComparison.OrdinalIgnoreCase) ==
                true)
            {
                var x= await context.GetChildContentAsync();
                output.PreElement.AppendFormat(preTest, FallbackTest, $"Script `{RemotePath}` already loaded. Did you create the correct test");
                output.PostElement.AppendFormat(postTest, FallbackTest, $"Script `{RemotePath}` still not loaded. Did you create the correct test");
            }
            if (CopySrcToFallback?.Contains(HostingEnvironment.EnvironmentName, StringComparison.OrdinalIgnoreCase) ==
                true)
            {
                try
                {
                    RemotePath = RemotePath.StartsWith("/")
                        ? Context.HttpContext.Request.Scheme + "://" + Context.HttpContext.Request.Host + RemotePath
                        : RemotePath;

                    FallbackSrc = FallbackSrc
                                  ??
                                  (AppSettings.Options.GetType()
                                      ?.GetProperty("ScriptFallbackPath")
                                      ?.GetValue(AppSettings.Options)
                                   ?? "/fallback/js/") + new Uri(RemotePath).Segments.Last();
                    var localPath = HostingEnvironment.MapPath(FallbackSrc.TrimStart('/'));
                    var pathHelper = new PathHelper(RemotePath, localPath);
                    if (!File.Exists(localPath))
                    {
                        using (var webClient = new WebClient())
                        {
                            var file = await webClient.DownloadStringTaskAsync(new Uri(RemotePath));
                            Directory.CreateDirectory(pathHelper.LocalDirectory);
                            File.WriteAllText(localPath, file);
                        }
                    }
                }
                catch (WebException ex)
                {
                    throw new FileNotFoundException($"The remote file:{RemotePath} cannot be found.", ex);
                }
                output.CopyHtmlAttribute("src", context);
            }
        }

    }
}