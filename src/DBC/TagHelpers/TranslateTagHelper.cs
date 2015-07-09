using System.Threading.Tasks;
using Microsoft.AspNet.Razor.Runtime.TagHelpers;

namespace DBC.Helpers
{
    [TargetElement("translate")]
    public class TranslateTagHelper : TagHelper
    {
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var childContent = await context.GetChildContentAsync();
            output.TagName = "";
            output.Content.SetContent(childContent.ToString().Localize());
        }
    }
}