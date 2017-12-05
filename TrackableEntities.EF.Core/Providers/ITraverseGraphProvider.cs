using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TrackableEntities.EF.Core.Internal
{
    public interface ITraverseGraphProvider
    {
        DbContext DbContext { get; }

        void TraverseGraph(object item, Action<EntityEntryGraphNode> callback);
        Task TraverseGraphAsync(object item, Func<EntityEntryGraphNode, Task> callback);
    }
}
