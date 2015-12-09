using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DBC.test.HtmlHelper
{
    public class FormValues : Dictionary<string, string>
    {
        public string HasCorrectValues(object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            foreach (var u in source.GetType().GetProperties(bindingAttr).Where(p => p.GetValue(source, null) != null))
            {
                if (!ContainsKey(u.Name))
                    return $"Form input '{u.Name}' is missing";

                var actual = u.GetValue(source, null);
                var expected = this[u.Name];
                if (!(actual is string))
                {
                    actual = actual.ToString().ToLowerInvariant();
                    expected = expected.ToLowerInvariant();
                }
                if (actual.ToString() != expected)
                {
                    return $"{u.Name}={actual} expected:{expected}";
                }
            }
            return "";
        }
    }
}