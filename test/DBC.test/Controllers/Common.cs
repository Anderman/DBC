using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DBC.test.HtmlHelper;
using DBC.ViewModels.Account;

namespace DBC.test.Controllers
{
    public static class ClientExtension
    {
        public static bool Login(this ClientWrapper client, string email, string password)
        {
            var resonse = client.Get("/Account/Login").Result;
            resonse = client.Post(2,
                new LoginViewModel
                {
                    Email = email,
                    Password = password,
                    RememberMe = true
                }).Result;
            return true;
        }
    }
}
