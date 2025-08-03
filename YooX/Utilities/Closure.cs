using System;

namespace YooX {
	public readonly struct Closure<TContext> {
		private readonly Delegate _delegate;
		private readonly TContext _context;

		public Closure(Delegate @delegate, TContext context = default) {
			_delegate = @delegate;
			_context = context;
		}

		public void Invoke() {
			switch(_delegate) {
				case Action action:
					action();
					break;
				case Action<TContext> actionWithContext:
					actionWithContext(_context);
					break;
				default:
					throw new InvalidOperationException($"Unsupported delegate type: {_delegate.GetType()}");
			}
		}

		public TResult Invoke<TResult>() {
			return _delegate switch {
				Func<TResult> func => func(),
				Func<TContext, TResult> funcWithContext => funcWithContext(_context),
				var _ => throw new InvalidOperationException($"Unsupported delegate type: {_delegate.GetType()}")
			};
		}

		public static Closure<TContext> Create(Action action) => new(action);
		public static Closure<TContext> Create(Action<TContext> action, TContext context) => new(action, context);
		public static Closure<TContext> Create<TResult>(Func<TResult> func) => new(func);
		public static Closure<TContext> Create<TResult>(Func<TContext, TResult> func, TContext context) => new(func, context);
	}
}