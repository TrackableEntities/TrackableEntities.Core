using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TrackableEntities.Common.Core;
using TrackableEntities.EF.Core.Tests.FamilyModels;
using TrackableEntities.EF.Core.Tests.Helpers;
using TrackableEntities.EF.Core.Tests.Mocks;
using TrackableEntities.EF.Core.Tests.NorthwindModels;
using Xunit;

namespace TrackableEntities.EF.Core.Tests
{
    [Collection("FamilyDbContext")]
    public class FamilyDbContextTests
    {
        private readonly FamilyDbContextFixture _fixture;

        public FamilyDbContextTests(FamilyDbContextFixture fixture)
        {
            fixture.Initialize();
            _fixture = fixture;
        }

        #region ApplyChanges Tests

        [Fact]
        public void Apply_Changes_Should_Mark_Added_Parent()
        {
            // Arrange
            var context = _fixture.GetContext();
            var parent = new Parent("Parent");
            parent.TrackingState = TrackingState.Added;

            // Act
            context.ApplyChanges(parent);

            // Assert
            Assert.Equal(EntityState.Added, context.Entry(parent).State);
        }

        [Fact]
        public void Apply_Changes_Should_Mark_Modified_Parent()
        {
            // Arrange
            var context = _fixture.GetContext();
            var parent = new Parent("Parent");
            parent.TrackingState = TrackingState.Modified;

            // Act
            context.ApplyChanges(parent);

            // Assert
            Assert.Equal(EntityState.Modified, context.Entry(parent).State);
        }

        [Fact]
        public void Apply_Changes_Should_Mark_Modified_Parent_Properties()
        {
            // Arrange
            var context = _fixture.GetContext();
            var parent = new Parent("Parent");
            parent.Hobby = "Hobby_Changed";
            parent.TrackingState = TrackingState.Modified;
            parent.ModifiedProperties = new List<string> { "Hobby" };

            // Act
            context.ApplyChanges(parent);

            // Assert
            Assert.Equal(EntityState.Modified, context.Entry(parent).State);
            Assert.True(context.Entry(parent).Property("Hobby").IsModified);
        }

        [Fact]
        public void Apply_Changes_Should_Mark_Deleted_Parent()
        {
            // Arrange
            var context = _fixture.GetContext();
            var parent = new Parent("Parent");
            parent.TrackingState = TrackingState.Deleted;

            // Act
            context.ApplyChanges(parent);

            // Assert
            Assert.Equal(EntityState.Deleted, context.Entry(parent).State);
        }

        [Fact]
        public void Apply_Changes_Should_Mark_Added_Parent_With_Children()
        {
            // Arrange
            var context = _fixture.GetContext();
            var parent = new Parent("Parent")
            {
                Children = new List<Child>
                    {
                        new Child("Child1"), 
                        new Child("Child2"),
                        new Child("Child3")
                    }
            };
            parent.TrackingState = TrackingState.Added;
            parent.Children[0].TrackingState = TrackingState.Added;
            parent.Children[1].TrackingState = TrackingState.Added;
            parent.Children[2].TrackingState = TrackingState.Added;

            // Act
            context.ApplyChanges(parent);

            // Assert
            Assert.Equal(EntityState.Added, context.Entry(parent).State);
            Assert.Equal(EntityState.Added, context.Entry(parent.Children[0]).State);
            Assert.Equal(EntityState.Added, context.Entry(parent.Children[1]).State);
            Assert.Equal(EntityState.Added, context.Entry(parent.Children[2]).State);
        }

        [Fact]
        public void Apply_Changes_Should_Mark_Family_Unchanged()
        {
            // Arrange
            var context = _fixture.GetContext();
            var parent = new MockFamily().Parent;

            // Act
            context.ApplyChanges(parent);

            // Assert
            var expectedState = EntityState.Unchanged;
            var states = context.GetEntityStates(parent, expectedState).ToList();
            TestHelpers.AssertStates(states, expectedState);
        }

        [Fact]
        public void Apply_Changes_Should_Mark_Family_Added()
        {
            // Arrange
            var context = _fixture.GetContext();
            var parent = new MockFamily().Parent;
            parent.TrackingState = TrackingState.Added;
            context.SetTrackingStates(parent, TrackingState.Added);

            // Act
            context.ApplyChanges(parent);

            // Assert
            var expectedState = EntityState.Added;
            var states = context.GetEntityStates(parent, expectedState).ToList();
            TestHelpers.AssertStates(states, expectedState);
        }

        [Fact]
        public void Apply_Changes_Should_Mark_Family_Modified()
        {
            // Arrange
            var context = _fixture.GetContext();
            var parent = new MockFamily().Parent;
            parent.TrackingState = TrackingState.Modified;
            context.SetTrackingStates(parent, TrackingState.Modified);

            // Act
            context.ApplyChanges(parent);

            // Assert
            var expectedState = EntityState.Modified;
            var states = context.GetEntityStates(parent, expectedState).ToList();
            TestHelpers.AssertStates(states, expectedState);
        }

        [Fact]
        public void Apply_Changes_Should_Mark_Parent_With_Added_Modified_Deleted_Children()
        {
            // Arrange
            var context = _fixture.GetContext();
            var child1 = new Child("Child1");
            var child2 = new Child("Child2");
            var child3 = new Child("Child3");
            var parent = new Parent("Parent")
            {
                Children = new List<Child> { child1, child2, child3 }
            };
            child1.TrackingState = TrackingState.Added;
            child2.TrackingState = TrackingState.Modified;
            child3.TrackingState = TrackingState.Deleted;

            // Act
            context.ApplyChanges(parent);

            // Assert
            Assert.Equal(EntityState.Unchanged, context.Entry(parent).State);
            Assert.Equal(EntityState.Added, context.Entry(child1).State);
            Assert.Equal(EntityState.Modified, context.Entry(child2).State);
            Assert.Equal(EntityState.Deleted, context.Entry(child3).State);
        }

        [Fact]
        public void Apply_Changes_Should_Mark_Grandchild_Deleted()
        {
            // Arrange
            var context = _fixture.GetContext();
            var parent = new MockFamily().Parent;
            parent.Children.RemoveAt(2);
            parent.Children.RemoveAt(1);
            var child = parent.Children[0];
            child.Children.RemoveAt(2);
            child.Children.RemoveAt(1);
            var grandchild = child.Children[0];
            grandchild.Children = new List<Child>();
            parent.TrackingState = TrackingState.Unchanged;
            child.TrackingState = TrackingState.Deleted;
            grandchild.TrackingState = TrackingState.Deleted;

            // Act
            context.ApplyChanges(parent);

            // Assert
            Assert.Equal(EntityState.Unchanged, context.Entry(parent).State);
            Assert.Equal(EntityState.Deleted, context.Entry(child).State);
            Assert.Equal(EntityState.Deleted, context.Entry(grandchild).State);
        }

        #endregion

        #region OneToMany AcceptChanges Tests

        [Fact]
        public void Accept_Changes_Should_Mark_Family_Unchanged()
        {
            // Arrange
            var parent = new MockFamily().Parent;
            parent.TrackingState = TrackingState.Modified;
            parent.Children[0].TrackingState = TrackingState.Modified;
            parent.Children[0].Children[0].TrackingState = TrackingState.Modified;
            parent.Children[0].Children[0].Children[0].TrackingState = TrackingState.Added;
            parent.Children[0].Children[0].Children[1].TrackingState = TrackingState.Modified;
            parent.Children[0].Children[0].Children[2].TrackingState = TrackingState.Deleted;
            parent.Children[1].TrackingState = TrackingState.Added;
            parent.Children[1].Children[0].TrackingState = TrackingState.Added;
            parent.Children[1].Children[0].Children[0].TrackingState = TrackingState.Added;
            parent.Children[1].Children[0].Children[1].TrackingState = TrackingState.Added;
            parent.Children[1].Children[0].Children[2].TrackingState = TrackingState.Added;
            parent.Children[2].TrackingState = TrackingState.Deleted;
            parent.Children[2].Children[1].TrackingState = TrackingState.Deleted;
            parent.Children[2].Children[1].Children[0].TrackingState = TrackingState.Deleted;
            parent.Children[2].Children[1].Children[1].TrackingState = TrackingState.Deleted;
            parent.Children[2].Children[1].Children[2].TrackingState = TrackingState.Deleted;

            // Act
            var context = _fixture.GetContext();
            context.AcceptChanges(parent);

            // Assert
            var states = context.GetTrackingStates(parent, TrackingState.Unchanged).ToList();
            Assert.Equal(40, states.Count);
        }

        [Fact]
        public void Accept_Changes_Should_Remove_ModifiedProperties_From_Family()
        {
            // Arrange
            var parent = new MockFamily().Parent;
            parent.ModifiedProperties = new List<string> { "Name" };
            parent.Children[0].ModifiedProperties = new List<string> { "Name" };
            parent.Children[0].Children[0].ModifiedProperties = new List<string> { "Name" };
            parent.Children[0].Children[0].Children[1].ModifiedProperties = new List<string> { "Name" };

            // Act
            var context = _fixture.GetContext();
            context.AcceptChanges(parent);

            // Assert
            IEnumerable<ICollection<string>> modifiedProps = context.GetModifiedProperties(parent);
            Assert.DoesNotContain(modifiedProps, p => p?.Count > 0);
        }

        #endregion
    }
}
