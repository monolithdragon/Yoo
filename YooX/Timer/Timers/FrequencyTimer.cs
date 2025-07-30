using System;
using UnityEngine;

namespace YooX.Timer {
	/// <summary>
	/// A Timer that triggers a tick event at a specified frequency.
	/// Inherits from the base Timer class and supports triggering actions at regular intervals.
	/// </summary>
	public class FrequencyTimer : Timer {
		private float _timeThreshold;

		/// <summary>
		/// Number of ticks per second the timer will execute.
		/// </summary>
		public int TicksPerSecond { get; private set; }

		public override bool IsFinished => !IsRunning;

		/// <summary>
		/// Event that triggers on each tick based on the timer's frequency.
		/// </summary>
		public readonly Action OnTick = delegate { };

		/// <summary>
		/// Constructor that initializes the timer with a specified ticks-per-second rate.
		/// </summary>
		/// <param name="ticksPerSecond">Number of ticks per second.</param>
		public FrequencyTimer(int ticksPerSecond) : base(0) {
			CalculateTimeThreshold(ticksPerSecond);
		}

		public override void Tick() {
			// Triggering the OnTick event when the threshold is met.
			if (IsRunning && CurrentTime >= _timeThreshold) {
				CurrentTime -= _timeThreshold;
				OnTick.Invoke();
			}

			// Accumulate time.
			if (IsRunning && CurrentTime < _timeThreshold) {
				CurrentTime += Time.deltaTime;
			}
		}

		/// <summary>
		/// Resets the timer with a new ticks-per-second rate and resets the internal state.
		/// </summary>
		/// <param name="newTicksPerSecond">New ticks per second value.</param>
		public void Reset(int newTicksPerSecond) {
			CalculateTimeThreshold(newTicksPerSecond);
			Reset();
		}

		/// <summary>
		/// Calculates the time threshold for each tick based on the number of ticks per second.
		/// </summary>
		/// <param name="ticksPerSecond">Number of ticks per second.</param>
		private void CalculateTimeThreshold(int ticksPerSecond) {
			TicksPerSecond = ticksPerSecond;
			_timeThreshold = 1f / TicksPerSecond;
		}
	}

}