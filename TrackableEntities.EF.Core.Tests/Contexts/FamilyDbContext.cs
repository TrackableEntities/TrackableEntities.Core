using Microsoft.EntityFrameworkCore;
using TrackableEntities.EF.Core.Tests.FamilyModels;

namespace TrackableEntities.EF.Core.Tests.Contexts
{
    public class FamilyDbContext : DbContext
    {
        public FamilyDbContext() { }

        public FamilyDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Parent> Parents => Set<Parent>();
        public DbSet<Child> Children => Set<Child>();
    }
}
