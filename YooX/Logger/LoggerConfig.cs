using UnityEngine;

namespace YooX {
	[CreateAssetMenu(fileName = "LoggerConfig", menuName = "YooX/LoggerConfig", order = 1)]
	public class LoggerConfig : ScriptableObject {
		public bool enableFileLogging = false;
		public bool includeTimestamp = false;
		public bool isDevelopment = true;
		public long maxLogFileSize = 5 * 1024 * 1024; // 5MB
		public int maxBackupCount = 3;
	}
}