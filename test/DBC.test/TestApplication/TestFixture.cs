// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc.Infrastructure;
using Microsoft.AspNet.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Microsoft.Extensions.PlatformAbstractions;

namespace DBC.test.TestApplication
{
    public class TestFixture : IDisposable
    {
        private readonly TestServer _server;
        public readonly object Server;
        public HttpClient Client { get; }

        public TestFixture(object startupInstance)
        {
            var startupTypeInfo = startupInstance.GetType().GetTypeInfo();

            var buildServices = (Func<IServiceCollection, IServiceProvider>)startupTypeInfo
                .DeclaredMethods
                .FirstOrDefault(m => m.Name == "ConfigureServices" && m.ReturnType == typeof(IServiceProvider))
                ?.CreateDelegate(typeof(Func<IServiceCollection, IServiceProvider>), startupInstance);

            var configureStartup = (Action<IApplicationBuilder, IHostingEnvironment, ILoggerFactory>)startupTypeInfo
                    .DeclaredMethods
                    .FirstOrDefault(m => m.Name == "Configure" && m.GetParameters().Length == 3)
                    ?.CreateDelegate(typeof(Action<IApplicationBuilder, IHostingEnvironment, ILoggerFactory>), startupInstance);
            Action<IApplicationBuilder> configureApplication = application =>
                    configureStartup(application, new TestHostingEnvironment(startupTypeInfo), new LoggerFactory());

            _server = TestServer.Create(
                configureApplication,
                configureServices: InitializeServices(startupTypeInfo.Assembly, buildServices));
            Server = startupInstance;
            Client = _server.CreateClient();
            Client.BaseAddress = new Uri("http://localhost");
        }


        public void Dispose()
        {
            Client.Dispose();
            _server.Dispose();
        }

        protected virtual void AddAdditionalServices(IServiceCollection services)
        {
        }

        private Func<IServiceCollection, IServiceProvider> InitializeServices(
            Assembly startupAssembly,
            Func<IServiceCollection, IServiceProvider> buildServices)
        {

            // When an application executes in a regular context, the application base path points to the root
            // directory where the application is located, for example .../samples/MvcSample.Web. However, when
            // executing an application as part of a test, the ApplicationBasePath of the IApplicationEnvironment
            // points to the root folder of the test project.
            // To compensate, we need to calculate the correct project path and override the application
            // environment value so that components like the view engine work properly in the context of the test.
            return (services) =>
            {

#if DNX451
                AppDomain.CurrentDomain.SetData("APP_CONTEXT_BASE_DIRECTORY", ApplicationRoot.GetDirectoryName(startupAssembly));
#endif
                services.AddInstance<IApplicationEnvironment>(new TestApplicationEnvironment(startupAssembly));

                services.AddInstance<IHostingEnvironment>(new TestHostingEnvironment(startupAssembly));

                // Inject a custom assembly provider. Overrides AddMvc() because that uses TryAdd().
                //services.AddInstance<IAssemblyProvider>(new TestStaticAssemblyProvider(startupAssembly));

                AddAdditionalServices(services);

                return buildServices(services);
            };
        }
    }

    public static class ApplicationRoot
    {
        public static string GetDirectoryName(Assembly assembly) =>
            Path.GetDirectoryName(PlatformServices.Default.LibraryManager.GetLibrary(assembly.GetName().Name).Path);
    }
}
