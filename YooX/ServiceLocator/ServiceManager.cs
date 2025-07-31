using System;
using System.Collections.Generic;
using UnityEngine;

namespace YooX.ServiceLocator {
	public class ServiceManager {
		private readonly Dictionary<Type, object> _services = new();

		public IEnumerable<object> RegisteredServices => _services.Values;

		public ServiceManager Register<TService>(TService service) {
			var type = typeof(TService);

			if (!_services.TryAdd(type, service)) {
				Logger.Error($"Service of type {type.FullName} already registered");
			}

			return this;
		}

		public ServiceManager Register(Type type, object service) {
			if (!type.IsInstanceOfType(service)) {
				throw new ArgumentException("Type of service doesn't match type of service interface", nameof(service));
			}

			if (!_services.TryAdd(type, service)) {
				Logger.Error($"Service of type {type.FullName} already registered");
			}

			return this;
		}

		public TService Get<TService>() where TService : class {
			var type = typeof(TService);

			if (_services.TryGetValue(type, out var service)) {
				return service as TService;
			}

			throw new ArgumentException($"[ServiceManager.Get]: Service of type {type.FullName} not registered");
		}

		public bool TryGet<TService>(out TService service) where TService : class {
			var type = typeof(TService);

			if (_services.TryGetValue(type, out var obj)) {
				service = obj as TService;
				return true;
			}

			service = null;
			return false;
		}
	}
}