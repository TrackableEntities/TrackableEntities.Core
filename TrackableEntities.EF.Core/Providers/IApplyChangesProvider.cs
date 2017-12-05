using TrackableEntities.Common.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace TrackableEntities.EF.Core
{
  public interface IApplyChangesProvider
  {
    DbContext DbContext { get; }

    void ApplyChanges(ITrackable item);
    void ApplyChanges(IEnumerable<ITrackable> items);    
  }
}
