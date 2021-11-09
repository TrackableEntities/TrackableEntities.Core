using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TrackableEntities.Common.Core;

namespace TrackableEntities.EF.Core.Tests.NorthwindModels
{
    public partial class Customer : ITrackable
    {
        [Key]
        public string? CustomerId { get; set; }
        public string? CustomerName { get; set; }

        public string? TerritoryId { get; set; }
        [ForeignKey("TerritoryId")]
        public Territory? Territory { get; set; }

        public List<Order> Orders { get; set; } = new List<Order>();
        public CustomerSetting? CustomerSetting { get; set; }
        public List<CustomerAddress> CustomerAddresses { get; set; } = new List<CustomerAddress>();

        [NotMapped]
        public TrackingState TrackingState { get; set; }
        [NotMapped]
        public ICollection<string>? ModifiedProperties { get; set; }
    }
}
