using System;
using System.Collections.Generic;
using System.Linq;

namespace TrackableEntities.Client.Core
{
    /// <summary>
    /// This class facilitates proper checking for circular references when iterating the graph nodes.
    /// </summary>
    public class ObjectVisitationHelper
    {
        private readonly Dictionary<object, object> _objectSet;

        /// <summary>
        /// The collection will contain a single object or will be initialized empty
        /// if no/null object is provided.
        /// </summary>
        public ObjectVisitationHelper(object? obj = null)
        {
            _objectSet = new Dictionary<object, object>(ObjectReferenceEqualityComparer<object>.Default);
            if (obj != null)
                _objectSet.Add(obj, obj);
        }

        /// <summary>
        /// Initializes an empty collection with custom equality comparer.
        /// </summary>
        public ObjectVisitationHelper(IEqualityComparer<object> comparer)
        {
            _objectSet = new Dictionary<object, object>(comparer);
        }

        /// <summary>
        /// Creates a shallow copy of self
        /// </summary>
        public ObjectVisitationHelper Clone()
        {
            return new ObjectVisitationHelper(this);
        }

        private ObjectVisitationHelper(ObjectVisitationHelper other)
        {
            _objectSet = other._objectSet.ToDictionary(
                x => x.Key,
                y => y.Value,
                other._objectSet.Comparer);
        }

        /// <summary>
        /// Checks if the given graph node has already been visited (is contained in the collection)
        /// If not, then the object will be visited straight away, otherwise NOP and return false.
        /// <param name="obj">An object to be visited</param>
        /// </summary>
        public bool TryVisit(object obj)
        {
            // Already visited? Return false
            if (IsVisited(obj)) return false;

            // If not yet visited, then visit
            _objectSet.Add(obj, obj);
            return true;
        }

        /// <summary>
        /// Checks if the given graph node has already been visited (is contained in the collection)
        /// <param name="obj">An object to be checked</param>
        /// </summary>
        public bool IsVisited(object obj)
        {
            if (obj == null)
                throw new NullReferenceException("obj");

            return _objectSet.ContainsKey(obj);
        }

        /// <summary>
        /// Finds a visited object which matches the given object by the current EqualityComparer
        /// <param name="obj">An object to be found</param>
        /// </summary>
        public object? FindVisited(object obj)
        {
            if (obj == null) throw new NullReferenceException(nameof(obj));

            if (_objectSet.TryGetValue(obj, out object? result)) return result;
            return null;
        }
    }
}