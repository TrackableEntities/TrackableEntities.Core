using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using TrackableEntities.Common.Core;

namespace TrackableEntities.EF.Core.Tests.NorthwindModels
{
    public partial class ProductInfo : ITrackable
    {
        public int ProductInfoKey1 { get; set; }
        public int ProductInfoKey2 { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public string Info { get; set; } = string.Empty;

        [NotMapped]
        public TrackingState TrackingState { get; set; }
        [NotMapped]
        public ICollection<string>? ModifiedProperties { get; set; }
    }
}
