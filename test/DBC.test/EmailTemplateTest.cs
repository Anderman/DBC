//using System;
//using System.IO;
//using System.Reflection;
//using System.Runtime.Versioning;
//using DBC.Services;
//using Microsoft.AspNet.Hosting;
//using Microsoft.AspNet.Http;
//using Microsoft.AspNet.Http.Internal;
//using Microsoft.AspNet.Mvc;
//using Microsoft.AspNet.Mvc.Infrastructure;
//using Microsoft.AspNet.Mvc.Razor;
//using Microsoft.AspNet.Mvc.Rendering;
//using Microsoft.AspNet.Mvc.ViewEngines;
//using Microsoft.Framework.DependencyInjection;
//using Microsoft.Framework.OptionsModel;
//using Moq;
//using Xunit;

//namespace DBC.test
//{
//    public class EmailTemplateTest
//    {
//        public void test()
//        {
//            IServiceCollection services = new ServiceCollection();

//            var currentHostingEnvironment = TestHelper.GetCurrentHostingEnvironment(nameof(DBC));
//            new Startup(currentHostingEnvironment).ConfigureServices(services);
//            var assemblyProvider = TestHelper.CreateAssemblyProvider(nameof(DBC));
//            services.AddInstance(assemblyProvider);
//            services.AddInstance(GetLoadContextAccessor());
//            services.AddInstance(TestHelper.GetCurrentApplicationEnvironment(nameof(DBC)));

//            IServiceProvider serviceProvider = services.BuildServiceProvider();
//            ITypeActivatorCache typeActivatorCache = services.BuildServiceProvider().GetRequiredService<ITypeActivatorCache>();//new DefaultTypeActivatorCache();
//            IOptions<MvcOptions> optionsAccessor = services.BuildServiceProvider().GetRequiredService<IOptions<MvcOptions>>();
//            IHttpContextAccessor c = services.BuildServiceProvider().GetRequiredService<IHttpContextAccessor>();
//            //IViewStartProvider viewEngineProvider = services.BuildServiceProvider().GetRequiredService<IViewEngineProvider>();
//            ICompositeViewEngine a = services.BuildServiceProvider().GetRequiredService<ICompositeViewEngine>();
//            //var optionsSetup = new MvcOptionsSetup();
//            //var options = new MvcOptions();
//            //optionsSetup.Configure(options);
//            //IOptions<MvcOptions> optionsAccessor = new MyOptions<MvcOptions>(options);
//            //IHttpContextAccessor c = new MyHttpContextAccessor();
//            //IViewEngineProvider viewEngineProvider = new DefaultViewEngineProvider(optionsAccessor, typeActivatorCache, serviceProvider);
//            //ICompositeViewEngine a = new CompositeViewEngine(viewEngineProvider);
//            var b = new EmailTemplate(a, c);
//        }

//        public class MyHttpContextAccessor : IHttpContextAccessor
//        {
//            public MyHttpContextAccessor()
//            {
//                HttpContext = new DefaultHttpContext();
//            }
//            public HttpContext HttpContext { get; set; }
//        }
//        public class MyOptions<TOptions> : IOptions<TOptions> where TOptions : class, new()
//        {
//            public MyOptions(TOptions options)
//            {
//                Options = options;
//            }
//            public TOptions GetNamedOptions(string name)
//            {
//                throw new NotImplementedException();
//            }

//            public TOptions Options { get; }
//        }
//        private static IAssemblyLoadContextAccessor GetLoadContextAccessor()
//        {
//            var loadContext = new Mock<IAssemblyLoadContext>();
//            loadContext.Setup(s => s.LoadStream(It.IsAny<Stream>(), It.IsAny<Stream>()))
//                       .Returns((Stream stream, Stream pdb) =>
//                       {
//                           var memoryStream = (MemoryStream)stream;
//                           return Assembly.Load(memoryStream.ToArray());
//                       });

//            var accessor = new Mock<IAssemblyLoadContextAccessor>();
//            accessor.Setup(a => a.GetLoadContext(typeof(RoslynCompilationService).Assembly))
//                    .Returns(loadContext.Object);
//            return accessor.Object;
//        }
//        private IApplicationEnvironment GetApplicationEnvironment()
//        {
//            var applicationEnvironment = new Mock<IApplicationEnvironment>();
//            applicationEnvironment.SetupGet(a => a.ApplicationName)
//                                  .Returns("MyApp");
//            applicationEnvironment.SetupGet(a => a.RuntimeFramework)
//                                  .Returns(new FrameworkName("ASPNET", new Version(4, 5, 1)));
//            applicationEnvironment.SetupGet(a => a.Configuration)
//                                  .Returns("Debug");
//            applicationEnvironment.SetupGet(a => a.ApplicationBasePath)
//                                  .Returns("MyBasePath");

//            return applicationEnvironment.Object;
//        }
//    }
//}