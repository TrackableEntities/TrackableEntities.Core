using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;

// ReSharper disable once CheckNamespace
namespace TrackableEntities.EF.Core.Internal
{
    /// <summary>
    /// Internal extension methods for trackable entities.
    /// Depends on Entity Framework Core infrastructure, which may change in future releases.
    /// </summary>
    public static class DbContextExtensionsInternal
    {
        internal static void EnsureProvider<TProvider>(this DbContext context, ref TProvider provider)
        {                                                                    
            if(provider == null)                
                provider = context.ResolveDefaultProvider<TProvider>();
        }
        
        internal static TProvider ResolveDefaultProvider<TProvider>(this DbContext context) =>
            (TProvider)context.ResolveDefaultProvider(typeof(TProvider));

        internal static object ResolveDefaultProvider(this DbContext context, Type providerType)
        {   
            //we might want to use ServiceLocator
            if(providerType == typeof(IApplyChangesProvider))
                return new ApplyChangesProvider(context);
            else if (providerType == typeof(ITraverseGraphProvider))
                return new TraverseGraphProvider(context);
            else if (providerType == typeof(ILoadRelatedEntitiesProvider))
                return new LoadRelatedEntitiesProvider(context);
            else 
                throw new NotSupportedException($"No default provider for '{providerType}'.");
        }                     

        /// <summary>
        /// Traverse an object graph executing a callback on each node.
        /// Depends on Entity Framework Core infrastructure, which may change in future releases.
        /// </summary>
        /// <param name="context">Used to query and save changes to a database</param>
        /// <param name="item">Object that implements ITrackable</param>
        /// <param name="callback">Callback executed on each node in the object graph</param>
        public static void TraverseGraph(this DbContext context, object item,
            Action<EntityEntryGraphNode> callback, ITraverseGraphProvider traverseGraphProvider = null)
        {                           
            if(traverseGraphProvider == null)
                traverseGraphProvider = context.ResolveDefaultProvider<ITraverseGraphProvider>();
            traverseGraphProvider.TraverseGraph(item, callback);
        }

        /// <summary>
        /// Traverse an object graph asynchronously executing a callback on each node.
        /// Depends on Entity Framework Core infrastructure, which may change in future releases.
        /// </summary>
        /// <param name="context">Used to query and save changes to a database</param>
        /// <param name="item">Object that implements ITrackable</param>
        /// <param name="callback">Async callback executed on each node in the object graph</param>
        public static Task TraverseGraphAsync(this DbContext context, object item,
            Func<EntityEntryGraphNode, Task> callback, ITraverseGraphProvider traverseGraphProvider = null)
        {
            if(traverseGraphProvider == null)
                traverseGraphProvider = context.ResolveDefaultProvider<ITraverseGraphProvider>();
            return traverseGraphProvider.TraverseGraphAsync(item, callback);
        }
    }
}
