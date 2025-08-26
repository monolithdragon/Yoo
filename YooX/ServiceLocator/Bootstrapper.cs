using UnityEngine;

namespace YooX.ServiceLocator {
	[DisallowMultipleComponent, RequireComponent(typeof(ServiceLocator))]
	public abstract class Bootstrapper : MonoBehaviour {
		private ServiceLocator _container;
		private bool _hasBeenBootstrapped;

		public ServiceLocator Container => _container.OrNull() ?? (_container = GetComponent<ServiceLocator>());

		private void Awake() => BootstrapOnDemand();

		public void BootstrapOnDemand() {
			if (_hasBeenBootstrapped) {
				return;
			}

			_hasBeenBootstrapped = true;
			Bootstrap();
		}

		abstract protected void Bootstrap();
	}
}