using System.Collections.Generic;
using System.Text;

namespace UnityEngine.LowLevel {
	/// <summary>
	/// Utility class for managing PlayerLoopSystem operations, such as inserting, 
	/// removing, and printing systems in the Unity player loop.
	/// </summary>
	static public class PlayerLoopUtils {
		/// <summary>
		/// Removes a specified PlayerLoopSystem from the given loop if it matches the provided system.
		/// </summary>
		/// <typeparam name="T">The type of the system to remove.</typeparam>
		/// <param name="loop">The PlayerLoopSystem to search and remove from.</param>
		/// <param name="systemRemove">The system to remove from the loop.</param>
		static public void RemoveSystem<T>(ref PlayerLoopSystem loop, in PlayerLoopSystem systemRemove) {
			if (loop.subSystemList == null) {
				return;
			}

			var playerLoopSystemList = new List<PlayerLoopSystem>(loop.subSystemList);

			for (var i = 0; i < playerLoopSystemList.Count; i++) {
				if (playerLoopSystemList[i].type == systemRemove.type && playerLoopSystemList[i].updateDelegate == systemRemove.updateDelegate) {
					playerLoopSystemList.RemoveAt(i);
					loop.subSystemList = playerLoopSystemList.ToArray();

					return;
				}
			}

			HandleSubSystemLoopForRemoval<T>(ref loop, systemRemove);
		}

		/// <summary>
		/// Inserts a PlayerLoopSystem at a specified index in the given loop, 
		/// if the loop matches the type <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T">The type of the system to insert into.</typeparam>
		/// <param name="loop">The PlayerLoopSystem to insert into.</param>
		/// <param name="systemToInsert">The system to insert into the loop.</param>
		/// <param name="index">The index at which to insert the system.</param>
		/// <returns>True if the system is inserted, otherwise false.</returns>
		static public bool InsertSystem<T>(ref PlayerLoopSystem loop, in PlayerLoopSystem systemToInsert, int index) {
			if (loop.type != typeof(T)) {
				return HandleSubSystemLoop<T>(ref loop, systemToInsert, index);
			}

			var playerLoopSystemList = new List<PlayerLoopSystem>();

			if (loop.subSystemList != null) {
				playerLoopSystemList.AddRange(loop.subSystemList);
			}

			playerLoopSystemList.Insert(index, systemToInsert);
			loop.subSystemList = playerLoopSystemList.ToArray();

			return true;
		}

		/// <summary>
		/// Prints the structure of the PlayerLoopSystem, showing its subsystems recursively.
		/// </summary>
		/// <param name="loop">The root PlayerLoopSystem to print.</param>
		static public void PrintPlayerLoop(PlayerLoopSystem loop) {
			var sb = new StringBuilder();
			sb.AppendLine("Unity Player Loop");

			foreach (var subsystem in loop.subSystemList) {
				PrintSubSystem(subsystem, sb, 0);
			}

			Debug.Log(sb.ToString());
		}

		/// <summary>
		/// Helper method to remove a system from sub-loops recursively.
		/// </summary>
		/// <typeparam name="T">The type of the system to remove.</typeparam>
		/// <param name="loop">The loop to search through.</param>
		/// <param name="systemRemove">The system to remove.</param>
		private static void HandleSubSystemLoopForRemoval<T>(ref PlayerLoopSystem loop, PlayerLoopSystem systemRemove) {
			if (loop.subSystemList == null) {
				return;
			}

			for (var i = 0; i < loop.subSystemList.Length; ++i) {
				RemoveSystem<T>(ref loop.subSystemList[i], systemRemove);
			}
		}

		/// <summary>
		/// Helper method to insert a system into sub-loops recursively.
		/// </summary>
		/// <typeparam name="T">The type of the system to insert into.</typeparam>
		/// <param name="loop">The loop to search through.</param>
		/// <param name="systemToInsert">The system to insert.</param>
		/// <param name="index">The index at which to insert the system.</param>
		/// <returns>True if the system is inserted, otherwise false.</returns>
		private static bool HandleSubSystemLoop<T>(ref PlayerLoopSystem loop, PlayerLoopSystem systemToInsert, int index) {
			if (loop.subSystemList == null) {
				return false;
			}

			for (var i = 0; i < loop.subSystemList.Length; ++i) {
				if (!InsertSystem<T>(ref loop.subSystemList[i], in systemToInsert, index)) {
					continue;
				}

				return true;
			}

			return false;
		}

		/// <summary>
		/// Helper method to print the details of a subsystem recursively.
		/// </summary>
		/// <param name="system">The PlayerLoopSystem to print.</param>
		/// <param name="sb">The StringBuilder to append output to.</param>
		/// <param name="level">The current level of recursion for indentation.</param>
		private static void PrintSubSystem(PlayerLoopSystem system, StringBuilder sb, int level) {
			sb.Append(' ', level * 2).AppendLine(system.type.ToString());

			if (system.subSystemList == null || system.subSystemList.Length == 0) {
				return;
			}

			foreach (var subsystem in system.subSystemList) {
				PrintSubSystem(subsystem, sb, level + 1);
			}
		}
	}
}