using System.Diagnostics;
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
        public static string GetLink(this XDocument htmlDocument, string link, int linkNumber = 1)
        {
            var nodes = htmlDocument.Descendants().Where(a => a.Attribute("href")?.Value == link).ElementAt(linkNumber - 1);
            return nodes?.Attribute("href").Value;
        }
        public static string ShowLinks(this XDocument htmlDocument)
        {
            var nodes = htmlDocument.Descendants("body").Descendants().Where(a => a.Attribute("href")?.Value != null);
            var summary= "\nlink: "+ string.Join("\nlink: ", nodes.Select(a => a.Attribute("href").Value +" ("+ string.Join(",", a.Value) + ")"));
            
            nodes = htmlDocument.Descendants("button").Where(a => a.Attribute("action")?.Value != null);
            nodes = htmlDocument.Descendants("form");//.Where(a => a.Attribute("action")?.Value != null);
            return summary + "\nform: " +string.Join("\nform: ", nodes.Select(a => " (" + string.Join(",", a.Value) + ")"));
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