using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CodeComb.HtmlAgilityPack;
using Microsoft.AspNet.Http;
namespace DBC.test.HtmlHelper
{
    public class ClientWrapper
    {
        public string Html;
        private HttpClient _client;
        public HtmlDocument Doc = new HtmlDocument();
        public HttpResponseMessage responseMsg;
        private formValues Cookies = new formValues();
        private formValues allInputs;

        public ClientWrapper(HttpClient client)
        {
            _client = client;
        }
        public async Task<HttpResponseMessage> Post(string url,int formIndex, formValues values)
        {
            //var list = defaults.Select(k =>  new KeyValuePair<string, string>(k.Key, k.Value.ToString())).ToList();
            //var l = allInputs.ToDictionary(x => x.Key, x => x.Value);
            foreach(var value in values)
            {
                allInputs[value.Key] = value.Value;
            }
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            foreach (var cookie in Cookies)
            {
                request.Headers.Add("Cookie", cookie.Key + "=" + cookie.Value);
            }
            request.Content = new FormUrlEncodedContent(allInputs);
            responseMsg = await _client.SendAsync(request);
            if (responseMsg.Headers.Location != null)
            {
                return await Get(responseMsg.Headers.Location.OriginalString, formIndex);
            }
            else
            {
                Html = await responseMsg.Content.ReadAsStringAsync();
                Doc.LoadHtml(Html);

                AddCookies(responseMsg);
                allInputs = GetAllInputFromForm(formIndex: formIndex);

                return await Task.FromResult(responseMsg);
            }
        }
        public async Task<HttpResponseMessage> Get(string url, int formIndex)
        {
            responseMsg = await _client.GetAsync(url);
            Html = await responseMsg.Content.ReadAsStringAsync();
            Doc.LoadHtml(Html);
            AddCookies(responseMsg);

            allInputs = GetAllInputFromForm(formIndex: formIndex);

            return await Task.FromResult(responseMsg);
        }
        public formValues GetAllInputFromForm(int formIndex = 1)
        {
            var nodes = Doc.DocumentNode.SelectNodes($"//form[{formIndex}]//input");
            var kv = new formValues();
            if (nodes!=null)
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
                    if (Cookies.ContainsKey(cookiekv[0]))
                    {
                        Cookies[cookiekv[0]] = cookiekv[1];
                    }
                    else
                    {
                        Cookies.Add(cookiekv[0], cookiekv[1]);
                    }
                }
            }
        }

    }
    public class formValues : Dictionary<string, string>
    {

    }
}


