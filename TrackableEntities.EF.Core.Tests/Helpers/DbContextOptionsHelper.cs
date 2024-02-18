using System;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace TrackableEntities.EF.Core.Tests.Helpers
{
    public static class DbContextOptionsHelper
    {
        public static DbContextOptions<TContext> GetContextOptions<TContext>(string initialCatalog)
            where TContext : DbContext
        {
            // default connection string for tests 'LocalDb'
            var connectionString =
                @"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=FamilyTest; Integrated Security=True; MultipleActiveResultSets=True";

            if (Environment.GetEnvironmentVariable("ConnectionString") != null)
            {
                // LocalDb isn't supported in Linux. To support running tests, allow env variable for conn string
                connectionString = Environment.GetEnvironmentVariable("ConnectionString");
            }

            var builder = new SqlConnectionStringBuilder(connectionString) { InitialCatalog = initialCatalog };

            return new DbContextOptionsBuilder<TContext>()
                .UseSqlServer(builder.ConnectionString)
                .Options;
        }
    }
}