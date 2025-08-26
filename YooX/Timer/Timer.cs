using System;
using UnityEngine;

namespace YooX.Timer {
	/// <summary>
	/// Represents an timer that tracks time progression and provides basic timer functionality.
	/// </summary>    
	public abstract class Timer : IDisposable {
		#region Fields

		/// <summary>
		/// The initial time set for the timer.
		/// </summary>
		private float _initialTime;

		/// <summary>
		/// Indicates if the object has been disposed.
		/// </summary>
		private bool _disposedValue;

		#endregion

		#region Properties

		/// <summary>
		/// The current remaining time on the timer.
		/// </summary>
		public float CurrentTime { get; protected set; }

		/// <summary>
		/// Indicates whether the timer is currently running.
		/// </summary>
		public bool IsRunning { get; protected set; }

		/// <summary>
		/// The progress of the timer as a value between 0 and 1.
		/// </summary>
		public float Progress => Mathf.Clamp(CurrentTime / _initialTime, 0f, 1f);

		/// <summary>
		/// Determine if the timer has finished.
		/// </summary>
		public abstract bool IsFinished { get; }

		#endregion

		#region Delegates

		/// <summary>
		/// Event triggered when the timer starts.
		/// </summary>
		public Action OnTimerStart = delegate { };

		/// <summary>
		/// Event triggered when the timer stops.
		/// </summary>
		public Action OnTimerStop = delegate { };

		#endregion

		#region Methods

		protected Timer(float initialTime) => _initialTime = initialTime;

		/// <summary>
		/// Starts the timer and invokes the OnTimerStart event.
		/// </summary>
		public void Start() {
			CurrentTime = _initialTime;

			if (!IsRunning) {
				IsRunning = true;
				TimerManager.RegisterTimer(this);
				OnTimerStart.Invoke();
			}
		}

		/// <summary>
		/// Stops the timer and invokes the OnTimerStop event.
		/// </summary>
		public void Stop() {
			if (IsRunning) {
				IsRunning = false;
				TimerManager.RemoveTimer(this);
				OnTimerStop.Invoke();
			}
		}

		/// <summary>
		/// Resumes the timer without resetting the time.
		/// </summary>
		public void Resume() => IsRunning = true;

		/// <summary>
		/// Pauses the timer without resetting the time.
		/// </summary>
		public void Pause() => IsRunning = false;

		/// <summary>
		/// Resets the timer to the initial time.
		/// </summary>
		public void Reset() => CurrentTime = _initialTime;

		/// <summary>
		/// Resets the timer with a new initial time.
		/// </summary>
		/// <param name="value">The new initial time.</param>
		public void Reset(float value) {
			_initialTime = value;
			Reset();
		}

		/// <summary>
		/// Define how the timer progresses.
		/// </summary>
		public abstract void Tick();

		#endregion

		#region IDisposable

		/// <summary>
		/// Disposes of the timer, unregistering it and releasing resources.
		/// </summary>
		/// <param name="disposing">Indicates if managed resources should be disposed.</param>
		virtual protected void Dispose(bool disposing) {
			if (_disposedValue) {
				return;
			}

			if (disposing) {
				TimerManager.RemoveTimer(this);
			}

			_disposedValue = true;
		}

		/// <summary>
		/// Finalizer to ensure the timer is disposed properly.
		/// </summary>
		~Timer() => Dispose(false);

		/// <summary>
		/// Public method to dispose the timer and suppress finalization.
		/// </summary>
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}