using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TrackableEntities.Common.Core;

namespace TrackableEntities.EF.Core.Tests.NorthwindModels
{
    public partial class CustomerSetting : ITrackable
    {
        [Key]
        public string CustomerId { get; set; } = string.Empty;
        public string Setting { get; set; } = string.Empty;

        [ForeignKey("CustomerId"), Required]
        public Customer? Customer { get; set; }

        [NotMapped]
        public TrackingState TrackingState { get; set; }
        [NotMapped]
        public ICollection<string>? ModifiedProperties { get; set; }
    }
}
