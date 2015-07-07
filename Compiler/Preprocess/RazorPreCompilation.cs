//using System;
//using Microsoft.AspNet.Mvc;

//namespace DBC.Compiler.Preprocess
//{
//    // Uncomment the following class to enable pre-compilation of Razor views.
//    // Pre-compilation may reduce the time it takes to build and launch your project.
//    // Please note, in this pre-release of Visual Studio 2015, enabling pre-compilation may cause IntelliSense and build errors in views using Tag Helpers.

//    public class RazorPreCompilation : RazorPreCompileModule
//    {
//        public RazorPreCompilation(IServiceProvider provider) : base(provider)
//        {
//            GenerateSymbols = true;
//        }
//    }
//}

// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.AspNet.Mvc
{
    /// <summary>
    /// Specifies that a property or parameter value should be initialized via the dependency injection
    /// container for activated types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class ActivateAttribute : Attribute
    {
    }
}