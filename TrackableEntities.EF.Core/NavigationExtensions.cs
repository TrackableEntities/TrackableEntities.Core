using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace TrackableEntities.EF.Core
{
    public static class NavigationExtensions
    {
        public static RelationshipType? GetRelationshipType(this INavigation nav)
        {
            if (nav == null) return null;
            if (nav.ForeignKey.IsUnique)
                return RelationshipType.OneToOne;
            return nav.IsDependentToPrincipal() ? RelationshipType.OneToMany : RelationshipType.ManyToOne;
        }
    }
}