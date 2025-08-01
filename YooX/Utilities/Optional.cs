using System;
using System.Collections.Generic;

namespace YooX {
	public struct Optional<T> {
		public static readonly Optional<T> NoValue = new();

		private readonly T _value;

		public T Value => HasValue ? _value : throw new InvalidOperationException("No Value");
		public bool HasValue { get; }

		public Optional(T value) {
			_value = value;
			HasValue = true;
		}

		public T GetValueOrDefault() => _value;
		public T GetValueOrDefault(T defaultValue) => HasValue ? _value : defaultValue;

		public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<TResult> onFailure) => HasValue ? onSuccess(_value) : onFailure();
		public Optional<TResult> SelectMany<TResult>(Func<T, Optional<TResult>> bind) => HasValue ? bind(_value) : Optional<TResult>.NoValue;
		public Optional<TResult> Select<TResult>(Func<T, TResult> map) => HasValue ? new Optional<TResult>(map(_value)) : Optional<TResult>.NoValue;

		public static Optional<TResult> Combine<T1, T2, TResult>(Optional<T1> first, Optional<T2> second, Func<T1, T2, TResult> combiner) {
			if (first.HasValue && second.HasValue) {
				return new Optional<TResult>(combiner(first.Value, second.Value));
			}

			return Optional<TResult>.NoValue;
		}

		public static Optional<T> Some(T value) => new(value);
		public static Optional<T> None() => NoValue;

		public bool Equals(Optional<T> other) => !HasValue ? !other.HasValue : EqualityComparer<T>.Default.Equals(_value, _value);
		public override bool Equals(object obj) => obj is Optional<T> other && Equals(other);
		public override int GetHashCode() => HasValue.GetHashCode() * 397 ^ EqualityComparer<T>.Default.GetHashCode(_value);
		public override string ToString() => HasValue ? $"Some({_value})" : "None";

		public static implicit operator Optional<T>(T value) => new(value);
		public static implicit operator bool(Optional<T> value) => value.HasValue;
		public static implicit operator T(Optional<T> value) => value.Value;
	}
}