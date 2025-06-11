using UnityEngine;

namespace YooX.SpawnSystem {
    public interface IEntityFactory<T> where T : IEntity {
        T Create(Transform spawnPoint);
    }

}
