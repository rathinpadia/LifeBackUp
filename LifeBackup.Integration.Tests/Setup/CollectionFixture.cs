using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace LifeBackup.Integration.Tests.Setup
{
    [CollectionDefinition("api")]
    public class CollectionFixture : ICollectionFixture<TestContext>
    {

    }
}
