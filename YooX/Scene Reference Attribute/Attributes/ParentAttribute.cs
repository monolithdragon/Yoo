using System;

namespace YooX.SceneReferenceAttribute {
    /// <summary>
    /// Parent looks for the reference on the parent hierarchy of the attributed components game object
    /// using GetComponent(s)InParent()
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ParentAttribute : SceneRefAttribute {
        public ParentAttribute(Flag flags = Flag.None, Type? filter = null) : base(RefLoc.Parent, flags, filter!) { }
    }
}
