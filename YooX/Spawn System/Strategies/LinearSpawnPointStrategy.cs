using System.Collections.Generic;
using UnityEngine;

namespace YooX.SpawnSystem {
    public class LinearSpawnPointStrategy : ISpawnPointStrategy {
        private int _index = 0;
        private readonly List<Transform> _spawnPoints;

        public LinearSpawnPointStrategy(List<Transform> spawnPoints) => _spawnPoints = spawnPoints;

        public Transform NextSpawnPoint() {
            var result = _spawnPoints[_index];
            _index = (_index + 1) % _spawnPoints.Count;
            return result;
        }
    }

}
