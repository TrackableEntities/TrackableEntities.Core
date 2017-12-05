using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TrackableEntities.Common.Core;
using TrackableEntities.EF.Core.Internal;

namespace TrackableEntities.EF.Core
{
    public class LoadRelatedEntitiesProvider : ILoadRelatedEntitiesProvider
    {
        public LoadRelatedEntitiesProvider(DbContext context) : this(context, new TraverseGraphProvider(context))
        {
        }
        public LoadRelatedEntitiesProvider(DbContext context, ITraverseGraphProvider traverseGraphProvider)
        {                                                                                                       
            DbContext = context;
            TraverseGraphProvider = traverseGraphProvider;
        }

        public DbContext DbContext { get; }
        public ITraverseGraphProvider TraverseGraphProvider { get; }

        public virtual void LoadRelatedEntities(ITrackable item)
        {
            // Traverse graph to load references          
            TraverseGraphProvider.TraverseGraph(item, n =>
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

        public virtual void LoadRelatedEntities(IEnumerable<ITrackable> items)
        {
            // Traverse graph to load references          
            foreach (var item in items)
                LoadRelatedEntities(item);
        }

        public virtual Task LoadRelatedEntitiesAsync(ITrackable item)
        { 
            // Detach each item in the object graph         
            return TraverseGraphProvider.TraverseGraphAsync(item, async n =>
            {
                if (n.Entry.State == EntityState.Detached)
                    n.Entry.State = EntityState.Unchanged;
                foreach (var reference in n.Entry.References)
                {
                    if (!reference.IsLoaded)
                        await reference.LoadAsync();
                }
            });
        }

        public virtual async Task LoadRelatedEntitiesAsync(IEnumerable<ITrackable> items)
        {
            // Traverse graph to load references
            foreach (var item in items)
                await LoadRelatedEntitiesAsync(item);
        }
    }
}
