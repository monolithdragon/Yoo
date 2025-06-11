using System.Collections.Generic;
using System.IO;

namespace YooTools.ProjectFolders {
    public class Folder {
        public readonly List<Folder> Folders;

        public string Name { get; }
        public string CurrentFolder { get; }
        public string ParentFolder { get; }

        public Folder(string name, string parentFolder) {
            Name = name;
            ParentFolder = parentFolder;
            CurrentFolder = parentFolder != string.Empty ? ParentFolder + Path.DirectorySeparatorChar + Name : Name;
            Folders = new List<Folder>();
        }

        /// <summary>
        /// Add new folder.
        /// </summary>
        /// <param name="name">Name of folder</param>
        /// <returns>The new folder.</returns>
        public Folder Add(string name) {
            var folder = ParentFolder.Length > 0 ? new Folder(name, ParentFolder + Path.DirectorySeparatorChar + Name)
                : new Folder(name, Name);
            Folders.Add(folder);
            return folder;
        }
    }
}
