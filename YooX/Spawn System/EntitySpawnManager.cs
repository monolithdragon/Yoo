using System.Collections.Generic;
using UnityEngine;

namespace YooX.SpawnSystem {
    public abstract class EntitySpawnManager : MonoBehaviour {
        [SerializeField] protected SpawnPointStrategyType spawnPointStrategyType = SpawnPointStrategyType.Linear;
        [SerializeField] protected List<Transform>? spawnPoints;

        protected enum SpawnPointStrategyType {
            Linear,
            Random
        }

        protected ISpawnPointStrategy? _spawnPointStrategy;

        protected virtual void Awake() {
            _spawnPointStrategy = spawnPointStrategyType switch {
                SpawnPointStrategyType.Linear => new LinearSpawnPointStrategy(spawnPoints!),
                SpawnPointStrategyType.Random => new RandomSpawnPointStrategy(spawnPoints!),
                _ => _spawnPointStrategy
            };

            foreach (Transform spawnPoint in GetComponentInChildren<Transform>()) {
                spawnPoints?.Add(spawnPoint);
            }
        }

        public abstract void Spawn();
    }

}
