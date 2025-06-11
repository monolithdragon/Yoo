using System.Collections.Generic;

namespace YooX.Timer {
    /// <summary>
    /// Manages the registration and updating of timers.
    /// </summary>
    public static class TimerManager {
        /// <summary>
        /// List of active timers.
        /// </summary>
        private static readonly List<Timer> _timers = new List<Timer>();

        /// <summary>
        /// Temporary list used for sweeping and updating timers.
        /// </summary>
        private static readonly List<Timer> _sweep = new List<Timer>();

        /// <summary>
        /// Registers a timer by adding it to the list of active timers.
        /// </summary>
        /// <param name="timer">The timer to register.</param>
        public static void RegisterTimer(Timer timer) => _timers.Add(timer);

        /// <summary>
        /// Removes a timer from the list of active timers.
        /// </summary>
        /// <param name="timer">The timer to remove.</param>
        public static void RemoveTimer(Timer timer) => _timers.Remove(timer);

        /// <summary>
        /// Updates all active timers by invoking their Tick method.
        /// </summary>
        public static void UpdateTimers() {
            if (_timers.Count == 0)
                return;

            // Update the sweep list with active timers.
            _sweep.RefreshWith(_timers);
            foreach (var timer in _sweep) {
                // Call the Tick method for each timer.
                timer.Tick();
            }
        }

        /// <summary>
        /// Clears all active timers and disposes them.
        /// </summary>
        public static void Clear() {
            // Copy all timers to the sweep list.
            _sweep.RefreshWith(_timers);
            foreach (var timer in _sweep) {
                timer.Dispose();
            }

            _timers.Clear();
            _sweep.Clear();
        }
    }

}
