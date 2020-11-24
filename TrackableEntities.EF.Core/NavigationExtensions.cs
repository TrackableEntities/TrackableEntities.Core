using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace TrackableEntities.EF.Core
{
    /// <summary>
    /// Extension methods for INavigation.
    /// </summary>
    public static class NavigationExtensions
    {
        /// <summary>
        /// Infer relationship type from an INavigation.
        /// </summary>
        /// <param name="navigation">Navigation property which can be used to navigate a relationship.</param>
        /// <returns>Type of relationship between entities; null if INavigation is null.</returns>
        public static RelationshipType? GetRelationshipType(this INavigationBase navigation)
        {
            var nav = navigation as INavigation;
            if (nav == null) return null;
            if (nav.ForeignKey.IsUnique)
                return RelationshipType.OneToOne;
            return nav.IsOnDependent ? RelationshipType.OneToMany : RelationshipType.ManyToOne;
        }
    }
}