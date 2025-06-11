using UnityEngine;

namespace YooX.SpawnSystem {
    public interface ISpawnPointStrategy {
        Transform NextSpawnPoint();
    }

}
