using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TrackableEntities.Common.Core;
using TrackableEntities.EF.Core.Tests.Contexts;
using TrackableEntities.EF.Core.Tests.Helpers;
using TrackableEntities.EF.Core.Tests.NorthwindModels;
using Xunit;

namespace TrackableEntities.EF.Core.Tests
{
    [Collection("NorthwindDbContext")]
    public class LoadRelatedEntitiesTests
    {
        private const string TestCustomerId1 = "AAAA";
        private const string TestCustomerId2 = "BBBB";
        private const string TestTerritoryId1 = "11111";
        private const string TestTerritoryId2 = "22222";
        private const string TestTerritoryId3 = "33333";
        private const int TestArea1 = 1;
        private const int TestArea2 = 2;
        private const int ProductInfo1A = 1;
        private const int ProductInfo1B = 2;
        private const int ProductInfo2A = 1;
        private const int ProductInfo2B = 3;

        private readonly NorthwindDbContextFixture _fixture;

        public LoadRelatedEntitiesTests(NorthwindDbContextFixture fixture)
        {
            _fixture = fixture;
            _fixture.Initialize(false, EnsureSeedData);
        }

        #region Setup

        private void EnsureSeedData()
        {
            // Test Customers
            var context = _fixture.GetContext();
            EnsureTestCustomer(context, TestCustomerId1, TestTerritoryId1);
            EnsureTestCustomer(context, TestCustomerId2, TestTerritoryId2);

            // Test Customer Settings
            EnsureTestCustomerSetting(context, TestCustomerId1);
            EnsureTestCustomerSetting(context, TestCustomerId2);

            EnsureTestArea(context, TestArea1);
            EnsureTestArea(context, TestArea2);

            // Test Territories
            EnsureTestTerritory(context, TestTerritoryId1);
            EnsureTestTerritory(context, TestTerritoryId2);
            EnsureTestTerritory(context, TestTerritoryId3);

            // Test Product Infos
            //EnsureTestProductInfo(context, ProductInfo1A, ProductInfo1B);
            //EnsureTestProductInfo(context, ProductInfo2A, ProductInfo2B);

            // Save changes
            context.SaveChanges();
        }

        private static void EnsureTestArea(NorthwindDbContext context, int areaId)
        {
            var area = context.Areas.SingleOrDefault(a => a.AreaId == areaId);
            if (area == null)
            {
                area = new Area { AreaId = areaId, AreaName = "Test Area " + areaId.ToString() };
                context.Areas.Add(area);
            }
        }

        private static void EnsureTestTerritory(NorthwindDbContext context, string territoryId)
        {
            var territory = context.Territories
                .SingleOrDefault(t => t.TerritoryId == territoryId);
            if (territory == null)
            {
                territory = new Territory { TerritoryId = territoryId, TerritoryDescription = "Test Territory " + territoryId };
                context.Territories.Add(territory);
            }
        }

        private static void EnsureTestCustomer(NorthwindDbContext context, string customerId, string territoryId)
        {
            var customer = context.Customers
                .SingleOrDefault(c => c.CustomerId == customerId);
            if (customer == null)
            {
                customer = new Customer
                {
                    CustomerId = customerId,
                    CustomerName = "Test Customer " + customerId,
                    TerritoryId = territoryId
                };
                context.Customers.Add(customer);
            }
        }

        private static void EnsureTestCustomerSetting(NorthwindDbContext context, string customerId)
        {
            var setting = context.CustomerSettings
                .SingleOrDefault(c => c.CustomerId == customerId);
            if (setting == null)
            {
                setting = new CustomerSetting { CustomerId = customerId, Setting = "Setting1" };
                context.CustomerSettings.Add(setting);
            }
        }

        private static void EnsureTestProductInfo(NorthwindDbContext context, int productInfo1, int productInfo2)
        {
            var info = context.ProductInfos
                .SingleOrDefault(pi => pi.ProductInfoKey1 == productInfo1
                    && pi.ProductInfoKey2 == productInfo2);
            if (info == null)
            {
                info = new ProductInfo
                {
                    ProductInfoKey1 = productInfo1,
                    ProductInfoKey2 = productInfo2,
                    Info = "Test Product Info"
                };
                context.ProductInfos.Add(info);
            }
        }

        private List<Order> CreateTestOrders(NorthwindDbContext context)
        {
            // Create test entities
            var category1 = new Category
            {
                CategoryName = "Test Category 1"
            };
            var category2 = new Category
            {
                CategoryName = "Test Category 2"
            };
            var product1 = new Product
            {
                ProductName = "Test Product 1",
                UnitPrice = 10M,
                Category = category1
            };
            var product2 = new Product
            {
                ProductName = "Test Product 2",
                UnitPrice = 20M,
                Category = category2
            };
            var product3 = new Product
            {
                ProductName = "Test Product 3",
                UnitPrice = 30M,
                Category = category2
            };
            var customer1 = context.Customers
                .Include(c => c.Territory)
                .Include(c => c.CustomerSetting)
                .Single(c => c.CustomerId == TestCustomerId1);
            var customer2 = context.Customers
                .Include(c => c.Territory)
                .Include(c => c.CustomerSetting)
                .Single(c => c.CustomerId == TestCustomerId1);
            var detail1 = new OrderDetail { Product = product1, Quantity = 11, UnitPrice = 11M };
            var detail2 = new OrderDetail { Product = product2, Quantity = 12, UnitPrice = 12M };
            var detail3 = new OrderDetail { Product = product2, Quantity = 13, UnitPrice = 13M };
            var detail4 = new OrderDetail { Product = product3, Quantity = 14, UnitPrice = 14M };
            var order1 = new Order
            {
                OrderDate = DateTime.Today,
                Customer = customer1,
                OrderDetails = new List<OrderDetail>
                {
                    detail1,
                    detail2,
                }
            };
            var order2 = new Order
            {
                OrderDate = DateTime.Today,
                Customer = customer2,
                OrderDetails = new List<OrderDetail>
                {
                    detail3,
                    detail4,
                }
            };

            // Persist entities
            context.Orders.Add(order1);
            context.Orders.Add(order2);
            context.SaveChanges();

            // Detach entities
            context.DetachEntities(order1);
            context.DetachEntities(order2);

            // Clear reference properties
            product1.Category = null;
            product2.Category = null;
            product3.Category = null;
            customer1.Territory = null;
            customer2.Territory = null;
            customer1.CustomerSetting = null;
            customer2.CustomerSetting = null;
            detail1.Product = null;
            detail2.Product = null;
            detail3.Product = null;
            detail4.Product = null;
            order1.OrderDetails = new List<OrderDetail> { detail1, detail2 };
            order2.OrderDetails = new List<OrderDetail> { detail3, detail4 };

            // Return orders
            return new List<Order> { order1, order2 };
        }

        //private List<Employee> CreateTestEmployees(NorthwindDbContext context)
        //{
        //    // Create test entities
        //    var area1 = new Area { AreaName = "Northern" };
        //    var area2 = new Area { AreaName = "Southern" };
        //    var territory1 = context.Territories.Single(t => t.TerritoryId == TestTerritoryId1);
        //    var territory2 = context.Territories.Single(t => t.TerritoryId == TestTerritoryId2);
        //    var territory3 = context.Territories.Single(t => t.TerritoryId == TestTerritoryId3);
        //    territory1.Area = area1;
        //    territory2.Area = area2;
        //    territory3.Area = area2;
        //    var employee1 = new Employee
        //    {
        //        FirstName = "Test",
        //        LastName = "Employee1",
        //        City = "San Fransicso",
        //        Country = "USA",
        //        Territories = new List<Territory> { territory1, territory2 }
        //    };
        //    var employee2 = new Employee
        //    {
        //        FirstName = "Test",
        //        LastName = "Employee2",
        //        City = "Sacramento",
        //        Country = "USA",
        //        Territories = new List<Territory> { territory2, territory3 }
        //    };

        //    // Persist entities
        //    context.Employees.Add(employee1);
        //    context.Employees.Add(employee2);
        //    context.SaveChanges();

        //    // Detach entities
        //    context.DetachEntities(employee1);
        //    context.DetachEntities(employee2);

        //    // Clear reference properties
        //    territory1.Area = null;
        //    territory2.Area = null;
        //    territory3.Area = null;
        //    employee1.Territories = new List<Territory> { territory1, territory2 };
        //    employee2.Territories = new List<Territory> { territory2, territory3 };

        //    // Return employees
        //    return new List<Employee> { employee1, employee2 };
        //}

        private List<Product> CreateTestProductsWithPromos(NorthwindDbContext context)
        {
            // Create test entities
            var promo1 = new HolidayPromo
            {
                PromoId = 1,
                PromoCode = "THX",
                HolidayName = "Thanksgiving"
            };
            var category1 = new Category
            {
                CategoryName = "Test Category 1a"
            };
            var product1 = new Product
            {
                ProductName = "Test Product 1a",
                UnitPrice = 10M,
                Category = category1,
                HolidayPromo = promo1
            };

            // Persist entities
            context.Products.Add(product1);
            context.SaveChanges();

            // Detach entities
            context.DetachEntities(product1);

            // Clear reference properties
            product1.Category = null;
            product1.HolidayPromo = null;

            // Return entities
            return new List<Product> { product1 };
        }

        private List<Product> CreateTestProductsWithProductInfo(NorthwindDbContext context)
        {
            // Create test entities
            var category1 = new Category
            {
                CategoryName = "Test Category 1b"
            };
            var info1 = context.ProductInfos
                .Single(pi => pi.ProductInfoKey1 == ProductInfo1A
                    && pi.ProductInfoKey2 == ProductInfo1B);
            var product1 = new Product
            {
                ProductName = "Test Product 1b",
                UnitPrice = 10M,
                Category = category1,
                ProductInfo = info1
            };

            // Persist entities
            context.Products.Add(product1);
            context.SaveChanges();

            // Detach entities
            context.DetachEntities(product1);

            // Clear reference properties
            product1.Category = null;
            product1.ProductInfo = null;

            // Return entities
            return new List<Product> { product1 };
        }

        #endregion

        #region Order-Customer: Many-to-One

        [Fact]
        public async void LoadRelatedEntitiesAsync_Should_Populate_Order_With_Customer()
        {
            // Arrange
            var context = _fixture.GetContext();
            var order = CreateTestOrders(context)[0];
            order.TrackingState = TrackingState.Added;

            // Act
            await context.LoadRelatedEntitiesAsync(order);

            // Assert
            Assert.NotNull(order.Customer);
            Assert.Equal(order.CustomerId, order.Customer?.CustomerId);
        }

        [Fact]
        public async void LoadRelatedEntitiesAsync_Load_All_Should_Populate_Order_With_Customer()
        {
            // Arrange
            var context = _fixture.GetContext();
            var order = CreateTestOrders(context)[0];

            // Act
            await context.LoadRelatedEntitiesAsync(order);

            // Assert
            Assert.NotNull(order.Customer);
            Assert.Equal(order.CustomerId, order.Customer?.CustomerId);
        }

        [Fact]
        public async void LoadRelatedEntitiesAsync_Should_Populate_Order_With_Customer_With_Territory()
        {
            // Arrange
            var context = _fixture.GetContext();
            var order = CreateTestOrders(context)[0];
            order.TrackingState = TrackingState.Added;

            // Act
            await context.LoadRelatedEntitiesAsync(order);

            // Assert
            Assert.NotNull(order.Customer?.Territory);
            Assert.Equal(order.Customer?.TerritoryId, order.Customer?.Territory?.TerritoryId);
        }

        [Fact]
        public async void LoadRelatedEntitiesAsync_Should_Populate_Order_With_Customer_With_Setting()
        {
            // Arrange
            var context = _fixture.GetContext();
            var order = CreateTestOrders(context)[0];
            order.TrackingState = TrackingState.Added;

            // Act
            await context.LoadRelatedEntitiesAsync(order);

            // Assert
            Assert.NotNull(order?.Customer?.CustomerSetting);
            Assert.Equal(order?.Customer?.CustomerId, order?.Customer?.CustomerSetting?.CustomerId);
        }

        #endregion

        #region Order-OrderDetail-Product-Category: One-to-Many-to-One

        [Fact]
        public async void LoadRelatedEntitiesAsync_Should_Populate_Order_Details_With_Product_With_Category()
        {
            // Arrange
            var context = _fixture.GetContext();
            var order = CreateTestOrders(context)[0];
            order.TrackingState = TrackingState.Added;

            // Act
            await context.LoadRelatedEntitiesAsync(order);

            // Assert
            var details = order.OrderDetails;
            Assert.DoesNotContain(details, d => d.Product == null);
            Assert.DoesNotContain(details, d => d.Product?.ProductId != d.ProductId);
            Assert.DoesNotContain(details, d => d.Product?.Category == null);
            Assert.DoesNotContain(details, d => d.Product?.Category?.CategoryId != d.Product?.CategoryId);
        }

        [Fact]
        public async void LoadRelatedEntitiesAsync_Load_All_Should_Populate_Order_Details_With_Product_With_Category()
        {
            // Arrange
            var context = _fixture.GetContext();
            var order = CreateTestOrders(context)[0];

            // Act
            await context.LoadRelatedEntitiesAsync(order);

            // Assert
            var details = order.OrderDetails;
            Assert.DoesNotContain(details, d => d.Product == null);
            Assert.DoesNotContain(details, d => d.Product?.ProductId != d.ProductId);
            Assert.DoesNotContain(details, d => d.Product?.Category == null);
            Assert.DoesNotContain(details, d => d.Product?.Category?.CategoryId != d.Product?.CategoryId);
        }

        #endregion
    }
}