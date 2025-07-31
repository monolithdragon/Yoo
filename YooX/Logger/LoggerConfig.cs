using UnityEngine;

namespace YooX.Logger {
	[CreateAssetMenu(fileName = "LoggerConfig", menuName = "YooX/Logger Config", order = 0)]
	public class LoggerConfig : ScriptableObject {
		public bool enableFileLogging = false;
		public bool includeTimestamp = true;
		public LogLevel minLogLevel = LogLevel.Info;
		public long maxLogFileSize = 5 * 1024 * 1024;// 5MB
		public int maxBackupCount = 3;
	}
}