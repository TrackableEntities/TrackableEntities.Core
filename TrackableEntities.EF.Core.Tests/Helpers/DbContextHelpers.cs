using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using TrackableEntities.Common.Core;

namespace TrackableEntities.EF.Core.Tests.Helpers
{
    public static class DbContextHelpers
    {
        public static void TraverseGraph(this DbContext context, object rootEntity,
            Action<EntityEntryGraphNode> callback)
        {
            IStateManager stateManager = context.Entry(rootEntity).GetInfrastructure().StateManager;
            var node = new EntityEntryGraphNode(stateManager.GetOrCreateEntry(rootEntity), null, null);
            IEntityEntryGraphIterator graphIterator = new EntityEntryGraphIterator();
            graphIterator.TraverseGraph(node, n =>
            {
                callback(n);
                return true;
            });
        }

        public static IEnumerable<EntityState> GetEntityStates(this DbContext context,
            ITrackable item, EntityState? entityState = null)
        {
            var entityStates = new List<EntityState>();
            context.TraverseGraph(item, n =>
            {
                entityStates.Add(n.Entry.State);
            });
            return entityStates;
        }

        public static void SetTrackingStates(this DbContext context,
            ITrackable item, TrackingState trackingState)
        {
            context.TraverseGraph(item, n =>
            {
                if (n.Entry.Entity is ITrackable trackable)
                {
                    trackable.TrackingState = trackingState;
                }
            });
        }
    }
}
