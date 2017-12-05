using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace TrackableEntities.EF.Core.Internal
{
    public class TraverseGraphProvider : ITraverseGraphProvider
    {
        public TraverseGraphProvider(DbContext dbContext)
        {
            DbContext = dbContext;
        }
        public DbContext DbContext { get; }

        protected virtual IStateManager GetStateManager(object item) => DbContext.Entry(item).GetInfrastructure().StateManager;

        public virtual void TraverseGraph(object item, Action<EntityEntryGraphNode> callback)
        {
            var stateManager = GetStateManager(item);
            var node = new EntityEntryGraphNode(stateManager.GetOrCreateEntry(item), null, null);
            IEntityEntryGraphIterator graphIterator = new EntityEntryGraphIterator();
            var visited = new HashSet<int>();

            graphIterator.TraverseGraph(node, n =>
            {
                // Check visited
                if (visited.Contains(n.Entry.Entity.GetHashCode()))
                    return false;

                // Execute callback
                callback(n);

                // Add visited
                visited.Add(n.Entry.Entity.GetHashCode());

                // Return true if node state is null
                return true;
            });
        }

        public virtual Task TraverseGraphAsync(object item, Func<EntityEntryGraphNode, Task> callback)
        {
            var stateManager = GetStateManager(item);
            var node = new EntityEntryGraphNode(stateManager.GetOrCreateEntry(item), null, null);
            IEntityEntryGraphIterator graphIterator = new EntityEntryGraphIterator();
            var visited = new HashSet<int>();

            return graphIterator.TraverseGraphAsync(node, async (n, ct) =>
            {
                // Check visited
                if (visited.Contains(n.Entry.Entity.GetHashCode()))
                    return false;

                // Execute callback
                await callback(n);

                // Add visited
                visited.Add(n.Entry.Entity.GetHashCode());

                // Return true if node state is null
                return true;
            });
        }
    }
}
