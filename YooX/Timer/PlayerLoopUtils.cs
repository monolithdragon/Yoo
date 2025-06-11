using System.Collections.Generic;
using System.Text;

namespace UnityEngine.LowLevel {
    /// <summary>
    /// Utility class for managing PlayerLoopSystem operations, such as inserting, 
    /// removing, and printing systems in the Unity player loop.
    /// </summary>
    public static class PlayerLoopUtils {
        /// <summary>
        /// Removes a specified PlayerLoopSystem from the given loop if it matches the provided system.
        /// </summary>
        /// <typeparam name="T">The type of the system to remove.</typeparam>
        /// <param name="loop">The PlayerLoopSystem to search and remove from.</param>
        /// <param name="systemRemove">The system to remove from the loop.</param>
        public static void RemoveSystem<T>(ref PlayerLoopSystem loop, in PlayerLoopSystem systemRemove) {
            if (loop.subSystemList == null)
                return;

            var playerLoppSystemList = new List<PlayerLoopSystem>(loop.subSystemList);
            for (int i = 0; i < playerLoppSystemList.Count; i++) {
                if ((playerLoppSystemList[i].type == systemRemove.type) && (playerLoppSystemList[i].updateDelegate == systemRemove.updateDelegate)) {
                    playerLoppSystemList.RemoveAt(i);
                    loop.subSystemList = playerLoppSystemList.ToArray();
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
        public static bool InsertSystem<T>(ref PlayerLoopSystem loop, in PlayerLoopSystem systemToInsert, int index) {
            if (loop.type != typeof(T))
                return HandleSubSystemLoop<T>(ref loop, systemToInsert, index);

            var playerLoppSystemList = new List<PlayerLoopSystem>();
            if (loop.subSystemList != null)
                playerLoppSystemList.AddRange(loop.subSystemList);
            playerLoppSystemList.Insert(index, systemToInsert);
            loop.subSystemList = playerLoppSystemList.ToArray();
            return true;
        }

        /// <summary>
        /// Prints the structure of the PlayerLoopSystem, showing its subsystems recursively.
        /// </summary>
        /// <param name="loop">The root PlayerLoopSystem to print.</param>
        public static void PrintPlayerLoop(PlayerLoopSystem loop) {
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
            if (loop.subSystemList == null)
                return;

            for (int i = 0; i < loop.subSystemList.Length; ++i) {
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
            if (loop.subSystemList == null)
                return false;

            for (int i = 0; i < loop.subSystemList.Length; ++i) {
                if (!InsertSystem<T>(ref loop.subSystemList[i], in systemToInsert, index))
                    continue;
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
            if ((system.subSystemList == null) || (system.subSystemList.Length == 0))
                return;

            foreach (var subsystem in system.subSystemList) {
                PrintSubSystem(subsystem, sb, level + 1);
            }
        }
    }

}
