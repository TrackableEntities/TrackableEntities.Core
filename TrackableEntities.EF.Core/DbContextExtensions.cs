using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TrackableEntities.Common.Core;
using TrackableEntities.EF.Core.Internal;

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
            // Detach root entity
            context.Entry(item).State = EntityState.Detached;

            // Recursively set entity state for DbContext entry
            context.ChangeTracker.TrackGraph(item, node =>
            {
                // Exit if not ITrackable
                if (node.Entry.Entity is not ITrackable trackable) return;

                // Detach node entity
                node.Entry.State = EntityState.Detached;

                // Get related parent entity
                if (node.SourceEntry != null)
                {
                    var relationship = node.InboundNavigation?.GetRelationshipType();
                    switch (relationship)
                    {
                        case RelationshipType.OneToOne:
                            // If parent is added set to added
                            if (node.SourceEntry.State == EntityState.Added)
                            {
                                SetEntityState(node.Entry, TrackingState.Added.ToEntityState(), trackable);
                            }
                            else if (node.SourceEntry.State == EntityState.Deleted)
                            {
                                SetEntityState(node.Entry, TrackingState.Deleted.ToEntityState(), trackable);
                            }
                            else
                            {
                                SetEntityState(node.Entry, trackable.TrackingState.ToEntityState(), trackable);
                            }
                            return;
                        case RelationshipType.ManyToOne:
                            // If parent is added set to added
                            if (node.SourceEntry.State == EntityState.Added)
                            {
                                SetEntityState(node.Entry, TrackingState.Added.ToEntityState(), trackable);
                                return;
                            }
                            // If parent is deleted set to deleted
                            var parent = node.SourceEntry.Entity as ITrackable;
                            if (node.SourceEntry.State == EntityState.Deleted
                                || parent?.TrackingState == TrackingState.Deleted)
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
                        case RelationshipType.OneToMany:
                            // If trackable is set deleted set entity state to unchanged,
                            // since it may be related to other entities.
                            if (trackable.TrackingState == TrackingState.Deleted)
                            {
                                SetEntityState(node.Entry, TrackingState.Unchanged.ToEntityState(), trackable);
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

        /// <summary>
        /// Set entity state to Detached for entities in more than one object graph.
        /// </summary>
        /// <param name="context">Used to query and save changes to a database</param>
        /// <param name="items">Objects that implement ITrackable</param>
        public static void DetachEntities(this DbContext context, IEnumerable<ITrackable> items)
        {
            // Detach each item in the object graph
            foreach (var item in items)
                context.DetachEntities(item);
        }

        /// <summary>
        /// Set entity state to Detached for entities in an object graph.
        /// </summary>
        /// <param name="context">Used to query and save changes to a database</param>
        /// <param name="item">Object that implements ITrackable</param>
        public static void DetachEntities(this DbContext context, ITrackable item)
        {
            // Detach each item in the object graph
            context.TraverseGraph(item, n => n.Entry.State = EntityState.Detached);
        }

        /// <summary>
        /// Traverse an object graph to populate null reference properties.
        /// </summary>
        /// <param name="context">Used to query and save changes to a database</param>
        /// <param name="item">Object that implements ITrackable</param>
        public static void LoadRelatedEntities(this DbContext context, ITrackable item)
        {
            // Traverse graph to load references          
            context.TraverseGraph(item, n =>
            {
                if (n.Entry.State == EntityState.Detached)
                    n.Entry.State = EntityState.Unchanged;
                foreach (var reference in n.Entry.References)
                {
                    if (!reference.IsLoaded)
                        reference.Load();
                }
            });
        }

        /// <summary>
        /// Traverse more than one object graph to populate null reference properties.
        /// </summary>
        /// <param name="context">Used to query and save changes to a database</param>
        /// <param name="items">Objects that implement ITrackable</param>
        public static void LoadRelatedEntities(this DbContext context, IEnumerable<ITrackable> items)
        {
            // Traverse graph to load references          
            foreach (var item in items)
                context.LoadRelatedEntities(item);
        }

        /// <summary>
        /// Traverse an object graph asynchronously to populate null reference properties.
        /// </summary>
        /// <param name="context">Used to query and save changes to a database</param>
        /// <param name="item">Object that implements ITrackable</param>
        public static async Task LoadRelatedEntitiesAsync(this DbContext context, ITrackable item)
        {
            // Detach each item in the object graph         
            await context.TraverseGraphAsync(item, async n =>
            {
                if (n.Entry.State == EntityState.Detached)
                    n.Entry.State = EntityState.Unchanged;
                foreach (var reference in n.Entry.References)
                {
                    Console.WriteLine(reference);
                    if (!reference.IsLoaded && reference.CurrentValue == null)
                        await reference.LoadAsync();
                }
            });
        }

        /// <summary>
        /// Traverse more than one object graph asynchronously to populate null reference properties.
        /// </summary>
        /// <param name="context">Used to query and save changes to a database</param>
        /// <param name="items">Objects that implement ITrackable</param>
        public static async Task LoadRelatedEntitiesAsync(this DbContext context, IEnumerable<ITrackable> items)
        {
            // Traverse graph to load references
            foreach (var item in items)
                await context.LoadRelatedEntitiesAsync(item);
        }

        /// <summary>
        /// Traverse an object graph to set TrackingState to Unchanged.
        /// </summary>
        /// <param name="context">Used to query and save changes to a database</param>
        /// <param name="item">Object that implements ITrackable</param>
        public static void AcceptChanges(this DbContext context, ITrackable item)
        {
            // Traverse graph to set TrackingState to Unchanged
            context.TraverseGraph(item, n =>
            {
                if (n.Entry.Entity is ITrackable trackable)
                {
                    if (trackable.TrackingState != TrackingState.Unchanged)
                        trackable.TrackingState = TrackingState.Unchanged;
                    if (trackable.ModifiedProperties?.Count > 0)
                        trackable.ModifiedProperties.Clear();
                }
            });
        }

        /// <summary>
        /// Traverse more than one object graph to set TrackingState to Unchanged.
        /// </summary>
        /// <param name="context">Used to query and save changes to a database</param>
        /// <param name="items">Objects that implement ITrackable</param>
        public static void AcceptChanges(this DbContext context, IEnumerable<ITrackable> items)
        {
            // Traverse graph to set TrackingState to Unchanged
            foreach (var item in items)
                context.AcceptChanges(item);
        }

        private static void SetEntityState(EntityEntry entry, EntityState state, ITrackable trackable)
        {
            // Set entity state to tracking state
            entry.State = state;

            // Set modified properties
            if (entry.State == EntityState.Modified
                && trackable.ModifiedProperties != null)
            {
                foreach (var property in entry.Properties)
                    property.IsModified = trackable.ModifiedProperties.Any(p =>
                        string.Compare(p, property.Metadata.Name, StringComparison.InvariantCultureIgnoreCase) == 0);
            }
        }
    }
}

