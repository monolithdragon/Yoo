using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace YooX.ServiceLocator {
	public class ServiceLocator : MonoBehaviour {
		private static ServiceLocator global;
		private static Dictionary<Scene, ServiceLocator> sceneContainers;
		private static List<GameObject> sceneGameObjects;

		private readonly ServiceManager _serviceManager = new();

		private const string GlobalServiceLocatorName = "ServiceLocator [Global]";
		private const string SceneServiceLocatorName = "ServiceLocator [Scene]";

		public static ServiceLocator Global {
			get {
				if (global != null) return global;

				if (FindFirstObjectByType<ServiceLocatorGlobalBootstrapper>() is { } found) {
					found.BootstrapOnDemand();
					return global;
				}

				var container = new GameObject(GlobalServiceLocatorName, typeof(ServiceLocator));
				container.AddComponent<ServiceLocatorGlobalBootstrapper>().BootstrapOnDemand();

				return global;
			}
		}

		private void OnDestroy() {
			if (global == this) {
				global = null;
			} else if (sceneContainers.ContainsValue(this)) {
				sceneContainers.Remove(gameObject.scene);
			}
		}

		public static ServiceLocator For(MonoBehaviour mb) => mb.GetComponentInParent<ServiceLocator>().OrNull() ?? ForSceneOf(mb) ?? Global;

		public static ServiceLocator ForSceneOf(MonoBehaviour mb) {
			var scene = mb.gameObject.scene;

			if (sceneContainers.TryGetValue(scene, out var container)) {
				return container;
			}

			sceneGameObjects.Clear();
			scene.GetRootGameObjects(sceneGameObjects);

			foreach (var go in sceneGameObjects.Where(go => go.GetComponent<ServiceLocatorSceneBootstrapper>() != null)) {
				if (go.TryGetComponent(out ServiceLocatorSceneBootstrapper bootstrapper) && bootstrapper.Container != mb) {
					bootstrapper.BootstrapOnDemand();
					return bootstrapper.Container;
				}
			}

			return Global;
		}

		public ServiceLocator Register<T>(T service) {
			_serviceManager.Register(service);
			return this;
		}

		public ServiceLocator Register(Type type, object service) {
			_serviceManager.Register(type, service);
			return this;
		}

		public ServiceLocator Get<T>(out T service) where T : class {
			if (TryGetService(out service)) return this;

			if (TryGetNextInHierarchy(out var container)) {
				container.Get(out service);
				return this;
			}

			throw new ArgumentException($"[ServiceLocator.Get]: Service of type {typeof(T).FullName} not registered");
		}

		public void ConfigureAsGlobal(bool dontDestroyOnLoad) {
			if (global == this) {
				Logger.Warning("Service already configured as global");
			} else if (global != null) {
				Logger.Error("Another ServiceLocator is already configured as global");
			} else {
				global = this;
				if (dontDestroyOnLoad) DontDestroyOnLoad(gameObject);
			}
		}

		public void ConfigureForScene() {
			var scene = gameObject.scene;

			if (sceneContainers.ContainsKey(scene)) {
				Logger.Error("Another ServiceLocator is already configured for this scene");
				return;
			}

			sceneContainers.Add(scene, this);
		}

		private bool TryGetService<T>(out T service) where T : class => _serviceManager.TryGet<T>(out service);

		private bool TryGetNextInHierarchy(out ServiceLocator container) {
			if (this == global) {
				container = null;
				return false;
			}

			container = transform.parent.OrNull()?.GetComponentInParent<ServiceLocator>().OrNull() ?? ForSceneOf(this);
			return container != null;
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void ResetStatics() {
			global = null;
			sceneContainers = new Dictionary<Scene, ServiceLocator>();
			sceneGameObjects = new List<GameObject>();
		}

#if UNITY_EDITOR
		[MenuItem("GameObject/ServiceLocator/Add Global")]
		private static void AddGlobal() {
			var go = new GameObject(GlobalServiceLocatorName, typeof(ServiceLocatorGlobalBootstrapper));
		}
		
		[MenuItem("GameObject/ServiceLocator/Add Scene")]
		private static void AddScene() {
			var go = new GameObject(SceneServiceLocatorName, typeof(ServiceLocatorSceneBootstrapper));
		}
#endif
	}
}