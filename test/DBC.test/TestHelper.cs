using System;
using System.IO;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.TestHost;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Runtime;
using Microsoft.Framework.Runtime.Infrastructure;

// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace DBC.test
{
    public static class TestHelper
    {
        // Path from Mvc\\test\\Microsoft.AspNet.Mvc.FunctionalTests

        public static TestServer SetupServer(string applicationWebSiteName, Func<Func<Task>, HttpContext, Task> testMiddlewareFunc)
        {
            var currentHostingEnvironment = GetCurrentHostingEnvironment(applicationWebSiteName);
            Action<IApplicationBuilder> appBuilder = new Startup(currentHostingEnvironment).SetupRequestPipeline;
            Action<IServiceCollection> configureServices = new Startup(currentHostingEnvironment).ConfigureServices;
            return TestHelper.CreateServer(app =>
                AddTestUses(app, appBuilder, testMiddlewareFunc),
                applicationWebSiteName, "../src", configureServices);
        }

        public static HostingEnvironment GetCurrentHostingEnvironment(string applicationWebSiteName)
        {
            //var hostingEnvironment = new HostingEnvironment(GetCurrentApplicationEnvironment(applicationWebSiteName));
            var env = GetCurrentApplicationEnvironment(applicationWebSiteName);
            var hostingEnvironment = new HostingEnvironment();
            hostingEnvironment.Initialize(env.ApplicationBasePath, environmentName: null);
            return hostingEnvironment;
        }

        public static TestApplicationEnvironment GetCurrentApplicationEnvironment(string applicationWebSiteName)
        {
            var curPath = Environment.CurrentDirectory;
            var path = Path.GetFullPath(Path.Combine(curPath, "..", "src", applicationWebSiteName));
            return new TestApplicationEnvironment(path, "DBC", "1.0.0", "DEBUG",
                new FrameworkName("DNX,Version=v4.5.1"));
        }

        public static TestServer CreateServer(Action<IApplicationBuilder> builder, string applicationWebSiteName,
            string applicationPath)
        {
            return CreateServer(builder, applicationWebSiteName, applicationPath, (Action<IServiceCollection>) null);
        }

        public static TestServer CreateServer(Action<IApplicationBuilder> builder, string applicationWebSiteName,
            string applicationPath, Action<IServiceCollection> configureServices)
        {
            return TestServer.Create(builder,
                services => AddTestServices(services, applicationWebSiteName, applicationPath, configureServices));
        }

        public static TestServer CreateServer(Action<IApplicationBuilder> builder, string applicationWebSiteName,
            string applicationPath, Func<IServiceCollection, IServiceProvider> configureServices)
        {
            return TestServer.Create(CallContextServiceLocator.Locator.ServiceProvider, builder,
                services =>
                {
                    AddTestServices(services, applicationWebSiteName, applicationPath, null);
                    return (configureServices != null) ? configureServices(services) : services.BuildServiceProvider();
                });
        }

        private static void AddTestUses(IApplicationBuilder app, Action<IApplicationBuilder> builder, Func<Func<Task>, HttpContext, Task> getValue)
        {
            app.Use(async (context, next) =>
            {
                await getValue(next, context);
            });
            builder?.Invoke(app);
        }
        private static void AddTestServices(IServiceCollection services, string applicationWebSiteName,
            string applicationPath, Action<IServiceCollection> configureServices)
        {
            // Get current IApplicationEnvironment; likely added by the host.
            var provider = services.BuildServiceProvider();
            var originalEnvironment = provider.GetRequiredService<IApplicationEnvironment>();

            // When an application executes in a regular context, the application base path points to the root
            // directory where the application is located, for example MvcSample.Web. However, when executing
            // an application as part of a test, the ApplicationBasePath of the IApplicationEnvironment points
            // to the root folder of the test project.
            // To compensate for this, we need to calculate the original path and override the application
            // environment value so that components like the view engine work properly in the context of the
            // test.
            var applicationBasePath = CalculateApplicationBasePath(originalEnvironment, applicationWebSiteName,
                applicationPath);
            var environment = new TestApplicationEnvironment(originalEnvironment, applicationBasePath,
                applicationWebSiteName);
            services.AddInstance<IApplicationEnvironment>(environment);
            var hostingEnvironment = new HostingEnvironment();
            hostingEnvironment.Initialize(applicationBasePath, environmentName: null);
            services.AddInstance<IHostingEnvironment>(hostingEnvironment);
            //beta 6?
            //services.AddInstance<IHostingEnvironment>(new HostingEnvironment(environment)); 

            // Injecting a custom assembly provider. Overrides AddMvc() because that uses TryAdd().
            var assemblyProvider = CreateAssemblyProvider(applicationWebSiteName);
            services.AddInstance(assemblyProvider);

            configureServices?.Invoke(services);
        }

        // Calculate the path relative to the application base path.
        private static string CalculateApplicationBasePath(
            IApplicationEnvironment appEnvironment,
            string applicationWebSiteName,
            string websitePath)
        {
            // Mvc/test/WebSites/applicationWebSiteName
            return Path.GetFullPath(
                Path.Combine(appEnvironment.ApplicationBasePath, websitePath, applicationWebSiteName));
        }

        public static IAssemblyProvider CreateAssemblyProvider(string siteName)
        {
            // Creates a service type that will limit MVC to only the controllers in the test site.
            // We only want this to happen when running in-process.
            var assembly = Assembly.Load(new AssemblyName(siteName));
            var provider = new FixedSetAssemblyProvider
            {
                CandidateAssemblies =
                {
                    assembly
                }
            };

            return provider;
        }
    }
}