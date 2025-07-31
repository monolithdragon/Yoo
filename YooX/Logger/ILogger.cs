using System;

namespace YooX.Logger {
	public interface ILogger : IDisposable {
		void Info(string message);
		void Warning(string message);
		void Error(string message);
		void Log(LogLevel level, string message);
	}
}