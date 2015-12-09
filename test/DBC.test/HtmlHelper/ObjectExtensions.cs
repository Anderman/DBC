using System.Linq;
using System.Reflection;

namespace DBC.test.HtmlHelper
{
    public static class ObjectExtensions
    {
        public static FormValues AsFormValues(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            var formValues = source as FormValues;
            if (formValues == null)
            {
                formValues = new FormValues();
                foreach (var u in source.GetType().GetProperties(bindingAttr).Where(p => p.GetValue(source, null) != null))
                {
                    formValues.Add(u.Name, u.GetValue(source, null).ToString());
                }
            }
            return formValues;
        }
    }
}