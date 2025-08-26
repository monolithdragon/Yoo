using System.Collections.Generic;
using UnityEngine;

namespace YooX.EventBus {
	public class EventBus<T> where T : IEvent {
		private static readonly HashSet<IEventBinding<T>> Bindings = new();

		static public void Register(IEventBinding<T> binding) => Bindings.Add(binding);
		static public void Unregister(IEventBinding<T> binding) => Bindings.Remove(binding);

		static public void Raise(T @event) {
			foreach (var binding in Bindings) {
				binding.OnEvent.Invoke(@event);
				binding.OnEventNoArgs.Invoke();
			}
		}

		private static void Clear() {
			Debug.Log($"Clearing {typeof(T).Name} bindings");
			Bindings.Clear();
		}
	}
}