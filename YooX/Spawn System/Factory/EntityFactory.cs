using UnityEngine;

namespace YooX.SpawnSystem {
    public class EntityFactory<T> : IEntityFactory<T> where T : IEntity {
        private readonly EntityData[] _data;

        public EntityFactory(EntityData[] data) => _data = data;

        public T Create(Transform spawnPoint) {
            var entityData = _data[Random.Range(0, _data.Length)];
            var entity = GameObject.Instantiate(entityData.prefab, spawnPoint.position, Quaternion.identity);
            return entity!.GetComponent<T>();
        }
    }

}
