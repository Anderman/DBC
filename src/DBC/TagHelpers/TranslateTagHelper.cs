using System.Threading.Tasks;
using Microsoft.AspNet.Razor.TagHelpers;

namespace DBC.TagHelpers
{
    [HtmlTargetElement("translate")]
    public class TranslateTagHelper : TagHelper
    {
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var childContent = await output.GetChildContentAsync();
            output.TagName = "";
            output.Content.SetHtmlContent(childContent.GetContent().ToString().Localize());
        }
    }
}