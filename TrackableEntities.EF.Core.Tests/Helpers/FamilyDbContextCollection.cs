using Xunit;

namespace TrackableEntities.EF.Core.Tests.Helpers
{
    [CollectionDefinition("FamilyDbContext")]
    public class FamilyDbContextCollection : ICollectionFixture<FamilyDbContextFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
