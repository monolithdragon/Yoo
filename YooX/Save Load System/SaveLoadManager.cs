using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace YooX.SaveLoadSystem {
	public class SaveLoadManager<T> : PersistentSingleton<SaveLoadManager<T>> where T : MonoBehaviour, IBind<ISaveable> {
		[SerializeField] private IGameData gameData;

		private IDataService _dataService;

		protected override void Awake() {
			base.Awake();

			_dataService = new FileDataService(new JsonSerializer());
		}

		private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
		private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

		public void NewGame() => SceneManager.LoadScene(gameData.CurrentLevelName);

		public void SaveGame() => _dataService.Save(gameData);

		public void LoadGame(string gameName) {
			gameData = _dataService.Load(gameName);

			if (string.IsNullOrWhiteSpace(gameData.CurrentLevelName)) {
				gameData.CurrentLevelName = "Demo";
			}

			SceneManager.LoadScene(gameData.CurrentLevelName);
		}

		public void ReloadGame() => LoadGame(gameData.Name);
		public void DeleteGame(string gameName) => _dataService.Delete(gameName);

		private void Bind([NotNull] ISaveable data) {
			var entity = FindObjectsByType<T>(FindObjectsSortMode.None).FirstOrDefault();

			if (entity != null) {
				entity.Bind(data);
			}
		}

		private void Bind(List<ISaveable> dataList) {
			var entities = FindObjectsByType<T>(FindObjectsSortMode.None);

			foreach (var entity in entities) {
				var data = dataList.FirstOrDefault(d => d.Id == entity.Id);

				if (data == null) {
					continue;
				}

				entity.Bind(data);
			}
		}

		private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
			if (scene.name == "MainMenu") {
				return;
			}

			Bind(gameData.Data);
		}
	}
}