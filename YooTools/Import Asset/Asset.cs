using System;

namespace YooTools.ImportAsset {
	public class Asset {
		public string Name { get; }
		public string SubFolder { get; }
		public string StorePath { get; }

		public Asset(string assetName, string subFolder) {
			Name = assetName;
			SubFolder = subFolder;
			StorePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/Unity/Asset Store-5.x";
		}
	}
}