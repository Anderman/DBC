using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CodeComb.HtmlAgilityPack;
using DBC.test.TestApplication;
using Microsoft.AspNet.Http;
namespace DBC.test.HtmlHelper
{
    public class ClientWrapper
    {
        public string Html;
        private readonly HttpClient _client;
        private readonly TestMessageServices _testMessageServices;
        public HtmlDocument Doc = new HtmlDocument();
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
            var formVal = Doc.FormValues(formIndex);
            var url = Doc.FormAction(formIndex);
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
            foreach (var cookie in _cookies)
            {
                request.Headers.Add("Cookie", cookie.Key + "=" + cookie.Value);
            }
            //var kv2 = _allInputs.ToList();
            request.Content = new FormUrlEncodedContent(formValues);
            //var headercontent = await request.Content.ReadAsStringAsync();

            ResponseMsg = await _client.SendAsync(request);
            if (ResponseMsg.Headers.Location != null)
            {
                return await Get(ResponseMsg.Headers.Location.OriginalString);
            }
            else
            {
                Html = await ResponseMsg.Content.ReadAsStringAsync();
                Doc.LoadHtml(Html);
                if (ResponseMsg.StatusCode != HttpStatusCode.OK)
                {
                    File.WriteAllText($"error{++_errorNumber}.html", Html);
                    File.WriteAllText($"error{_errorNumber}.txt", string.Join(
                        "\n", formValues.Select(a => $"'{a.Key}'='{a.Value}'")
                        ));
                }

                AddCookies(ResponseMsg);

                return await Task.FromResult(ResponseMsg);
            }
        }
        public async Task<HttpResponseMessage> Get(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Accept-Language", "en-US");
            ResponseMsg = await _client.SendAsync(request);
            Html = await ResponseMsg.Content.ReadAsStringAsync();
            Doc.LoadHtml(Html);
            AddCookies(ResponseMsg);

            //_allInputs = GetAllInputFromForm(formIndex: formIndex);

            return await Task.FromResult(ResponseMsg);
        }

        public async Task<HttpResponseMessage> Click_on_Link_in_Email(string emailAddress)
        {
            var url = _testMessageServices.TestHtmlEmail[emailAddress].Url;
            return await Get(url);
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
        public static formValues FormValues(this HtmlDocument htmlDocument, int formIndex = 1)
        {
            var nodes = htmlDocument.DocumentNode.SelectNodes($"//form[{formIndex}]//input");
            var kv = new formValues();
            if (nodes != null)
            {
                foreach (HtmlNode node in nodes)
                {
                    string name = node.Attributes["name"]?.Value;
                    if (name != null)
                    {
                        if (kv.ContainsKey(name))
                        {
                            kv[name] = WebUtility.HtmlDecode(node.Attributes["value"]?.Value);
                        }
                        else
                        {
                            kv.Add(name, WebUtility.HtmlDecode(node.Attributes["value"]?.Value));
                        }
                    }
                }
            }
            return kv;
        }
        public static string FormAction(this HtmlDocument htmlDocument, int formIndex = 1)
        {
            var nodes = htmlDocument.DocumentNode.SelectNodes($"//form[{formIndex}]");
            return nodes.First().Attributes["action"]?.Value;
        }
        public static string ErrorMsg(this HtmlDocument htmlDocument)
        {
            var nodes = htmlDocument.DocumentNode.SelectNodes("//div[@data-valmsg-summary='true']");
            if (nodes?.Count > 0)
                return nodes.First()?.InnerText;
            else
                return null;
        }
    }
    public class formValues : Dictionary<string, string>
    {

    }
}


