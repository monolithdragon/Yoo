using System;
using UnityEngine;

namespace YooX.DependencyInjection {
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property)]
	public sealed class InjectAttribute : PropertyAttribute { }
}