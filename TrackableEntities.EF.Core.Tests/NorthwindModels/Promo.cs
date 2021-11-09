using System.ComponentModel.DataAnnotations;

namespace TrackableEntities.EF.Core.Tests.NorthwindModels
{
    public partial class Promo
    {
        [Key]
        public int PromoId { get; set; }
        public string PromoCode { get; set; } = string.Empty;
    }
}
