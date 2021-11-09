using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TrackableEntities.Common.Core;

namespace TrackableEntities.EF.Core.Tests.FamilyModels
{
    public class Parent : ITrackable
    {
        public Parent() { }
        public Parent(string name)
        {
            Name = name;
        }

        [Key]
        public string Name { get; set; } = string.Empty;
        public string Hobby { get; set; } = string.Empty;
        public List<Child> Children { get; set; } = new List<Child>();

        [NotMapped]
        public TrackingState TrackingState { get; set; }
        [NotMapped]
        public ICollection<string>? ModifiedProperties { get; set; }
    }
}
