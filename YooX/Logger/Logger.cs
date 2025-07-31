using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace YooX {
	public class Logger : Singleton<Logger> {
		[SerializeField] private LoggerConfig config;
		
		private StreamWriter _fileStream;
		private string _logFilePath;
		private readonly object _fileLock = new();

		private void Start() {
			if (config.enableFileLogging) {
				OpenLogFile();
			}

			Application.logMessageReceivedThreaded += HandleUnityLog;
		}

		public static void Info(string message) => Log(LogLevel.Info, message);
		public static void Warning(string message) => Log(LogLevel.Warning, message);
		public static void Error(string message) => Log(LogLevel.Error, message);
		public static void Log(LogLevel level, string message) => Instance.LogInternal(level, message);

		public void Dispose() {
			Application.logMessageReceivedThreaded -= HandleUnityLog;
		}

		private void OpenLogFile() {
			lock (_fileLock) {
				try {
					_logFilePath = Path.Combine(Application.persistentDataPath, "game_log.txt");
					var directory = Path.GetDirectoryName(_logFilePath);
					if (directory != null && !Directory.Exists(directory)) {
						Directory.CreateDirectory(directory);
					}

					_fileStream = new StreamWriter(_logFilePath, true, Encoding.UTF8) {
						AutoFlush = true
					};
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
				var oldestBackup = $"{_logFilePath}.{config.maxBackupCount}";
				if (File.Exists(oldestBackup)) {
					File.Delete(oldestBackup);
				}

				// Shift existing backups
				for (var i = config.maxBackupCount - 1; i >= 1; i--) {
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
			if (!config.enableFileLogging || _fileStream == null) return;

			lock (_fileLock) {
				try {
					_fileStream.WriteLine(message);

					// Check file size and rotate if needed
					if (_fileStream.BaseStream.Length > config.maxLogFileSize) {
						RotateLogFiles();
					}
				} catch (Exception e) {
					Debug.LogError($"Log write failed: {e.Message}");
				}
			}
		}

		private string FormatMessage(LogLevel level, string message, string color, string sourceFile, int sourceLineNumber, string memberName) =>
			config.includeTimestamp ?
				$"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}][{Path.GetFileName(sourceFile.RichColor(color).RichItalic())}:{sourceLineNumber.ToString().RichColor(color).RichItalic()} -  {memberName.RichColor(color).RichItalic()}][{level.ToString().RichColor(color).RichItalic()}] {message.RichColor(color).RichBold()}" :
				$"[{Path.GetFileName(sourceFile.RichColor(color).RichItalic())}:{sourceLineNumber.ToString().RichColor(color).RichItalic()} -  {memberName.RichColor(color).RichItalic()}][{level.ToString().RichColor(color).RichItalic()}] {message.RichColor(color).RichBold()}";

		// Handle Unity's native logs
		private void HandleUnityLog(string logString, string stackTrace, LogType type) {
			if (type == LogType.Exception) {
				LogInternal(LogLevel.Error, $"{logString}\n{stackTrace}");
			}
		}

		// Core logging method

		private void LogInternal(LogLevel level,
		                         string message,
		                         [CallerMemberName] string memberName = "",
		                         [CallerFilePath] string sourceFile = "",
		                         [CallerLineNumber] int sourceLineNumber = 0) {
			if (level < config.minLogLevel) return;

			var color = level switch {
				LogLevel.Info => "green",
				LogLevel.Warning => "yellow",
				LogLevel.Error => "red",
				_ => string.Empty
			};

			var formatted = FormatMessage(level, message, color, sourceFile, sourceLineNumber, memberName);

			// Unity console
			switch(level) {
				case LogLevel.Error:
					Debug.LogError(formatted);
					break;
				case LogLevel.Warning:
					Debug.LogWarning(formatted);
					break;
				case LogLevel.Info:
					Debug.Log(formatted);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(level), level, null);
			}

			// File output
			if (config.enableFileLogging) {
				WriteToFile(formatted);
			}
		}
	}
}