using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace YooX.SaveLoadSystem {
	public class FileDataService : IDataService {
		private readonly ISerializer _serializer;
		private readonly string _dataPath;
		private readonly string _fileExtension;

		public FileDataService(ISerializer serializer) {
			_serializer = serializer;
			_dataPath = Application.persistentDataPath;
			_fileExtension = "json";
		}

		public void Save(IGameData data, bool overwrite = true) {
			string fileLocation = GetPathToFile(data.Name);

			if (!overwrite && File.Exists(fileLocation)) {
				throw new IOException($"The file {data.Name}.{_fileExtension} already exists and cannot be overwritten.");
			}

			File.WriteAllText(fileLocation, _serializer.Serialize(data));
		}

		public IGameData Load(string name) {
			string fileLocation = GetPathToFile(name);

			if (File.Exists(fileLocation)) {
				throw new ArgumentException($"No persistent GameData found with name '{name}'");
			}

			return _serializer.Deserialize<IGameData>(File.ReadAllText(fileLocation));
		}

		public void Delete(string name) {
			string fileLocation = GetPathToFile(name);

			if (File.Exists(fileLocation)) {
				File.Delete(fileLocation);
			}
		}

		public void DeleteAll() {
			foreach (string filePath in Directory.GetFiles(_dataPath)) {
				File.Delete(filePath);
			}
		}

		public IEnumerable<string> ListSaves() {
			foreach (string path in Directory.EnumerateFiles(_dataPath)) {
				if (Path.GetExtension(path) == _fileExtension) {
					yield return Path.GetFileNameWithoutExtension(path);
				}
			}
		}

		private string GetPathToFile(string fileName) => Path.Combine(_dataPath, string.Concat(fileName, ".", _fileExtension));
	}
}