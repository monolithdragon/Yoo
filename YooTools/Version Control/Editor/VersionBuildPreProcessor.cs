using System;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace YooTools.VersionControl {
	[InitializeOnLoad]
	public class VersionBuildPreProcessor : IPreprocessBuildWithReport {
		public int callbackOrder => 0;

		static VersionBuildPreProcessor() { }

		public void OnPreprocessBuild(BuildReport report) {
			var versionSo = CurrentVersion.Instance;
			string version = versionSo.version.ToVersion();
			PlayerSettings.bundleVersion = version;

			int bundleVersionCode = versionSo.version.major * 100000 + versionSo.version.minor * 1000 + versionSo.version.build;
			PlayerSettings.Android.bundleVersionCode = bundleVersionCode;
			PlayerSettings.macOS.buildNumber = $"{bundleVersionCode}00";

			UpdateCurrentVersion(versionSo);
			EditorUtility.SetDirty(versionSo);
			AssetDatabase.SaveAssets();
		}

		private void UpdateCurrentVersion(CurrentVersion currentVersion) {
			if (currentVersion == null) {
				return;
			}

			if (VersionControl.GitDirectory != null) {
				currentVersion.version.gitBranch = VersionControl.GetGitBranch();
				currentVersion.version.gitCommitSHA = VersionControl.GetGitSHA();
			}

			currentVersion.version.buildTarget = EditorUserBuildSettings.activeBuildTarget.ToString();
			currentVersion.version.isDevelopment = Debug.isDebugBuild;
			currentVersion.version.buildDateTime = DateTime.Now.ToString("u");
		}
	}
}