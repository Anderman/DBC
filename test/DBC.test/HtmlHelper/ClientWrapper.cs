using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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

        public string GetSecurityCode(string emailAddress)
        {
            return _testMessageServices.TestHtmlEmail[emailAddress].Body.Split(':')?[1];
        }
    }
}