using System;

namespace YooX {
	public struct Either<TLeft, TRight> {
		private readonly TLeft _left;
		private readonly TRight _right;
		private readonly bool _isRight;

		public bool IsLeft => !_isRight;
		public bool IsRight => _isRight;

		public TLeft Left {
			get {
				if (IsRight) throw new InvalidOperationException("No Left value presented");
				return _left;
			}
		}
		public TRight Right {
			get {
				if (IsLeft) throw new InvalidOperationException("No Right value presented");
				return _right;
			}
		}

		public Either(TLeft left, TRight right, bool isRight) {
			_left = left;
			_right = right;
			_isRight = isRight;
		}

		public static Either<TLeft, TRight> FromLeft(TLeft left) => new(left, default, false);
		public static Either<TLeft, TRight> FromRight(TRight right) => new(default, right, true);

		public TResult Match<TResult>(Func<TLeft, TResult> leftFunc, Func<TRight, TResult> rightFunc) => IsRight ? rightFunc(_right) : leftFunc(_left);

		public Either<TLeft, TResult> SelectMany<TResult>(Func<TRight, Either<TLeft, TResult>> bind) => IsRight ? bind(_right) : Either<TLeft, TResult>.FromLeft(_left);

		public Either<TLeft, TResult> Select<TResult>(Func<TRight, TResult> map) => IsRight ? Either<TLeft, TResult>.FromRight(map(_right)) : Either<TLeft, TResult>.FromLeft(_left);

		public static implicit operator Either<TLeft, TRight>(TLeft left) => FromLeft(left);
		public static implicit operator Either<TLeft, TRight>(TRight right) => FromRight(right);

		public override string ToString() => IsRight ? $"Right({_right})" : $"Left({_left})";

	}
}