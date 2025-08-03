using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace YooX {
	public interface ILogger<T> : IDisposable where T : ScriptableObject {
		public T Config { get; set; }

		public void Log(
			string message,
			LogLevel level = LogLevel.Info,
			[CallerMemberName] string memberName = "",
			[CallerFilePath] string sourceFile = "",
			[CallerLineNumber] int sourceLineNumber = 0
		);

		public void Warning(
			string message,
			LogLevel level = LogLevel.Warning,
			[CallerMemberName] string memberName = "",
			[CallerFilePath] string sourceFile = "",
			[CallerLineNumber] int sourceLineNumber = 0
		);

		public void Error(
			string message,
			LogLevel level = LogLevel.Error,
			[CallerMemberName] string memberName = "",
			[CallerFilePath] string sourceFile = "",
			[CallerLineNumber] int sourceLineNumber = 0
		);

	}
}