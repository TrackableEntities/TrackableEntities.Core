using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TrackableEntities.Common.Core;

namespace TrackableEntities.EF.Core
{
  public class ApplyChangesProvider : IApplyChangesProvider
  {
    public ApplyChangesProvider(DbContext dbContext)
    {
      DbContext = dbContext;
    }

    public DbContext DbContext { get; }

    /// <summary>
    /// Update entity state on DbContext for an object graph.
    /// </summary>
    /// <param name="item">Object that implements ITrackable</param>
    public virtual void ApplyChanges(ITrackable item)
    {
      // Detach root entity
      DbContext.Entry(item).State = EntityState.Detached;

      // Recursively set entity state for DbContext entry
      DbContext.ChangeTracker.TrackGraph(item, node =>
      {
        // Exit if not ITrackable
        if (!(node.Entry.Entity is ITrackable trackable)) return;

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

    protected virtual void SetEntityState(EntityEntry entry, EntityState state, ITrackable trackable)
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

    /// <summary>
    /// Update entity state on DbContext for more than one object graph.
    /// </summary>
    /// <param name="items">Objects that implement ITrackable</param>
    public virtual void ApplyChanges(IEnumerable<ITrackable> items)
    {
      // Apply changes to collection of items
      foreach (var item in items)
        ApplyChanges(item);
    }
  }
}
