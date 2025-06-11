using System;

namespace YooX.SceneReferenceAttribute {
    /// <summary>
    /// Anywhere will only validate the reference isn't null, but relies on you to 
    /// manually assign the reference yourself.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class AnywhereAttribute : SceneRefAttribute {
        public AnywhereAttribute(Flag flags = Flag.None, Type? filter = null) : base(RefLoc.Anywhere, flags, filter!) { }
    }

}
