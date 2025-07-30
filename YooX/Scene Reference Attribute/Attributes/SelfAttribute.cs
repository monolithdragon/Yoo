using System;

namespace YooX.SceneReferenceAttribute {
	/// <summary>
	/// Self looks for the reference on the same game object as the attributed component
	/// using GetComponent(s)()
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class SelfAttribute(Flag flags = Flag.None, Type? filter = null) : SceneRefAttribute(RefLoc.Self, flags, filter!);
}