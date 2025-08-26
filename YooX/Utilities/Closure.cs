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
			switch (_delegate) {
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

		public TResult Invoke<TResult>() =>
			_delegate switch {
				Func<TResult> func => func(),
				Func<TContext, TResult> funcWithContext => funcWithContext(_context),
				var _ => throw new InvalidOperationException($"Unsupported delegate type: {_delegate.GetType()}")
			};

		static public Closure<TContext> Create(Action action) => new(action);
		static public Closure<TContext> Create(Action<TContext> action, TContext context) => new(action, context);
		static public Closure<TContext> Create<TResult>(Func<TResult> func) => new(func);
		static public Closure<TContext> Create<TResult>(Func<TContext, TResult> func, TContext context) => new(func, context);
	}
}