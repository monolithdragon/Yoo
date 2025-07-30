using UnityEngine;

namespace YooX.Timer {
	/// <summary>
	/// A Timer that counts up from zero to infinity.  
	/// Great for measuring durations.
	/// </summary>
	public class StopwatchTimer() : Timer(0) {
		public override bool IsFinished => false;

		public override void Tick() {
			if (IsRunning) {
				CurrentTime += Time.deltaTime;
			}
		}
	}

}