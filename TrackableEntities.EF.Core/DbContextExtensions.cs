using System;
using System.Collections.Generic;
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
        public static void ApplyChanges(this DbContext context, ITrackable item, IApplyChangesProvider applyChangesProvider = null)
        {
            context.EnsureProvider(ref applyChangesProvider);                
            applyChangesProvider.ApplyChanges(item);
        }


        /// <summary>
        /// Update entity state on DbContext for more than one object graph.
        /// </summary>
        /// <param name="context">Used to query and save changes to a database</param>
        /// <param name="items">Objects that implement ITrackable</param>
        public static void ApplyChanges(this DbContext context, IEnumerable<ITrackable> items, IApplyChangesProvider applyChangesProvider = null)
        {
            context.EnsureProvider(ref applyChangesProvider);                
            applyChangesProvider.ApplyChanges(items);
        }

        /// <summary>
        /// Set entity state to Detached for entities in more than one object graph.
        /// </summary>
        /// <param name="context">Used to query and save changes to a database</param>
        /// <param name="items">Objects that implement ITrackable</param>
        public static void DetachEntities(this DbContext context, IEnumerable<ITrackable> items, ITraverseGraphProvider traverseGraphProvider = null)
        {                    
            // Detach each item in the object graph
            foreach (var item in items)
                context.DetachEntities(item, traverseGraphProvider);
        }

        /// <summary>
        /// Set entity state to Detached for entities in an object graph.
        /// </summary>
        /// <param name="context">Used to query and save changes to a database</param>
        /// <param name="item">Object that implements ITrackable</param>
        public static void DetachEntities(this DbContext context, ITrackable item, ITraverseGraphProvider traverseGraphProvider = null)
        {
            // Detach each item in the object graph
            context.TraverseGraph(item, n => n.Entry.State = EntityState.Detached, traverseGraphProvider);
        }

        /// <summary>
        /// Traverse an object graph to populate null reference properties.
        /// </summary>
        /// <param name="context">Used to query and save changes to a database</param>
        /// <param name="item">Object that implements ITrackable</param>
        public static void LoadRelatedEntities(this DbContext context, ITrackable item, ILoadRelatedEntitiesProvider loadRelatedEntitiesProvider = null)
        {
            context.EnsureProvider(ref loadRelatedEntitiesProvider);                
            loadRelatedEntitiesProvider.LoadRelatedEntities(item);
        }

        /// <summary>
        /// Traverse more than one object graph to populate null reference properties.
        /// </summary>
        /// <param name="context">Used to query and save changes to a database</param>
        /// <param name="items">Objects that implement ITrackable</param>
        public static void LoadRelatedEntities(this DbContext context, IEnumerable<ITrackable> items, ILoadRelatedEntitiesProvider loadRelatedEntitiesProvider = null)
        {
            context.EnsureProvider(ref loadRelatedEntitiesProvider);                
            loadRelatedEntitiesProvider.LoadRelatedEntities(items);
        }

        /// <summary>
        /// Traverse an object graph asynchronously to populate null reference properties.
        /// </summary>
        /// <param name="context">Used to query and save changes to a database</param>
        /// <param name="item">Object that implements ITrackable</param>
        public static Task LoadRelatedEntitiesAsync(this DbContext context, ITrackable item, ILoadRelatedEntitiesProvider loadRelatedEntitiesProvider = null)
        {
            context.EnsureProvider(ref loadRelatedEntitiesProvider);                
            return loadRelatedEntitiesProvider.LoadRelatedEntitiesAsync(item);
        }

        /// <summary>
        /// Traverse more than one object graph asynchronously to populate null reference properties.
        /// </summary>
        /// <param name="context">Used to query and save changes to a database</param>
        /// <param name="items">Objects that implement ITrackable</param>
        public static Task LoadRelatedEntitiesAsync(this DbContext context, IEnumerable<ITrackable> items, ILoadRelatedEntitiesProvider loadRelatedEntitiesProvider = null)
        {
            context.EnsureProvider(ref loadRelatedEntitiesProvider);                
            return loadRelatedEntitiesProvider.LoadRelatedEntitiesAsync(items);
        }

        /// <summary>
        /// Traverse an object graph to set TrackingState to Unchanged.
        /// </summary>
        /// <param name="context">Used to query and save changes to a database</param>
        /// <param name="item">Object that implements ITrackable</param>
        public static void AcceptChanges(this DbContext context, ITrackable item, ITraverseGraphProvider traverseGraphProvider = null)
        {                        
            context.EnsureProvider(ref traverseGraphProvider);                
            // Traverse graph to set TrackingState to Unchanged
            traverseGraphProvider.TraverseGraph(item, n =>
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
        public static void AcceptChanges(this DbContext context, IEnumerable<ITrackable> items, ITraverseGraphProvider traverseGraphProvider = null)
        {
            // Traverse graph to set TrackingState to Unchanged
            foreach (var item in items)
                context.AcceptChanges(item, traverseGraphProvider);
        }
    }
}

