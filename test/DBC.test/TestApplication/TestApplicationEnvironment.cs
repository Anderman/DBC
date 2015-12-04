// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Reflection;
using System.Runtime.Versioning;
using Microsoft.Extensions.PlatformAbstractions;

namespace DBC.test.TestApplication
{
    // An application environment that overrides the base path of the original
    // application environment in order to make it point to the folder of the original web
    // aaplication so that components like ViewEngines can find views as if they were executing
    // in a regular context.
    public class TestApplicationEnvironment : IApplicationEnvironment
    {
        private readonly IApplicationEnvironment _original;

        public TestApplicationEnvironment(Assembly startupAssembly) : this(
            PlatformServices.Default.Application,
            startupAssembly.GetName().Name,
            ApplicationRoot.GetDirectoryName(startupAssembly))
        {
        }

        public TestApplicationEnvironment(IApplicationEnvironment original, string name, string basePath)
        {
            _original = original;
            ApplicationName = name;
            ApplicationBasePath = basePath;
        }

        public string ApplicationName { get; }
        public string ApplicationBasePath { get; }

        public string ApplicationVersion =>
            _original.ApplicationVersion;

        public string Configuration =>
            _original.Configuration;

        public FrameworkName RuntimeFramework =>
            _original.RuntimeFramework;

        public object GetData(string name)
        {
            return _original.GetData(name);
        }

        public void SetData(string name, object value)
        {
            _original.SetData(name, value);
        }
    }
}