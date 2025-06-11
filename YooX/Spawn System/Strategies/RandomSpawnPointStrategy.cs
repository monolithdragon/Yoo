using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace YooX.SpawnSystem {
    public class RandomSpawnPointStrategy : ISpawnPointStrategy {
        private List<Transform> _unusedSpawnPoints;
        private readonly List<Transform> _spawnPoints;

        public RandomSpawnPointStrategy(List<Transform> spawnPoints) {
            _spawnPoints = spawnPoints;
            _unusedSpawnPoints = new List<Transform>(spawnPoints);
        }

        public Transform NextSpawnPoint() {
            if (!_unusedSpawnPoints.Any()) {
                _unusedSpawnPoints = new List<Transform>(_spawnPoints);
            }

            var randomIndex = Random.Range(0, _unusedSpawnPoints.Count);
            var result = _unusedSpawnPoints[randomIndex];
            _unusedSpawnPoints.RemoveAt(randomIndex);
            return result;
        }
    }

}
