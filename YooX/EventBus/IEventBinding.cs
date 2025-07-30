using System;

namespace YooX.EventBus {
	public interface IEventBinding<T> {
		public Action<T> OnEvent { get; set; }
		public Action OnEventNoArgs { get; set; }
	}
}