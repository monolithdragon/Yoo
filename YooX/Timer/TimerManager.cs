using System.Collections.Generic;

namespace YooX.Timer {
	/// <summary>
	/// Manages the registration and updating of timers.
	/// </summary>
	static public class TimerManager {
		/// <summary>
		/// List of active timers.
		/// </summary>
		private static readonly List<Timer> Timers = new();

		/// <summary>
		/// Temporary list used for sweeping and updating timers.
		/// </summary>
		private static readonly List<Timer> Sweep = new();

		/// <summary>
		/// Registers a timer by adding it to the list of active timers.
		/// </summary>
		/// <param name="timer">The timer to register.</param>
		static public void RegisterTimer(Timer timer) => Timers.Add(timer);

		/// <summary>
		/// Removes a timer from the list of active timers.
		/// </summary>
		/// <param name="timer">The timer to remove.</param>
		static public void RemoveTimer(Timer timer) => Timers.Remove(timer);

		/// <summary>
		/// Updates all active timers by invoking their Tick method.
		/// </summary>
		static public void UpdateTimers() {
			if (Timers.Count == 0) {
				return;
			}

			// Update the sweep list with active timers.
			Sweep.RefreshWith(Timers);

			foreach (var timer in Sweep) {
				// Call the Tick method for each timer.
				timer.Tick();
			}
		}

		/// <summary>
		/// Clears all active timers and disposes them.
		/// </summary>
		static public void Clear() {
			// Copy all timers to the sweep list.
			Sweep.RefreshWith(Timers);

			foreach (var timer in Sweep) {
				timer.Dispose();
			}

			Timers.Clear();
			Sweep.Clear();
		}
	}
}