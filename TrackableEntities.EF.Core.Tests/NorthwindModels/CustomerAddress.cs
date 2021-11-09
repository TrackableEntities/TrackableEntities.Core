using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TrackableEntities.Common.Core;

namespace TrackableEntities.EF.Core.Tests.NorthwindModels
{
    public partial class CustomerAddress : ITrackable
    {
        [Key]
        public int CustomerAddressId { get; set; }
        public string Street { get; set; } = string.Empty;
        [Column]
        public string CustomerId { get; set; } = string.Empty;
        [ForeignKey("CustomerId")]
        public Customer? Customer { get; set; }

        [NotMapped]
        public TrackingState TrackingState { get; set; }
        [NotMapped]
        public ICollection<string>? ModifiedProperties { get; set; }
    }
}
