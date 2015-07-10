using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DBC
{
    public class AppSettings
    {
        public string SiteTitle { get; set; }
        public string CssFallbackPath { get; set; } = "/fallback/css/css/";
        public string JsFallbackPath { get; set; } = "/fallback/js/";
    }
}
