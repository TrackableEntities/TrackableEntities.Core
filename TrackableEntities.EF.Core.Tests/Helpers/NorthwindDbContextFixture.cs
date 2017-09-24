using System;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TrackableEntities.EF.Core.Tests.Contexts;

namespace TrackableEntities.EF.Core.Tests.Helpers
{
    public class NorthwindDbContextFixture : IDisposable
    {
        private readonly DbConnection _connection;
        private readonly DbContextOptions<NorthwindDbContext> _options;

        public NorthwindDbContextFixture()
        {
            // In-memory database only exists while the connection is open
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            _options = new DbContextOptionsBuilder<NorthwindDbContext>()
                .UseSqlite(_connection)
                .Options;
        }

        public NorthwindDbContext GetContext(bool seedData = false)
        {
            var context = new NorthwindDbContext(_options);
            context.Database.EnsureCreated();
            if (seedData)
                context.EnsureSeedData();
            return context;
        }

        public void Dispose()
        {
            _connection.Close();
        }
    }
}
