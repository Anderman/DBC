using System.Reflection;
using Microsoft.AspNet.Mvc.Infrastructure;

namespace DBC.test.TestApplication
{
    public class TestStaticAssemblyProvider : StaticAssemblyProvider
    {
        public TestStaticAssemblyProvider(Assembly assembly)
        {
            CandidateAssemblies.Add(assembly);
        }
    }
}