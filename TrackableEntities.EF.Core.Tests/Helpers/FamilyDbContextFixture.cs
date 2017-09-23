using System;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TrackableEntities.EF.Core.Tests.Contexts;

namespace TrackableEntities.EF.Core.Tests.Helpers
{
    public class FamilyDbContextFixture : IDisposable
    {
        private readonly DbConnection _connection;
        private readonly DbContextOptions<FamilyDbContext> _options;

        public FamilyDbContextFixture()
        {
            // In-memory database only exists while the connection is open
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            _options = new DbContextOptionsBuilder<FamilyDbContext>()
                .UseSqlite(_connection)
                .Options;
        }

        public FamilyDbContext GetContext()
        {
            var context = new FamilyDbContext(_options);
            context.Database.EnsureCreated();
            return context;
        }

        public void Dispose()
        {
            _connection.Close();
        }
    }
}
