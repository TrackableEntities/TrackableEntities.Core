using Microsoft.EntityFrameworkCore.Metadata;
using TrackableEntities.EF.Core.Tests.Helpers;
using TrackableEntities.EF.Core.Tests.Mocks;
using TrackableEntities.EF.Core.Tests.NorthwindModels;
using Xunit;

namespace TrackableEntities.EF.Core.Tests
{
    [Collection("NorthwindDbContext")]
    public class NavigationExtensionsTests
    {
        private readonly NorthwindDbContextFixture _fixture;

        public NavigationExtensionsTests(NorthwindDbContextFixture fixture)
        {
            _fixture = fixture;
            _fixture.Initialize();
        }

        [Fact]
        public void GetRelationshipType_Order_Customer_Should_Return_OneToMany()
        {
            // Arrange
            var context = _fixture.GetContext();
            var order = new MockNorthwind().Orders[0];
            INavigationBase nav = context.Entry(order).Navigation(nameof(order.Customer)).Metadata;

            // Act
            RelationshipType? relType = nav.GetRelationshipType();

            // Assert
            Assert.Equal(RelationshipType.OneToMany, relType);
        }

        [Fact]
        public void GetRelationshipType_Order_OrderDetail_Should_Return_ManyToOne()
        {
            // Arrange
            var context = _fixture.GetContext();
            var order = new MockNorthwind().Orders[0];
            INavigationBase nav = context.Entry(order).Navigation(nameof(order.OrderDetails)).Metadata;

            // Act
            RelationshipType? relType = nav.GetRelationshipType();

            // Assert
            Assert.Equal(RelationshipType.ManyToOne, relType);
        }

        [Fact]
        public void GetRelationshipType_Customer_CustomerSetting_Should_Return_OneToOne()
        {
            // Arrange
            var context = _fixture.GetContext();
            var customer = new MockNorthwind().Customers[0];
            customer.CustomerSetting = new CustomerSetting
            {
                CustomerId = customer.CustomerId!,
                Customer = customer,
                Setting = "Setting 1"
            };
            INavigationBase nav = context.Entry(customer).Navigation(nameof(customer.CustomerSetting)).Metadata;

            // Act
            RelationshipType? relType = nav.GetRelationshipType();

            // Assert
            Assert.Equal(RelationshipType.OneToOne, relType);
        }


    }
}