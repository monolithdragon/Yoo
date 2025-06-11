using System;

namespace YooX.SceneReferenceAttribute {
    /// <summary>
    /// Child looks for the reference on the child hierarchy of the attributed components game object
    /// using GetComponent(s)InChildren()
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ChildAttribute : SceneRefAttribute {
        public ChildAttribute(Flag flags = Flag.None, Type? filter = null) : base(RefLoc.Child, flags, filter!) { }
    }
}
