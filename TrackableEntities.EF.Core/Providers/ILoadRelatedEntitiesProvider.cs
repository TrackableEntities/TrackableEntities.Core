using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TrackableEntities.Common.Core;

namespace TrackableEntities.EF.Core
{
    public interface ILoadRelatedEntitiesProvider
    {
        DbContext DbContext { get; }

        void LoadRelatedEntities(ITrackable item);
        void LoadRelatedEntities(IEnumerable<ITrackable> items);

        Task LoadRelatedEntitiesAsync(ITrackable item);
        Task LoadRelatedEntitiesAsync(IEnumerable<ITrackable> items);
    }
}
