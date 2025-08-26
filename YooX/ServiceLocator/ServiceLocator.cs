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
		private static ServiceLocator _global;
		private static Dictionary<Scene, ServiceLocator> _sceneContainers;
		private static List<GameObject> _sceneGameObjects;

		private readonly ServiceManager _serviceManager = new();

		private const string GlobalServiceLocatorName = "ServiceLocator [Global]";
		private const string SceneServiceLocatorName = "ServiceLocator [Scene]";

		static public ServiceLocator Global {
			get {
				if (_global != null) {
					return _global;
				}

				if (FindFirstObjectByType<ServiceLocatorGlobalBootstrapper>() is { } found) {
					found.BootstrapOnDemand();

					return _global;
				}

				var container = new GameObject(GlobalServiceLocatorName, typeof(ServiceLocator));
				container.AddComponent<ServiceLocatorGlobalBootstrapper>().BootstrapOnDemand();

				return _global;
			}
		}

		private void OnDestroy() {
			if (_global == this) {
				_global = null;
			} else if (_sceneContainers.ContainsValue(this)) {
				_sceneContainers.Remove(gameObject.scene);
			}
		}

		static public ServiceLocator For(MonoBehaviour mb) => mb.GetComponentInParent<ServiceLocator>().OrNull() ?? ForSceneOf(mb) ?? Global;

		static public ServiceLocator ForSceneOf(MonoBehaviour mb) {
			var scene = mb.gameObject.scene;

			if (_sceneContainers.TryGetValue(scene, out var container)) {
				return container;
			}

			_sceneGameObjects.Clear();
			scene.GetRootGameObjects(_sceneGameObjects);

			foreach (var go in _sceneGameObjects.Where(go => go.GetComponent<ServiceLocatorSceneBootstrapper>() != null)) {
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
			if (TryGetService(out service)) {
				return this;
			}

			if (TryGetNextInHierarchy(out var container)) {
				container.Get(out service);

				return this;
			}

			throw new ArgumentException($"[ServiceLocator.Get]: Service of type {typeof(T).FullName} not registered");
		}

		public void ConfigureAsGlobal(bool dontDestroyOnLoad) {
			if (_global == this) {
				Debug.LogWarning("Service already configured as global");
			} else if (_global != null) {
				Debug.LogError("Another ServiceLocator is already configured as global");
			} else {
				_global = this;

				if (dontDestroyOnLoad) {
					transform.SetParent(null);
					DontDestroyOnLoad(gameObject);
				}
			}
		}

		public void ConfigureForScene() {
			var scene = gameObject.scene;

			if (_sceneContainers.ContainsKey(scene)) {
				Debug.LogError("Another ServiceLocator is already configured for this scene");

				return;
			}

			_sceneContainers.Add(scene, this);
		}

		private bool TryGetService<T>(out T service) where T : class => _serviceManager.TryGet<T>(out service);

		private bool TryGetNextInHierarchy(out ServiceLocator container) {
			if (this == _global) {
				container = null;

				return false;
			}

			container = transform.parent.OrNull()?.GetComponentInParent<ServiceLocator>().OrNull() ?? ForSceneOf(this);

			return container != null;
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void ResetStatics() {
			_global = null;
			_sceneContainers = new Dictionary<Scene, ServiceLocator>();
			_sceneGameObjects = new List<GameObject>();
		}

#if UNITY_EDITOR
        [MenuItem("GameObject/ServiceLocator/Add Global")]
        static void AddGlobal() {
            var go = new GameObject(GlobalServiceLocatorName, typeof(ServiceLocatorGlobalBootstrapper));
        }

        [MenuItem("GameObject/ServiceLocator/Add Scene")]
        static void AddScene() {
            var go = new GameObject(SceneServiceLocatorName, typeof(ServiceLocatorSceneBootstrapper));
        }
#endif
	}
}