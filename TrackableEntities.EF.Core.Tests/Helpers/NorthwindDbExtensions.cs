using TrackableEntities.EF.Core.Tests.Contexts;
using TrackableEntities.EF.Core.Tests.Mocks;

namespace TrackableEntities.EF.Core.Tests.Helpers
{
    internal static class NorthwindDbExtensions
    {
        public static void EnsureSeedData(this NorthwindDbContext context)
        {
            var model = new MockNorthwind();
            foreach (var category in model.Categories)
            {
                context.Categories.Add(category);
            }
            foreach (var product in model.Products)
            {
                context.Products.Add(product);
            }
            foreach (var customer in model.Customers)
            {
                context.Customers.Add(customer);
            }
            foreach (var order in model.Orders)
            {
                context.Orders.Add(order);
            }
            foreach (var area in model.Areas)
            {
                context.Areas.Add(area);
            }
            foreach (var territory in model.Territories)
            {
                context.Territories.Add(territory);
            }
            foreach (var employee in model.Employees)
            {
                context.Employees.Add(employee);
            }            
            context.SaveChanges();
        }
    }
}
