using System;
using System.Reflection;
using Microsoft.AspNet.Hosting;

namespace DBC.test.TestApplication
{
    public class TestHostingEnvironment : HostingEnvironment
    {

        private TestHostingEnvironment(string applicationRoot)
        {
            EnvironmentName = "Development";
            this.Initialize(applicationRoot, config: null);
        }

        public TestHostingEnvironment(Assembly assembly) : this(ApplicationRoot.GetDirectoryName(assembly))
        {
        }
        public TestHostingEnvironment(Type type) : this(type.Assembly)
        {
        }
    }
}