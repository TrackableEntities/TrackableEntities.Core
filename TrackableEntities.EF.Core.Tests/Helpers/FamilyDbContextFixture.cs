using System;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TrackableEntities.EF.Core.Tests.Contexts;

namespace TrackableEntities.EF.Core.Tests.Helpers
{
    public class FamilyDbContextFixture : IDisposable
    {
        private FamilyDbContext? _context;
        private DbConnection? _connection;
        private DbContextOptions<FamilyDbContext>? _options;

        public void Initialize(bool useInMemory = true, Action? seedData = null)
        {
            if (useInMemory)
            {
                // In-memory database only exists while the connection is open
                _connection = new SqliteConnection("DataSource=:memory:");
                _connection.Open();
                _options = new DbContextOptionsBuilder<FamilyDbContext>()
                    .UseSqlite(_connection)
                    .Options;
            }
            else
            {
                _options = new DbContextOptionsBuilder<FamilyDbContext>()
                    .UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catelog=FamilyTest; Integrated Security=True; MultipleActiveResultSets=True")
                    .Options;
            }
            _context = new FamilyDbContext(_options);
            _context.Database.EnsureCreated();
            seedData?.Invoke();
        }

        public FamilyDbContext GetContext()
        {
            if (_context == null)
                throw new InvalidOperationException("You must first call Initialize before getting the context.");
            return _context;
        }

        public void Dispose()
        {
            _connection?.Close();
        }
    }
}
