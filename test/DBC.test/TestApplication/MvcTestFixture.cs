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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Microsoft.Extensions.PlatformAbstractions;

namespace DBC.test.TestApplication
{
    public class MvcTestFixture : IDisposable
    {
        private readonly TestServer _server;

        public MvcTestFixture(object startupInstance)
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
                    configureStartup(application, GetHostingEnvironment(startupTypeInfo), new LoggerFactory());



            // RequestLocalizationOptions saves the current culture when constructed, potentially changing response
            // localization i.e. RequestLocalizationMiddleware behavior. Ensure the saved culture
            // (DefaultRequestCulture) is consistent regardless of system configuration or personal preferences.
            _server = TestServer.Create(
                configureApplication,
                configureServices: InitializeServices(startupTypeInfo.Assembly, buildServices));

            Client = _server.CreateClient();
            Client.BaseAddress = new Uri("http://localhost");
        }

        public HttpClient Client { get; }

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
            var applicationName = startupAssembly.GetName().Name;
            var applicationRoot = GetApplicationRoot(applicationName);

            var applicationEnvironment = PlatformServices.Default.Application;

            return (services) =>
            {
#if DNX451
                AppDomain.CurrentDomain.SetData("APP_CONTEXT_BASE_DIRECTORY", applicationRoot);
#endif
                services.AddInstance<IApplicationEnvironment>(
                    new TestApplicationEnvironment(applicationEnvironment, applicationName, applicationRoot));

                var hostingEnvironment = GetHostingEnvironment(applicationRoot);
                services.AddInstance<IHostingEnvironment>(hostingEnvironment);

                // Inject a custom assembly provider. Overrides AddMvc() because that uses TryAdd().
                var assemblyProvider = GetStaticAssemblyProvider(startupAssembly);
                services.AddInstance<IAssemblyProvider>(assemblyProvider);

                AddAdditionalServices(services);

                return buildServices(services);
            };
        }

        private static StaticAssemblyProvider GetStaticAssemblyProvider(Assembly assembly)
        {
            var assemblyProvider = new StaticAssemblyProvider();
            assemblyProvider.CandidateAssemblies.Add(assembly);
            return assemblyProvider;
        }


        private static HostingEnvironment GetHostingEnvironment(Type type)
        {
            return GetHostingEnvironment(type.Assembly);
        }

        private static HostingEnvironment GetHostingEnvironment(Assembly assembly)
        {
            return GetHostingEnvironment(GetApplicationRoot(assembly));
        }

        private static HostingEnvironment GetHostingEnvironment(string applicationRoot)
        {
            var hostingEnvironment = new HostingEnvironment() { EnvironmentName = "Testing" };
            hostingEnvironment.Initialize(applicationRoot, config: null);
            return hostingEnvironment;
        }

        public static string GetApplicationRoot(Assembly assembly) =>
            GetApplicationRoot(assembly.GetName().Name);
        public static string GetApplicationRoot(string applicationName) =>
            Path.GetDirectoryName(PlatformServices.Default.LibraryManager.GetLibrary(applicationName).Path);

    }
}
