using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace YooX.DependencyInjection {
	[DefaultExecutionOrder(-1000)]
	public class Injector : Singleton<Injector> {
		private const BindingFlags Binding = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
		private readonly Dictionary<Type, object> _registry = new();

		protected override void Awake() {
			base.Awake();

			// Find all modules implementing IDependencyProvider
			var providers = FindAllMonoBehaviours().OfType<IDependencyProvider>();
			foreach (var provider in providers) {
				RegisterProvider(provider);
			}

			// Find all injectable objects and inject their dependencies
			var injectables = FindAllMonoBehaviours().Where(IsInjectable);
			foreach (var injectable in injectables) {
				Inject(injectable);
			}
		}

		public void ValidateDependencies() {
			var monoBehaviours = FindAllMonoBehaviours();
			var providers = monoBehaviours.OfType<IDependencyProvider>();
			var providerDependencies = GetProviderDependencies(providers);
			var invalidDependencyList = monoBehaviours
			                            .SelectMany(mb => mb.GetType().GetFields(Binding), (mb, field) => new { mb, field })
			                            .Where(t => Attribute.IsDefined(t.field, typeof(InjectAttribute)))
			                            .Where(t => !providerDependencies.Contains(t.field.FieldType) && t.field.GetValue(t.mb) == null)
			                            .Select(t => $"[Validation]: {t.mb.GetType().Name} is missing dependency {t.field.FieldType.Name} on GameObject {t.mb.gameObject.name}.")
			                            .ToList();

			if (!invalidDependencyList.Any()) {
				Debug.Log("[Validation]: All dependencies are valid.");
			} else {
				Debug.LogError($"[Validation]: {invalidDependencyList.Count} dependencies are invalid.");
				foreach (var dependency in invalidDependencyList) {
					Debug.LogError($"[Validation]: {dependency}");
				}
			}
		}

		private HashSet<Type> GetProviderDependencies(IEnumerable<IDependencyProvider> providers) {
			var result = new HashSet<Type>();

			foreach (var provider in providers) {
				var methods = provider.GetType().GetMethods(Binding);

				foreach (var method in methods) {
					if (!Attribute.IsDefined(method, typeof(ProvideAttribute))) continue;

					var returnType = method.ReturnType;
					result.Add(returnType);
				}
			}

			return result;
		}

		public void ClearDependencies() {
			foreach (var monoBehaviour in FindAllMonoBehaviours()) {
				var type = monoBehaviour.GetType();
				var injectableFields = type.GetFields(Binding).Where(member => Attribute.IsDefined(member, typeof(InjectAttribute)));

				foreach (var injectableField in injectableFields) {
					injectableField.SetValue(monoBehaviour, null);
				}
			}

			Debug.Log("[Injector]: All injectable fields cleared.");
		}

		private static MonoBehaviour[] FindAllMonoBehaviours() => FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.InstanceID);

		private void RegisterProvider(IDependencyProvider provider) {
			var methods = provider.GetType().GetMethods(Binding);
			foreach (var method in methods) {
				if (!Attribute.IsDefined(method, typeof(ProvideAttribute))) continue;

				var returnType = method.ReturnType;
				var provideInstance = method.Invoke(provider, null);
				if (provideInstance != null) {
					_registry.Add(returnType, provideInstance);
					Debug.Log($"Registered {returnType.Name} from {provideInstance.GetType().Name}");
				} else {
					throw new Exception($"Provider {provider.GetType().Name} returned null for {returnType.Name}.");
				}
			}
		}

		private static bool IsInjectable(MonoBehaviour instance) {
			var members = instance.GetType().GetMethods(Binding);
			return members.Any(member => Attribute.IsDefined(member, typeof(InjectAttribute)));
		}

		private void Inject(object instance) {
			InjectField(instance);
			InjectMethod(instance);
			InjectProperty(instance);
		}

		private void InjectField(object instance) {
			var type = instance.GetType();
			var injectableFields = type.GetFields(Binding).Where(member => Attribute.IsDefined(member, typeof(InjectAttribute)));
			foreach (var injectableField in injectableFields) {
				if (injectableField.GetValue(instance) != null) {
					Debug.LogWarning($"[Injector]: Field '{injectableField.Name}' of class '{type.Name}' is already injected.");
					continue;
				}
				var fieldType = injectableField.FieldType;
				var resolveInstance = Resolve(fieldType);
				if (resolveInstance == null) {
					throw new Exception($"Failed to resolve '{fieldType.Name}' for '{type.Name}'");
				}

				injectableField.SetValue(instance, resolveInstance);
				Debug.Log($"Field injected '{fieldType.Name}' into '{type.Name}'");
			}
		}

		private void InjectMethod(object instance) {
			var type = instance.GetType();
			var injectableMethods = type.GetMethods(Binding).Where(member => Attribute.IsDefined(member, typeof(InjectAttribute)));
			foreach (var injectableMethod in injectableMethods) {
				var requiredParameters = injectableMethod.GetParameters()
				                                         .Select(parameter => parameter.ParameterType)
				                                         .ToArray();
				var resolvedInstances = requiredParameters.Select(Resolve).ToArray();
				if (resolvedInstances.Any(resolvedInstance => resolvedInstance == null)) {
					throw new Exception($"Failed to inject {type.Name}.{injectableMethod.Name}");
				}

				injectableMethod.Invoke(instance, resolvedInstances);
				Debug.Log($"Method injected {type.Name}.{injectableMethod.Name}");
			}
		}

		private void InjectProperty(object instance) {
			var type = instance.GetType();
			var injectableProperties = type.GetProperties(Binding).Where(member => Attribute.IsDefined(member, typeof(InjectAttribute)));
			foreach (var injectableProperty in injectableProperties) {
				var propertyType = injectableProperty.PropertyType;
				var resolveInstance = Resolve(propertyType);
				if (resolveInstance == null) {
					throw new Exception($"Failed to resolve {propertyType.Name} for {type.Name}");
				}

				injectableProperty.SetValue(instance, resolveInstance);
				Debug.Log($"Property injected {propertyType.Name} into {type.Name}");
			}
		}

		private object Resolve(Type type) {
			_registry.TryGetValue(type, out var instance);
			return instance;
		}
	}
}