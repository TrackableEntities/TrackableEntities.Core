using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using TrackableEntities.Common.Core;

namespace TrackableEntities.EF.Core.Tests.NorthwindModels
{
    public partial class EmployeeTerritory : ITrackable
    {
        public int EmployeeId { get; set; } = 0;
        public string TerritoryId { get; set; } = string.Empty;

        public Employee? Employee { get; set; }
        public Territory? Territory { get; set; }

        [NotMapped]
        public TrackingState TrackingState { get; set; }
        [NotMapped]
        public ICollection<string>? ModifiedProperties { get; set; }
    }
}
