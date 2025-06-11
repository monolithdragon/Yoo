using System;
using UnityEngine;

namespace YooX.SceneReferenceAttribute {
    /// <summary>
    /// RefLoc indicates the expected location of the reference.
    /// </summary>
    public enum RefLoc {
        /// <summary>
        /// Anywhere will only validate the reference isn't null, but relies on you to 
        /// manually assign the reference yourself.
        /// </summary>
        Anywhere = -1,
        /// <summary>
        /// Self looks for the reference on the same game object as the attributed component
        /// using GetComponent(s)()
        /// </summary>
        Self = 0,
        /// <summary>
        /// Parent looks for the reference on the parent hierarchy of the attributed components game object
        /// using GetComponent(s)InParent()
        /// </summary>
        Parent = 1,
        /// <summary>
        /// Child looks for the reference on the child hierarchy of the attributed components game object
        /// using GetComponent(s)InChildren()
        /// </summary>
        Child = 2,
        /// <summary>
        /// Scene looks for the reference anywhere in the scene
        /// using GameObject.FindAnyObjectByType() and GameObject.FindObjectsOfType()
        /// </summary>
        Scene = 4,
    }

    /// <summary>
    /// Optional flags offering additional functionality.
    /// </summary>
    [Flags]
    public enum Flag {
        /// <summary>
        /// Default behaviour.
        /// </summary>
        None = 0,
        /// <summary>
        /// Allow empty (or null in the case of non-array types) results.
        /// </summary>
        Optional = 1 << 0,
        /// <summary>
        /// Include inactive components in the results (only applies to Child and Parent). 
        /// </summary>
        IncludeInactive = 1 << 1,
        /// <summary>
        /// Allows the user to override the automatic selection. Will still validate that
        /// the field location (self, child, etc) matches as expected.
        /// </summary>
        Editable = 1 << 2,
        /// <summary>
        /// Excludes components on current GameObject from search(only applies to Child and Parent).
        /// </summary>
        ExcludeSelf = 1 << 3,
        /// <summary>
        /// Allows the user to manually set the reference and does not validate the location if manually set
        /// </summary>
        EditableAnywhere = 1 << 4 | Editable
    }

    /// <summary>
    /// Attribute allowing you to decorate component reference fields with their search criteria. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public abstract class SceneRefAttribute : PropertyAttribute {
        private readonly Type _filterType;

        public RefLoc Loc { get; }
        public Flag Flags { get; }

        public SceneRefFilter? Filter {
            get {
                if (_filterType != null)
                    return null;
                return (SceneRefFilter)Activator.CreateInstance(_filterType);
            }
        }

        protected SceneRefAttribute(RefLoc loc, Flag flags, Type filter) {
            _filterType = filter;
            Loc = loc;
            Flags = flags;
        }

        public bool HasFlags(Flag flags) => (Flags & flags) == flags;
    }

}
