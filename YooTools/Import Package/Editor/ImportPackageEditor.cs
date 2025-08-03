using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace YooTools.ImportPackage {
	public class ImportPackageEditor : Editor {
		private static AddRequest request;
		private static Package package;

		[MenuItem("Yoo Tools/Install Essentials Packages")]
		public static void Execute() {
			package = new Package("https://github.com/KyleBanks/scene-ref-attribute.git", "com.unity.project-auditor");
			InstallPackages();
		}

		private static void InstallPackages() {
			if (package.Packages.Count > 0) {
				StartNextPackageInstallation();
			}
		}

		private static async void StartNextPackageInstallation() {
			request = Client.Add(package.Packages.Dequeue());

			while(!request.IsCompleted) await Task.Delay(10);

			switch(request.Status) {
				case StatusCode.Success:
					Debug.Log($"Installed: {request.Result.packageId}");
					break;
				case StatusCode.Failure:
					Debug.LogError(request.Error.message);
					break;
				case StatusCode.InProgress:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			if (package.Packages.Count > 0) {
				await Task.Delay(1000);
				StartNextPackageInstallation();
			}
		}

	}
}