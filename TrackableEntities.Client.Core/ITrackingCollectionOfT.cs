using System.Collections.Generic;
using System.ComponentModel;
using TrackableEntities.Common.Core;

namespace TrackableEntities.Client.Core
{
    /// <summary>
    /// Interface implemented by trackable collections.
    /// </summary>
    public interface ITrackingCollection<TEntity> : ICollection<TEntity>
        where TEntity : class, ITrackable, INotifyPropertyChanged
    {
        /// <summary>
        /// Get entities that have been marked as Added, Modified or Deleted.
        /// </summary>
        /// <returns>Collection containing only changed entities</returns>
        ChangeTrackingCollection<TEntity> GetChanges();
    }
}
