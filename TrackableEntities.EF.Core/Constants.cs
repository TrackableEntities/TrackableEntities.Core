namespace TrackableEntities.EF.Core
{
    /// <summary>
    /// File containing constants.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Exception messages.
        /// </summary>
        public static class ExceptionMessages
        {
            /// <summary>
            /// Exception message for deleted with children.
            /// </summary>
            public const string DeletedWithAddedChildren =
                "An entity may not be marked as Deleted if it has related entities which are marked as Added. " +
                "Remove added related entities before deleting a parent entity.";

            /// <summary>
            /// Exception message for relationship not determined.
            /// </summary>
            public const string RelationshipNotDetermined =
                "Cannot determine relationship type for {0} property on {1}.";
        }
    }
}
