using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
                // Exit if not ITrackable
                if (!(node.Entry.Entity is ITrackable trackable)) return;

                // Get related parent entity
                if (node.SourceEntry != null)
                {
                    var relationship = node.InboundNavigation?.GetRelationshipType();
                    switch (relationship)
                    {
                        case RelationshipType.OneToOne:
                            // If parent is added, set to added
                            if (node.SourceEntry.State == EntityState.Added)
                            {
                                SetEntityState(node.Entry, TrackingState.Added.ToEntityState(), trackable);
                            }
                            else
                            {
                                SetEntityState(node.Entry, trackable.TrackingState.ToEntityState(), trackable);
                            }
                            return;
                        case RelationshipType.ManyToOne:
                            // If parent is added, set to added
                            if (node.SourceEntry.State == EntityState.Added)
                            {
                                SetEntityState(node.Entry, TrackingState.Added.ToEntityState(), trackable);
                                return;
                            }
                            // If parent is deleted, set to deleted
                            if (node.SourceEntry.State == EntityState.Deleted)
                            {
                                try
                                {
                                    // Will throw if there are added children
                                    SetEntityState(node.Entry, TrackingState.Deleted.ToEntityState(), trackable);
                                }
                                catch (InvalidOperationException e)
                                {
                                    throw new InvalidOperationException(Constants.ExceptionMessages.DeletedWithAddedChildren, e);
                                }
                                return;
                            }
                            break;
                    }
                }

                // Set entity state to tracking state
                SetEntityState(node.Entry, trackable.TrackingState.ToEntityState(), trackable);
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

        private static void SetEntityState(EntityEntry entry, EntityState state, ITrackable trackable)
        {
            // Set entity state to tracking state
            entry.State = state;

            // Set modified properties
            if (entry.State == EntityState.Modified
                && trackable.ModifiedProperties != null)
            {
                foreach (var property in trackable.ModifiedProperties)
                    entry.Property(property).IsModified = true;
            }
        }
    }
}
