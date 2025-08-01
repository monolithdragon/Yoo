using System.Collections.Generic;

namespace YooX.SaveLoadSystem {
	public interface IGameData {
		string Name { get; set; }
		string CurrentLevelName { get; set; }
		List<ISaveable> Data { get; set; }
	}
}