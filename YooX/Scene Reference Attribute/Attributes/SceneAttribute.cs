using System;

namespace YooX.SceneReferenceAttribute {
	/// <summary>
	/// Scene looks for the reference anywhere in the scene
	/// using GameObject.FindAnyObjectByType() and GameObject.FindObjectsOfType()
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class SceneAttribute(Flag flags = Flag.None, Type? filter = null) : SceneRefAttribute(RefLoc.Scene, flags, filter!);
}