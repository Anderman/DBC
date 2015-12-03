// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace DBC.test.TestApplication
{
    public class MvcTestFixture<TStartup> : MvcTestFixture
        where TStartup : new()
    {
        public MvcTestFixture()
            : base(new TStartup())
        {
        }
    }
}
