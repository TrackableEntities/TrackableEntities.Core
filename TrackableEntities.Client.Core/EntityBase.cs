using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using TrackableEntities.Common.Core;

namespace TrackableEntities.Client.Core
{
    /// <summary>
    /// Base class for model entities
    /// </summary>
    [JsonObject(IsReference = true)]
    [DataContract(IsReference = true)]
    public abstract partial class EntityBase : INotifyPropertyChanged, ITrackable, IIdentifiable
    {
        /// <summary>
        /// Event for notification of property changes
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Fire PropertyChanged event.
        /// </summary>
        /// <typeparam name="TResult">Property return type</typeparam>
        /// <param name="property">Lambda expression for property</param>
        protected void NotifyPropertyChanged<TResult>
            (Expression<Func<TResult>> property)
        {
            string propertyName = ((MemberExpression)property.Body).Member.Name;
            // ReSharper disable once ExplicitCallerInfoArgument
            NotifyPropertyChanged(propertyName);
        }

        /// <summary>
        /// Fire PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Generate entity identifier used for correlation with MergeChanges (if not yet done)
        /// </summary>
        public void SetEntityIdentifier()
        {
            if (EntityIdentifier == Guid.Empty)
                EntityIdentifier = Guid.NewGuid();
        }

        /// <summary>
        /// Copy entity identifier used for correlation with MergeChanges from another entity
        /// </summary>
        /// <param name="other">Other trackable object</param>
        public void SetEntityIdentifier(IIdentifiable other)
        {
            if (other is EntityBase otherEntity)
                EntityIdentifier = otherEntity.EntityIdentifier;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same
        /// type. The comparison is based on EntityIdentifier.
        /// 
        /// If the local EntityIdentifier is empty, then return false.
        /// </summary>
        /// <param name="other">An object to compare with this object</param>
        /// <returns></returns>
        public bool IsEquatable(IIdentifiable? other)
        {
            if (EntityIdentifier == default)
                return false;

            if (other is not EntityBase otherEntity)
                return false;

            return EntityIdentifier.Equals(otherEntity.EntityIdentifier);
        }

        bool IEquatable<IIdentifiable>.Equals(IIdentifiable? other)
        {
            return IsEquatable(other);
        }

        /// <summary>
        /// Change-tracking state of an entity.
        /// </summary>
        [DataMember]
        public TrackingState TrackingState { get; set; }

        /// <summary>
        /// Properties on an entity that have been modified.
        /// </summary>
        [DataMember]
        public ICollection<string>? ModifiedProperties { get; set; }

        /// <summary>
        /// Identifier used for correlation with MergeChanges.
        /// </summary>
        [DataMember]
        public Guid EntityIdentifier { get; set; }
    }
}
