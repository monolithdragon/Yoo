namespace YooX.SpawnSystem {
    public class EntitySpawner<T> where T : IEntity {
        private readonly IEntityFactory<T> _entityFactory;
        private readonly ISpawnPointStrategy _spawnPointStrategy;

        public EntitySpawner(IEntityFactory<T> entityFactory, ISpawnPointStrategy spawnPointStrategy) {
            _entityFactory = entityFactory;
            _spawnPointStrategy = spawnPointStrategy;
        }

        public T Spawn() {
            return _entityFactory.Create(_spawnPointStrategy.NextSpawnPoint());
        }
    }

}
