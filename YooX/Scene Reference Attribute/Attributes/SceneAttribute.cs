using System;

namespace YooX.SceneReferenceAttribute {
    /// <summary>
    /// Scene looks for the reference anywhere in the scene
    /// using GameObject.FindAnyObjectByType() and GameObject.FindObjectsOfType()
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class SceneAttribute : SceneRefAttribute {
        public SceneAttribute(Flag flags = Flag.None, Type? filter = null) : base(RefLoc.Scene, flags, filter!) { }
    }
}
