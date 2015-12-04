using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using DBC.Services;

namespace DBC.test.TestApplication
{
    public class TestMessageServices : IEmailSender
    {
        public Dictionary<string, TestHtmlEmail> TestHtmlEmail = new Dictionary<string, TestHtmlEmail>();

        public Task SendEmailAsync(string to, string subject, string message)
        {

            if (TestHtmlEmail.ContainsKey(to))
            {
                TestHtmlEmail[to] = new TestHtmlEmail
                {
                    Subject = subject,
                    Body = message,
                    Url = GetCode(message)
                };
            }
            else
            {
                TestHtmlEmail.Add(to,
                    new TestHtmlEmail
                    {
                        Subject = subject,
                        Body = message,
                        Url = GetCode(message)
                    }
                );
            }

            return Task.FromResult(0);
        }

        private string GetCode(string message)
        {
            var searchStr = "<a href=\"";
            var start = message.IndexOf(searchStr, StringComparison.Ordinal)+ searchStr.Length;
            if (start > searchStr.Length)
            {
                var end = message.IndexOf("\"", start , StringComparison.Ordinal);
                if (end > 0)
                    return WebUtility.HtmlDecode(message.Substring(start , end - start));
            }
            return "";
            
        }
    }

    public class TestHtmlEmail
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Url { get; set; }
    }
}