using System.Threading.Tasks;
using UnityEngine;

namespace YooX {
	static public class AsyncOperationExtensions {
		/// <summary>
		/// Converts a Unity <see cref="AsyncOperation"/> to a <see cref="Task"/> that completes when the operation is done.
		/// </summary>
		/// <param name="asyncOperation">The Unity async operation to await.</param>
		/// <returns>A task that completes when the async operation is finished.</returns>
		static public Task AsTask(this AsyncOperation asyncOperation) {
			var taskCompleteSource = new TaskCompletionSource<bool>();
			asyncOperation.completed += _ => taskCompleteSource.SetResult(true);

			return taskCompleteSource.Task;
		}
	}
}