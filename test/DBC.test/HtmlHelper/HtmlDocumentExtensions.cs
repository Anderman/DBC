using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace DBC.test.HtmlHelper
{
    public static class HtmlDocumentExtensions
    {
        public static FormValues FormValues(this XDocument htmlDocument, int formIndex = 1)
        {
            var nodes = htmlDocument.Descendants("form").ElementAt(formIndex - 1).Descendants("input");
            var kv = new FormValues();
            foreach (var node in nodes)
            {
                var name = node.Attribute("name")?.Value;
                if (name != null)
                {
                    if (kv.ContainsKey(name))
                    {
                        kv[name] = WebUtility.HtmlDecode(node.Attribute("value")?.Value ?? "");
                    }
                    else
                    {
                        kv.Add(name, WebUtility.HtmlDecode(node.Attribute("value")?.Value ?? ""));
                    }
                }
            }
            return kv;
        }

        public static string FormAction(this XDocument htmlDocument, int formIndex = 1)
        {
            var nodes = htmlDocument.Descendants("form").ElementAt(formIndex - 1);
            return nodes.Attribute("action").Value;
        }

        public static string InnerText(this XDocument htmlDocument)
        {
            return string.Join("", htmlDocument.Descendants("body").Select(a => a.Value));
        }
        public static string ErrorMsg(this XDocument htmlDocument)
        {
            var err = string.Join("", htmlDocument.Descendants("form").Descendants("title").Select(a => a.Value).Contains("Error")
                ? htmlDocument.Descendants("div").Where(a => a.Attribute("class").Value == "page-container").Descendants().Select(a => a.Value)
                : htmlDocument.Descendants("div").Where(a => a.Attribute("data-valmsg-summary")?.Value == "true").Descendants("ul").Descendants().Select(a => a.Value)
                );
            return string.IsNullOrWhiteSpace(err) ? null : err;
        }
    }
}