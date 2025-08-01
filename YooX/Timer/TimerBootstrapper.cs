using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace YooX.Timer {
	/// <summary>
	/// Bootstrapper class for initializing and managing TimerManager in Unity's PlayerLoop.
	/// Automatically registers TimerManager into the update loop when assemblies are loaded.
	/// </summary>
	public static class TimerBootstrapper {
		/// <summary>
		/// PlayerLoopSystem that holds the TimerManager for updating timers.
		/// </summary>
		private static PlayerLoopSystem _timerSystem;

		/// <summary>
		/// Initializes the TimerManager by inserting it into the Unity update loop.
		/// This method is called after all assemblies have loaded.
		/// </summary>
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
		public static void Initialize() {
			var currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();

			// Insert TimerManager into the Update loop
			if (!InsertTimerManager<Update>(ref currentPlayerLoop, 0)) {
				Debug.LogWarning("Timers not initialized, unable to register TimerManager into the Update loop");
				return;
			}

			PlayerLoop.SetPlayerLoop(currentPlayerLoop);

#if UNITY_EDITOR
            // Handle play mode state changes in the editor to remove TimerManager on exiting play mode
            EditorApplication.playModeStateChanged -= OnPlayModeState;
            EditorApplication.playModeStateChanged += OnPlayModeState;

            static void OnPlayModeState(PlayModeStateChange state) {
                if (state == PlayModeStateChange.ExitingPlayMode) {
                    var currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
                    RemoveTimerManager<Update>(ref currentPlayerLoop);
                    PlayerLoop.SetPlayerLoop(currentPlayerLoop);

                    // Clear timers on exiting play mode
                    TimerManager.Clear();
                }
            }
#endif
		}

		/// <summary>
		/// Removes the TimerManager from the specified loop.
		/// </summary>
		/// <typeparam name="T">The type of the loop where TimerManager is removed from.</typeparam>
		/// <param name="loop">The PlayerLoopSystem to remove from.</param>
		private static void RemoveTimerManager<T>(ref PlayerLoopSystem loop) {
			PlayerLoopUtils.RemoveSystem<T>(ref loop, in _timerSystem);
		}

		/// <summary>
		/// Inserts the TimerManager into the specified loop at the given index.
		/// </summary>
		/// <typeparam name="T">The type of the loop where TimerManager is inserted.</typeparam>
		/// <param name="loop">The PlayerLoopSystem to insert into.</param>
		/// <param name="index">The index to insert TimerManager at.</param>
		/// <returns>True if TimerManager is successfully inserted, otherwise false.</returns>
		private static bool InsertTimerManager<T>(ref PlayerLoopSystem loop, int index) {
			_timerSystem = new PlayerLoopSystem {
				type = typeof(TimerManager),
				updateDelegate = TimerManager.UpdateTimers,
				subSystemList = null
			};

			return PlayerLoopUtils.InsertSystem<T>(ref loop, in _timerSystem, index);
		}
	}

}