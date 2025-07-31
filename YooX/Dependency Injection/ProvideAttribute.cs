using System;
using UnityEngine;

namespace YooX.DependencyInjection {
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class ProvideAttribute : PropertyAttribute { }
}