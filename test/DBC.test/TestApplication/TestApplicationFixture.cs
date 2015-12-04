// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Anderman.TagHelpers;
using DBC.Models.DB;
using DBC.Services;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace DBC.test.TestApplication
{
    public class TestApplicationFixture<TStartup> : TestFixture
        where TStartup : new()
    {
        public Mock<MessageServices> MockMessageServices;

        public TestApplicationFixture()
            : base(new TStartup())
        {
        }

        protected override void AddAdditionalServices(IServiceCollection services)
        {
            services.AddEntityFramework()
                .AddInMemoryDatabase()
                .AddDbContext<ApplicationDbContext>(o => o.UseInMemoryDatabase());

            services.AddIdentity<ApplicationUser, IdentityRole>(options => { options.SignIn.RequireConfirmedEmail = true; })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            services
                .AddMvc(m => { m.ModelMetadataDetailsProviders.Add(new AdditionalValuesMetadataProvider()); });
            LocalizationServiceCollectionJsonExtensions.AddLocalization(services);

            // Add application services.
            services.AddSingleton<IEmailSender, TestMessageServices>();
            services.AddSingleton<ISmsSender, MessageServices>();
            services.AddTransient<IEmailTemplate, EmailTemplate>();
        }
    }
}