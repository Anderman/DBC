using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using DBC.test.TestApplication;
using Microsoft.AspNet.Http;
namespace DBC.test.HtmlHelper
{
    public class ClientWrapper
    {
        public string Html;
        private readonly HttpClient _client;
        private readonly TestMessageServices _testMessageServices;
        public XDocument HtmlDocument = new XDocument();
        public HttpResponseMessage ResponseMsg;
        private readonly formValues _cookies = new formValues();
        private int _errorNumber = 0;


        public ClientWrapper(HttpClient client, TestMessageServices testMessageServices)
        {
            _client = client;
            _testMessageServices = testMessageServices;
        }

        public ClientWrapper(HttpClient client)
        {
            _client = client;
        }

        public async Task<HttpResponseMessage> Post(int formIndex, formValues defaults)
        {
            var formVal = HtmlDocument.FormValues(formIndex);
            var url = HtmlDocument.FormAction(formIndex);
            return await Post(url, formVal, defaults);
        }

        public async Task<HttpResponseMessage> Post(string url, formValues formValues, formValues defaults)
        {
            foreach (var value in defaults)
            {
                formValues[value.Key] = value.Value;
            }
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("Accept-Language", "en-US");
            request.Headers.Add("Cookie", _cookies.Select(c => c.Key + "=" + c.Value));
            request.Content = new FormUrlEncodedContent(formValues);

            ResponseMsg = await _client.SendAsync(request);
            AddCookies(ResponseMsg);
            if (ResponseMsg.Headers.Location != null)
            {
                return await Get(ResponseMsg.Headers.Location.OriginalString);
            }
            else
            {
                Html = await ResponseMsg.Content.ReadAsStringAsync();
                HtmlDocument = (Html.StartsWith("<!")) ? XDocument.Parse(Html) : Html.StartsWith("{") ? new XDocument() : XDocument.Parse("<root>" + Html + "</root>");
                if (ResponseMsg.StatusCode != HttpStatusCode.OK)
                {
                    File.WriteAllText($"error{++_errorNumber}.html", Html);
                    File.WriteAllText($"error{_errorNumber}.txt", string.Join(
                        "\n", formValues.Select(a => $"'{a.Key}'='{a.Value}'")
                        ));
                }
                return await Task.FromResult(ResponseMsg);
            }
        }
        public async Task<HttpResponseMessage> Get(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Accept-Language", "en-US");
            request.Headers.Add("Cookie", _cookies.Select(c => c.Key + "=" + c.Value));
            ResponseMsg = await _client.SendAsync(request);
            if (ResponseMsg.Headers.Location != null)
            {
                return await Get(ResponseMsg.Headers.Location.OriginalString);
            }
            Html = await ResponseMsg.Content.ReadAsStringAsync();
            HtmlDocument = (Html.StartsWith("<!")) ? XDocument.Parse(Html) : Html.StartsWith("{") ? new XDocument() : XDocument.Parse("<root>" + Html + "</root>");


            //HtmlDocument.LoadHtml(Html);
            AddCookies(ResponseMsg);
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

        public string AbsolutePath => ResponseMsg.RequestMessage.RequestUri.AbsolutePath;


        public void AddCookies(HttpResponseMessage response)
        {
            IEnumerable<string> setCookies;
            if (response.Headers.TryGetValues("Set-Cookie", out setCookies))
            {
                foreach (var setCookie in setCookies)
                {
                    var cookiekv = setCookie.Split(';').First().Split('=');
                    if (_cookies.ContainsKey(cookiekv[0]))
                    {
                        _cookies[cookiekv[0]] = cookiekv[1];
                    }
                    else
                    {
                        _cookies.Add(cookiekv[0], cookiekv[1]);
                    }
                }
            }
        }

    }

    public static class HtmlDocumentExtensions
    {
        public static formValues FormValues(this XDocument htmlDocument, int formIndex = 1)
        {

            var nodes = htmlDocument.Descendants("form").ElementAt(formIndex-1).Descendants("input");
            var kv = new formValues();
            foreach (var node in nodes)
            {
                string name = node.Attribute("name")?.Value;
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
            var nodes = htmlDocument.Descendants("form").ElementAt(formIndex-1);
            return nodes.Attribute("action").Value;
        }
        public static string ErrorMsg(this XDocument htmlDocument)
        {
            var err = string.Join("", htmlDocument.Descendants("form").Descendants("title").ToString().Contains("Error") == true 
                ? htmlDocument.Descendants("div").Where(a => a.Attribute("class").Value == "page-container").Descendants().Select(a=>a.Value) 
                : htmlDocument.Descendants("div").Where(a => a.Attribute("data-valmsg-summary")?.Value == "true").Descendants("ul").Descendants().Select(a => a.Value)
                );
            return string.IsNullOrWhiteSpace(err) ? null : err;
        }
    }
    public class formValues : Dictionary<string, string>
    {

    }
}


