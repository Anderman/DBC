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
using Microsoft.AspNet.Http;
namespace DBC.test.HtmlHelper
{
    public class ClientWrapper
    {
        public string Html;
        private readonly HttpClient _client;
        public HtmlDocument Doc = new HtmlDocument();
        public HttpResponseMessage ResponseMsg;
        private readonly formValues _cookies = new formValues();
        private formValues _allInputs;
        private int _errorNumber = 0;


        public ClientWrapper(HttpClient client)
        {
            _client = client;
        }
        public async Task<HttpResponseMessage> Post(string url, int formIndex, formValues values)
        {
            foreach (var value in values)
            {
                _allInputs[value.Key] = value.Value;
            }
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            foreach (var cookie in _cookies)
            {
                request.Headers.Add("Cookie", cookie.Key + "=" + cookie.Value);
            }
            //var kv2 = _allInputs.ToList();
            request.Content = new FormUrlEncodedContent(_allInputs);
            //var headercontent = await request.Content.ReadAsStringAsync();

            ResponseMsg = await _client.SendAsync(request);
            if (ResponseMsg.Headers.Location != null)
            {
                return await Get(ResponseMsg.Headers.Location.OriginalString, formIndex);
            }
            else
            {
                Html = await ResponseMsg.Content.ReadAsStringAsync();
                Doc.LoadHtml(Html);
                if (ResponseMsg.StatusCode != HttpStatusCode.OK)
                {
                    File.WriteAllText($"error{++_errorNumber}.html", Html);
                    File.WriteAllText($"error{_errorNumber}.txt", string.Join(
                        "\n", _allInputs.Select(a => $"'{a.Key}'='{a.Value}'")
                        ));
                }

                AddCookies(ResponseMsg);
                _allInputs = GetAllInputFromForm(formIndex: formIndex);

                return await Task.FromResult(ResponseMsg);
            }
        }
        public static string StripHTML(string HTMLText, bool decode = true)
        {
            Regex reg = new Regex("<[^>]+>", RegexOptions.IgnoreCase);
            var stripped = reg.Replace(HTMLText, "");
            return decode ? WebUtility.HtmlDecode(stripped) : stripped;
        }
        public async Task<HttpResponseMessage> Get(string url, int formIndex)
        {
            ResponseMsg = await _client.GetAsync(url);
            Html = await ResponseMsg.Content.ReadAsStringAsync();
            Doc.LoadHtml(Html);
            AddCookies(ResponseMsg);

            _allInputs = GetAllInputFromForm(formIndex: formIndex);

            return await Task.FromResult(ResponseMsg);
        }
        public formValues GetAllInputFromForm(int formIndex = 1)
        {
            var nodes = Doc.DocumentNode.SelectNodes($"//form[{formIndex}]//input");
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
                            kv[name] = node.Attributes["value"]?.Value;
                        }
                        else
                        {
                            kv.Add(name, node.Attributes["value"]?.Value);
                        }
                    }
                }
            }
            return kv;
        }
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
    public class formValues : Dictionary<string, string>
    {

    }
}


