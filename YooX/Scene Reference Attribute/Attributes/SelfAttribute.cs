using System;

namespace YooX.SceneReferenceAttribute {
    /// <summary>
    /// Self looks for the reference on the same game object as the attributed component
    /// using GetComponent(s)()
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class SelfAttribute : SceneRefAttribute {
        public SelfAttribute(Flag flags = Flag.None, Type? filter = null) : base(RefLoc.Self, flags, filter!) { }
    }
}
