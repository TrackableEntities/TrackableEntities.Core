using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TrackableEntities.Common.Core;

namespace TrackableEntities.EF.Core.Tests.NorthwindModels
{
    public partial class Territory : ITrackable
    {
        public Territory()
        {
            EmployeeTerritories = new List<EmployeeTerritory>();
            Customers = new List<Customer>();
        }

        [Key]
        public string TerritoryId { get; set; }
        public string TerritoryDescription { get; set; }
        public List<EmployeeTerritory> EmployeeTerritories { get; set; }
        public List<Customer> Customers { get; set; }
        public int? AreaId { get; set; }
        [ForeignKey("AreaId")]
        public Area Area { get; set; }

        [NotMapped]
        public TrackingState TrackingState { get; set; }
        [NotMapped]
        public ICollection<string> ModifiedProperties { get; set; }
    }
}
