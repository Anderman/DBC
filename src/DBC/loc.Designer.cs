namespace DBC
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Xml.Linq;

    public class loc
    {
        private static System.Globalization.CultureInfo resourceCulture;
        public static System.Globalization.CultureInfo Culture
        {
            get
            {
                return resourceCulture;
            }
            set

            {
                resourceCulture = value;
            }
        }
        public static string Email => "E-mail";
        public static string RememberThisBrowser => "Vraag niet meer om een Beveligingscode?";
        public static string Code => "Code";
        public static string Password => "Wachtwoord";
        public static string ConfirmPassword => "Bevestig wachtwoord";
        public static string RememberMe => "Onthoud mijn login";
        public static string TheMustBeAtLeastCharactersLong => "Het {0} moet minimaal {2} tekens lang zijn.";
        public static string ThePasswordAndConfirmationPasswordDoNotMatch => "De ingevoerde wachtwoorden komen niet met elkaar overeen.";

        public static string GetString(string org) 
        {
            string value;
            return TranslationTable.TryGetValue(org, out value) ? value : org;
        }

        private static Dictionary<string, string> TranslationTable = new Dictionary<string, string>()
        {
            ["Forgot your password?"] = "Wachtwoord vergeten?",
            ["Confirm your account"]= "Bevestig uw inlog gegevens",
            ["Invalid login attempt."] = "Ongeldige inlog poging"
        };
    }

    public static class Extensions
    {
        public static String Localize(this String text)
        {
            return loc.GetString(text);
        }
        // helper so that you can use ForEach on any IEnumerable
        public static void ForEach<T>(this IEnumerable<T> values, Action<T> action)
        {
            foreach (var v in values)
            {
                action(v);
            }
        }
    }
}
