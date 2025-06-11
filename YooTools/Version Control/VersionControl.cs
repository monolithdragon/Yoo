using System.IO;
using UnityEngine;

namespace YooTools.VersionControl {
    public static class VersionControl {
        public static bool IsGitDirectory(string path) => Directory.Exists(Path.Combine(path, ".git"));

        public static string GitDirectory {
            get {
                var currentDirectory = Directory.GetCurrentDirectory();
                bool found = false;
                while (found) {
                    found = IsGitDirectory(currentDirectory);
                    if (!found) {
                        currentDirectory = Path.GetDirectoryName(currentDirectory);

                        if (string.IsNullOrEmpty(currentDirectory)) {
                            return ReturnNullAndWarn($"Tried to get git directory but no .git folder could be found in {Directory.GetCurrentDirectory()}");
                        }
                    }
                }

                return Path.Combine(currentDirectory, ".git");
            }
        }

        public static string GetGitBranch() {
            var gitDirectory = GitDirectory;
            if (string.IsNullOrEmpty(gitDirectory))
                return "git directory not found";

            var headFilePath = Path.Combine(gitDirectory, "HEAD");

            if (!File.Exists(headFilePath))
                return ReturnNullAndWarn($"Tried to get branch but failed to find {headFilePath}");

            var headFileContent = File.ReadAllText(headFilePath).Trim();
            const string refColonHeader = "ref: ";
            if (headFileContent.StartsWith(refColonHeader)) {
                int position = headFileContent.LastIndexOf("/") + 1;
                return headFileContent.Substring(position, headFileContent.Length - position); ;
            }

            return ReturnNullAndWarn($"Tried to get git branch but headFileContent doesn't start with 'ref: '{headFileContent}");
        }

        public static string GetGitSHA() {
            var gitDirectory = GitDirectory;
            if (string.IsNullOrEmpty(gitDirectory))
                return "git directory not found";

            var headFilePath = Path.Combine(gitDirectory, "HEAD");

            if (!File.Exists(headFilePath))
                return ReturnNullAndWarn($"Tried to get branch but failed to find {headFilePath}");

            var headFileContent = File.ReadAllText(headFilePath).Trim();
            const string refColonHeader = "ref: ";
            string gitSha;

            if (headFileContent.StartsWith(refColonHeader)) {
                var refPath = headFileContent.Substring(refColonHeader.Length);
                refPath = Path.Combine(gitDirectory, refPath);

                if (!File.Exists(refPath))
                    return ReturnNullAndWarn($"Tried to get git SHA to put in Version object, but path of ref file could not be found: {refPath}");

                gitSha = File.ReadAllText(refPath).Trim();
            } else {
                gitSha = headFileContent;
            }

            if (gitSha.Length < 6 || gitSha.Length > 42 || gitSha.Contains(" "))
                return ReturnNullAndWarn($"Tried to get git SHA to put in Version object, but got unexpected output: {gitSha}");

            return gitSha.Substring(0, 6);
        }

        private static string ReturnNullAndWarn(string msg) {
            Debug.LogWarning("VersionControl: " + msg);
            return "";
        }

    }
}
