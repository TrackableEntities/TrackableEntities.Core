using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TrackableEntities.Common.Core;

namespace TrackableEntities.EF.Core
{
    /// <summary>
    /// Extension methods for DbContext to persist trackable entities.
    /// </summary>
    public static class DbContextExtensions
    {
        /// <summary>
        /// Update entity state on DbContext for an object graph.
        /// </summary>
        /// <param name="context">Used to query and save changes to a database</param>
        /// <param name="item">Object that implements ITrackable</param>
        public static void ApplyChanges(this DbContext context, ITrackable item)
        {
            // Recursively set entity state for DbContext entry
            context.ChangeTracker.TrackGraph(item, node =>
            {
                if (!(node.Entry.Entity is ITrackable trackable)) return;
                node.Entry.State = trackable.TrackingState.ToEntityState();
                if (trackable.TrackingState == TrackingState.Modified 
                    && trackable.ModifiedProperties != null)
                {
                    foreach (var property in trackable.ModifiedProperties)
                        node.Entry.Property(property).IsModified = true;
                }
            });
        }

        /// <summary>
        /// Update entity state on DbContext for more than one object graph.
        /// </summary>
        /// <param name="context">Used to query and save changes to a database</param>
        /// <param name="items">Objects that implement ITrackable</param>
        public static void ApplyChanges(this DbContext context, IEnumerable<ITrackable> items)
        {
            // Apply changes to collection of items
            foreach (var item in items)
                context.ApplyChanges(item);
        }
    }
}
