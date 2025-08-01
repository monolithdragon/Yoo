using System.Collections.Generic;

namespace YooX.SaveLoadSystem {
	public interface IDataService {
		void Save(IGameData data, bool overwrite = true);
		IGameData Load(string name);
		void Delete(string name);
		void DeleteAll();
		IEnumerable<string> ListSaves();
	}
}