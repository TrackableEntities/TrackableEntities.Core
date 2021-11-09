using Microsoft.EntityFrameworkCore;
using TrackableEntities.EF.Core.Tests.NorthwindModels;

namespace TrackableEntities.EF.Core.Tests.Contexts
{
    public class NorthwindDbContext : DbContext
    {
        public NorthwindDbContext() { }

        public NorthwindDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Promo> Promos => Set<Promo>();
        public DbSet<ProductInfo> ProductInfos => Set<ProductInfo>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<CustomerAddress> CustomerAddresses => Set<CustomerAddress>();
        public DbSet<CustomerSetting> CustomerSettings => Set<CustomerSetting>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();
        public DbSet<Employee> Employees => Set<Employee>();  
        public DbSet<Territory> Territories => Set<Territory>();
        public DbSet<Area> Areas => Set<Area>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>()
                .HasOne(c => c.CustomerSetting)
                .WithOne(c => c.Customer);
            modelBuilder.Entity<Promo>().ToTable("Promos");
            modelBuilder.Entity<ProductInfo>()
                .HasKey(pi => new { pi.ProductInfoKey1, pi.ProductInfoKey2 });
            modelBuilder.Entity<Product>()
                .HasOne(p => p.ProductInfo)
                .WithOne(p => p.Product)
                .HasForeignKey<ProductInfo>(pi => pi.ProductId);
            modelBuilder.Entity<EmployeeTerritory>()
                .HasKey(et => new { et.EmployeeId, et.TerritoryId });
            modelBuilder.Entity<Area>().ToTable("Area");
        }
    }
}
