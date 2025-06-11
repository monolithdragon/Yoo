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
            package = new Package("https://github.com/KyleBanks/scene-ref-attribute.git");
            InstallPackages();
        }

        private static void InstallPackages() {
            if (package.Packages.Count > 0) {
                StartNextPackageInstallation();
            }
        }

        private async static void StartNextPackageInstallation() {
            request = Client.Add(package.Packages.Dequeue());

            while (!request.IsCompleted)
                await Task.Delay(10);

            if (request.Status == StatusCode.Success)
                Debug.Log($"Installed: {request.Result.packageId}");
            else if (request.Status == StatusCode.Failure)
                Debug.LogError(request.Error.message);

            if (package.Packages.Count > 0) {
                await Task.Delay(1000);
                StartNextPackageInstallation();
            }
        }

    }
}
