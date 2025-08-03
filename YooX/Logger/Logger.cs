using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace YooX {
	public class Logger : ILogger {
		private StreamWriter _fileStream;
		private string _logFilePath;
		private readonly object _fileLock = new();

		public LoggerConfig Config { get; set; }

		public Logger(LoggerConfig config) {
			Config = config ?? throw new ArgumentException("LoggerConfig cannot be null");

			if (Config.enableFileLogging) {
				OpenLogFile();
			}

			Application.logMessageReceivedThreaded += HandleUnityLog;
		}

		public void Info(string message, LogLevel level = LogLevel.Info, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFile = "", [CallerLineNumber] int sourceLineNumber = 0) {
			string formatted = FormatMessage(level, message, sourceFile, sourceLineNumber, memberName);
			Debug.Log(formatted);

			if (Config.enableFileLogging) {
				WriteToFile(formatted);
			}
		}

		public void Warning(string message, LogLevel level = LogLevel.Warning, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFile = "", [CallerLineNumber] int sourceLineNumber = 0) {
			string formatted = FormatMessage(level, message, sourceFile, sourceLineNumber, memberName);
			Debug.LogWarning(formatted);

			if (Config.enableFileLogging) {
				WriteToFile(formatted);
			}
		}

		public void Error(string message, LogLevel level = LogLevel.Error, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFile = "", [CallerLineNumber] int sourceLineNumber = 0) {
			string formatted = FormatMessage(level, message, sourceFile, sourceLineNumber, memberName);
			Debug.LogError(formatted);

			if (Config.enableFileLogging) {
				WriteToFile(formatted);
			}
		}

		public void Dispose() {
			Application.logMessageReceivedThreaded -= HandleUnityLog;
		}

		private void OpenLogFile() {
			lock (_fileLock) {
				try {
					_logFilePath = Path.Combine(Application.persistentDataPath, "game_log.txt");
					string directory = Path.GetDirectoryName(_logFilePath);
					if (directory != null && !Directory.Exists(directory)) {
						Directory.CreateDirectory(directory);
					}

					_fileStream = new StreamWriter(_logFilePath, true, Encoding.UTF8) { AutoFlush = true };
					Debug.Log($"Log file initialized: {_logFilePath}");
				} catch (Exception e) {
					Debug.LogError($"Failed to initialize log file: {e.Message}");
				}
			}
		}

		private void CloseLogFile() {
			lock (_fileLock) {
				_fileStream?.Close();
				_fileStream = null;
			}
		}

		private void RotateLogFiles() {
			lock (_fileLock) {
				CloseLogFile();

				// Delete oldest backup
				var oldestBackup = $"{_logFilePath}.{Config.maxBackupCount}";
				if (File.Exists(oldestBackup)) {
					File.Delete(oldestBackup);
				}

				// Shift existing backups
				for (int i = Config.maxBackupCount - 1; i >= 1; i--) {
					var currentBackup = $"{_logFilePath}.{i}";
					var newBackup = $"{_logFilePath}.{i + 1}";
					if (File.Exists(currentBackup)) {
						File.Move(currentBackup, newBackup);
					}
				}

				// Rename current to first backup
				File.Move(_logFilePath, $"{_logFilePath}.1");

				OpenLogFile();
			}
		}

		private void WriteToFile(string message) {
			if (!Config.enableFileLogging || _fileStream == null) return;

			lock (_fileLock) {
				try {
					_fileStream.WriteLine(message);

					// Check file size and rotate if needed
					if (_fileStream.BaseStream.Length > Config.maxLogFileSize) {
						RotateLogFiles();
					}
				} catch (Exception e) {
					Debug.LogError($"Log write failed: {e.Message}");
				}
			}
		}

		private string FormatMessage(LogLevel level, string message, string sourceFile, int sourceLineNumber, string memberName) {
			string color = level switch {
				LogLevel.Info => "green",
				LogLevel.Warning => "yellow",
				LogLevel.Error => "red",
				var _ => string.Empty
			};

			return Config.includeTimestamp
				? $"[{DateTime.Now.ToString("yyyy-MM-dd").RichColor(color).RichItalic().RichBold()}][{Path.GetFileName(sourceFile).RichColor(color).RichItalic().RichBold()}:{sourceLineNumber.ToString().RichColor(color).RichItalic()} -  {memberName.RichColor(color).RichItalic().RichBold()}] {message.RichColor(color).RichBold()}"
				: $"[{Path.GetFileName(sourceFile).RichColor(color).RichItalic()}:{sourceLineNumber.ToString().RichColor(color).RichItalic().RichBold()} -  {memberName.RichColor(color).RichItalic().RichBold()}] {message.RichColor(color).RichBold()}";
		}


		// Handle Unity's native logs
		private void HandleUnityLog(string logString, string stackTrace, LogType type) {
			if (type == LogType.Exception) {
				Error($"{logString}\n{stackTrace}");
			}
		}
	}
}