using System.Collections.Generic;

namespace YooTools.ImportPackage {
	public class Package {
		public string Name { get; }
		public Queue<string> Packages { get; }

		public Package(params string[] names) {
			Packages = new Queue<string>();

			foreach (string name in names) {
				Name = name;
				Packages.Enqueue(name);
			}
		}
	}
}