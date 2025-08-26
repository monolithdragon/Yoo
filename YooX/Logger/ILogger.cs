using System;
using System.Runtime.CompilerServices;

namespace YooX {
	public interface ILogger : IDisposable {
		public LoggerConfig Config { get; set; }

		public void Info(
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