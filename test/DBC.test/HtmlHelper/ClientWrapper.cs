using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using DBC.test.TestApplication;

namespace DBC.test.HtmlHelper
{
    public class ClientWrapper
    {
        private readonly HttpClient _client;
        private readonly Cookies _cookies = new Cookies();
        private readonly TestMessageServices _testMessageServices;
        private int _errorNumber;
        public string Html;
        public XDocument HtmlDocument = new XDocument();
        public HttpResponseMessage ResponseMsg;


        public ClientWrapper(HttpClient client, TestMessageServices testMessageServices)
        {
            _client = client;
            _testMessageServices = testMessageServices;
        }

        public ClientWrapper(HttpClient client)
        {
            _client = client;
        }

        public string AbsolutePath => ResponseMsg.RequestMessage.RequestUri.AbsolutePath;

        public async Task<HttpResponseMessage> Post(int formIndex, object defaults)
        {
            var formVal = HtmlDocument.FormValues(formIndex);
            var url = HtmlDocument.FormAction(formIndex);
            return await Post(url, formVal, defaults.AsFormValues());
        }

        public async Task<HttpResponseMessage> Post(string url, FormValues formValues, FormValues defaults)
        {
            foreach (var value in defaults)
            {
                formValues[value.Key] = value.Value;
            }
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("Accept-Language", "en-US");
            request.Headers.Add("cookie", _cookies.Select(c => c.Key + "=" + c.Value.Value));
            request.Content = new FormUrlEncodedContent(formValues);

            ResponseMsg = await _client.SendAsync(request);
            _cookies.Add(ResponseMsg);
            if (ResponseMsg.Headers.Location != null)
            {
                return await Get(ResponseMsg.Headers.Location.OriginalString);
            }
            Html = await ResponseMsg.Content.ReadAsStringAsync();
            HtmlDocument = Html.StartsWith("<!") ? XDocument.Parse(Html) : Html.StartsWith("{") ? new XDocument() : XDocument.Parse("<root>" + Html + "</root>");
            if (ResponseMsg.StatusCode != HttpStatusCode.OK)
            {
                File.WriteAllText($"error{++_errorNumber}.html", Html);
                File.WriteAllText($"error{_errorNumber}.txt", string.Join(
                    "\n", formValues.Select(a => $"'{a.Key}'='{a.Value}'")
                    ));
            }
            return await Task.FromResult(ResponseMsg);
        }

        public async Task<HttpResponseMessage> Get(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Accept-Language", "en-US");
            request.Headers.Add("cookie", _cookies.Select(c => c.Key + "=" + c.Value.Value));
            ResponseMsg = await _client.SendAsync(request);
            if (ResponseMsg.Headers.Location != null)
            {
                return await Get(ResponseMsg.Headers.Location.OriginalString);
            }
            Html = await ResponseMsg.Content.ReadAsStringAsync();
            HtmlDocument = Html.StartsWith("<!") ? XDocument.Parse(Html) : Html.StartsWith("{") ? new XDocument() : XDocument.Parse("<root>" + Html + "</root>");

            //HtmlDocument.LoadHtml(Html);
            _cookies.Add(ResponseMsg);
            return await Task.FromResult(ResponseMsg);
        }

        public async Task<HttpResponseMessage> Click_on_Link_in_Email(string emailAddress)
        {
            var url = _testMessageServices.TestHtmlEmail[emailAddress].Url;
            return await Get(url);
        }

        public string getSecurityCode(string emailAddress)
        {
            return _testMessageServices.TestHtmlEmail[emailAddress].Body.Split(':')?[1];
        }


        public DateTime? GetCookieExpiredDate(string cookie)
        {
            return null;
        }
    }


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

        public static string ErrorMsg(this XDocument htmlDocument)
        {
            var err = string.Join("", htmlDocument.Descendants("form").Descendants("title").ToString().Contains("Error")
                ? htmlDocument.Descendants("div").Where(a => a.Attribute("class").Value == "page-container").Descendants().Select(a => a.Value)
                : htmlDocument.Descendants("div").Where(a => a.Attribute("data-valmsg-summary")?.Value == "true").Descendants("ul").Descendants().Select(a => a.Value)
                );
            return string.IsNullOrWhiteSpace(err) ? null : err;
        }
    }

    public class FormValues : Dictionary<string, string>
    {
        public string HasCorrectValues(object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            foreach (var u in source.GetType().GetProperties(bindingAttr).Where(p => p.GetValue(source, null) != null))
            {
                if (!ContainsKey(u.Name))
                    return $"Form input '{u.Name}' is missing";

                var actual = u.GetValue(source, null);
                var expected = this[u.Name];
                if (!(actual is string))
                {
                    actual = actual.ToString().ToLowerInvariant();
                    expected = expected.ToLowerInvariant();
                }
                if (actual.ToString() != expected)
                {
                    return $"{u.Name}={actual} expected:{expected}";
                }
            }
            return "";
        }
    }

    public static class ObjectExtensions
    {
        public static FormValues AsFormValues(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            var formValues = source as FormValues;
            if (formValues == null)
            {
                formValues = new FormValues();
                foreach (var u in source.GetType().GetProperties(bindingAttr).Where(p => p.GetValue(source, null) != null))
                {
                    formValues.Add(u.Name, u.GetValue(source, null).ToString());
                }
            }
            return formValues;
        }
    }
}