using UnityEditor;
using UnityEngine;

namespace YooX.ServiceLocator {
	[AddComponentMenu("ServiceLocator/ServiceLocator Global")]
	public class ServiceLocatorGlobalBootstrapper : Bootstrapper {
		[SerializeField] private bool dontDestroyOnload = true;
		protected override void Bootstrap() {
			Container.ConfigureAsGlobal(dontDestroyOnload);
		}
	}
}