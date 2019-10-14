using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;

// ReSharper disable once CheckNamespace
namespace TrackableEntities.EF.Core.Internal
{
    /// <summary>
    /// Internal extension methods for trackable entities.
    /// </summary>
    public static class DbContextExtensionsInternal
    {
        /// <summary>
        /// Traverse an object graph executing a callback on each node.
        /// </summary>
        /// <param name="context">Used to query and save changes to a database</param>
        /// <param name="item">Object that implements ITrackable</param>
        /// <param name="callback">Callback executed on each node in the object graph</param>
        public static void TraverseGraph(this DbContext context, object item,
            Action<EntityEntryGraphNode> callback)
        {
#pragma warning disable EF1001 // Internal EF Core API usage.
            IStateManager stateManager = context.Entry(item).GetInfrastructure().StateManager;
            var node = new EntityEntryGraphNode<object>(stateManager.GetOrCreateEntry(item), null, null, null);
            IEntityEntryGraphIterator graphIterator = new EntityEntryGraphIterator();
#pragma warning restore EF1001 // Internal EF Core API usage.
            var visited = new HashSet<int>();

            graphIterator.TraverseGraph<object>(node, n =>
            {
                // Check visited
                if (visited.Contains(n.Entry.Entity.GetHashCode()))
                    return false;

                // Execute callback
                callback(n);

                // Add visited
                visited.Add(n.Entry.Entity.GetHashCode());

                // Continue traversal
                return true;
            });
        }

        /// <summary>
        /// Traverse an object graph asynchronously executing a callback on each node.
        /// </summary>
        /// <param name="context">Used to query and save changes to a database</param>
        /// <param name="item">Object that implements ITrackable</param>
        /// <param name="callback">Async callback executed on each node in the object graph</param>
        public static async Task TraverseGraphAsync(this DbContext context, object item,
            Func<EntityEntryGraphNode, Task> callback)
        {
#pragma warning disable EF1001 // Internal EF Core API usage.
            IStateManager stateManager = context.Entry(item).GetInfrastructure().StateManager;
            var node = new EntityEntryGraphNode<object>(stateManager.GetOrCreateEntry(item), null, null, null);
            IEntityEntryGraphIterator graphIterator = new EntityEntryGraphIterator();
#pragma warning restore EF1001 // Internal EF Core API usage.
            var visited = new HashSet<int>();

            await graphIterator.TraverseGraphAsync<object>(node, async (n, ct) =>
            {
                // Check visited
                if (visited.Contains(n.Entry.Entity.GetHashCode()))
                    return false;

                // Execute callback
                await callback(n);

                // Add visited
                visited.Add(n.Entry.Entity.GetHashCode());

                // Continue traversal
                return true;
            });
        }
    }
}
