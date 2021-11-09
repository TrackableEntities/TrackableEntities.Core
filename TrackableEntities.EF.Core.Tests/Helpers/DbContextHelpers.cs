using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TrackableEntities.Common.Core;
using TrackableEntities.EF.Core.Internal;

namespace TrackableEntities.EF.Core.Tests.Helpers
{
    public static class DbContextHelpers
    {
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

        public static IEnumerable<ICollection<string>> GetModifiedProperties(
            this DbContext context, ITrackable item)
        {
            var modifiedProps = new List<ICollection<string>>();
            context.TraverseGraph(item, n =>
            {
                if (n.Entry.Entity is ITrackable trackable)
                {
                    if (trackable.ModifiedProperties != null)
                        modifiedProps.Add(trackable.ModifiedProperties);
                }
            });
            return modifiedProps;
        }

        public static IEnumerable<TrackingState> GetTrackingStates(
            this DbContext context, ITrackable item, TrackingState? trackingState = null)
        {
            var states = new List<TrackingState>();
            context.TraverseGraph(item, n =>
            {
                if (n.Entry.Entity is ITrackable trackable)
                {
                    if (trackingState == null || trackable.TrackingState == trackingState)
                        states.Add(trackable.TrackingState);
                }
            });
            return states;
        }
    }
}
