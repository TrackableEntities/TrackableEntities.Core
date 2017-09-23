using Microsoft.EntityFrameworkCore;
using TrackableEntities.EF.Core.Tests.NorthwindModels;

namespace TrackableEntities.EF.Core.Tests.Contexts
{
    public class NorthwindDbContext : DbContext
    {
        public NorthwindDbContext() { }

        public NorthwindDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Promo> Promos { get; set; }
        public DbSet<ProductInfo> ProductInfos { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerAddress> CustomerAddresses { get; set; }
        public DbSet<CustomerSetting> CustomerSettings { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Territory> Territories { get; set; }
        public DbSet<EmployeeTerritory> EmployeeTerritories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>()
                .HasOne(c => c.CustomerSetting)
                .WithOne(c => c.Customer);
            modelBuilder.Entity<Promo>().ToTable("Promos");
            modelBuilder.Entity<HolidayPromo>().ToTable("HolidayPromos");
            modelBuilder.Entity<EmployeeTerritory>()
                .HasKey(et => new { et.EmployeeId, et.TerritoryId });
            modelBuilder.Entity<ProductInfo>()
                .HasKey(pi => new { pi.ProductInfoKey1, pi.ProductInfoKey2 });
            modelBuilder.Entity<Product>()
                .HasOne(p => p.ProductInfo)
                .WithOne(p => p.Product);
        }
    }
}
