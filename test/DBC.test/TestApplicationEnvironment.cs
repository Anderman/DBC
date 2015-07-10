// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Runtime.Versioning;
using Microsoft.Framework.Runtime;

namespace DBC.test
{
    // Represents an application environment that overrides the base path of the original
    // application environment in order to make it point to the folder of the original web
    // aplication so that components like ViewEngines can find views as if they were executing
    // in a regular context.
    public class TestApplicationEnvironment : IApplicationEnvironment
    {
        public TestApplicationEnvironment(IApplicationEnvironment originalAppEnvironment, string appBasePath,
            string appName)
        {
            ApplicationBasePath = appBasePath;
            ApplicationName = appName;
            Version = originalAppEnvironment.Version;
            Configuration = originalAppEnvironment.Configuration;
            RuntimeFramework = originalAppEnvironment.RuntimeFramework;
        }

        public TestApplicationEnvironment(string appBasePath, string appName, string version, string configuration,
            FrameworkName runtimeFramework)
        {
            ApplicationBasePath = appBasePath;
            ApplicationName = appName;
            Version = version;
            Configuration = configuration;
            RuntimeFramework = runtimeFramework;
        }

        public string ApplicationName { get; }
        public string Version { get; }
        public string ApplicationBasePath { get; }
        public string Configuration { get; }
        public FrameworkName RuntimeFramework { get; }
    }
}