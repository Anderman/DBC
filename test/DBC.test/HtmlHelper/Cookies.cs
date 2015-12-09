using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Net.Http.Headers;

namespace DBC.test.HtmlHelper
{
    public class Cookies: Dictionary<string, SetCookieHeaderValue>
    {
        public void Add(HttpResponseMessage response)
        {
            IEnumerable<string> setCookies;
            if (response.Headers.TryGetValues("Set-cookie", out setCookies))
            {
                foreach (var setCookie in setCookies)
                {
                    var cookie = SetCookieHeaderValue.Parse(setCookie);
                    if (this.ContainsKey(cookie.Name))
                    {
                        this[cookie.Name] = cookie;
                    }
                    else
                    {
                        this.Add(cookie.Name, cookie);
                    }
                }
            }
        }
    }
}