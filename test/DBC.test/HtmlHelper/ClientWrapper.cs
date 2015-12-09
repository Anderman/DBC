using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using DBC.test.TestApplication;
using Microsoft.Net.Http.Headers;

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
        public async Task<HttpResponseMessage> Get(string url)
        {
            ResponseMsg =  await SendRequest(HttpMethod.Get, url, null);

            return await Task.FromResult(ResponseMsg);
        }

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
            ResponseMsg  = await SendRequest(HttpMethod.Post, url, formValues);
            if (ResponseMsg.StatusCode != HttpStatusCode.OK)
            {
                File.WriteAllText($"error{++_errorNumber}.html", Html);
                File.WriteAllText($"error{_errorNumber}.txt", string.Join(
                    "\n", formValues.Select(a => $"'{a.Key}'='{a.Value}'")
                    ));
            }
            return await Task.FromResult(ResponseMsg);
        }

        private async Task<HttpResponseMessage> SendRequest(HttpMethod httpMethod, string url, FormValues formValues)
        {
            var request = new HttpRequestMessage(httpMethod, url);
            request.Headers.Add("Accept-Language", "en-US");
            request.Headers.Add("cookie", _cookies.Select(c => c.Key + "=" + c.Value.Value));
            if (formValues != null) request.Content = new FormUrlEncodedContent(formValues);
            ResponseMsg = await _client.SendAsync(request);
            _cookies.Add(ResponseMsg);
            if (ResponseMsg.Headers.Location != null)
            {
                return await Get(ResponseMsg.Headers.Location.OriginalString);
            }
            Html = await ResponseMsg.Content.ReadAsStringAsync();
            HtmlDocument = Html.StartsWith("<!") ? XDocument.Parse(Html) : Html.StartsWith("{") ? new XDocument() : XDocument.Parse("<root>" + Html + "</root>");
            return ResponseMsg;
        }

        public bool HasEmail(string emailAddress)
        {
            return _testMessageServices.TestHtmlEmail.ContainsKey(emailAddress);
        }
        public string ValidateResponse(string expectedRequestUrl)
        {
            var actualRequestUrl = ResponseMsg.RequestMessage.RequestUri.AbsolutePath;
            var err=HtmlDocument.ErrorMsg();
            if (!ResponseMsg.IsSuccessStatusCode || actualRequestUrl != expectedRequestUrl || err != null)
            {
                var body = HtmlDocument.InnerText();
                return $"expected url '{expectedRequestUrl}' != '{actualRequestUrl}'\n errmsg='{err}' \n body='{body}'";
            }
            return null;
        }

        public string ValidateForm(int index, object values)
        {
            var err= HtmlDocument.FormValues(index).HasCorrectValues(values);
            return string.IsNullOrWhiteSpace(err) ? null : err;
        }
        public string AbsolutePath => ResponseMsg.RequestMessage.RequestUri.AbsolutePath;

        public async Task<HttpResponseMessage> ClickOnLinkInEmail(string emailAddress)
        {
            var url = _testMessageServices.TestHtmlEmail[emailAddress].Url;
            return await Get(url);
        }

        public string GetSecurityCode(string emailAddress)
        {
            return _testMessageServices.TestHtmlEmail[emailAddress].Body.Split(':')?[1];
        }

        public SetCookieHeaderValue GetCookie(string cookieName)
        {
            return _cookies.ContainsKey(cookieName) ? _cookies[cookieName] : new SetCookieHeaderValue(cookieName);
        }
    }
}