using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using Utility;

public enum NetworkType {
    Local,
    Server
}

namespace Network {
    public class NetworkController : NetworkSingleton<NetworkController> {
        private readonly Dictionary<string, GameObject>
            _spawnedObjects = new(); // should be filled with everything spawned?
        
        /// <summary>
        /// Clears method
        /// </summary>
        public List<GameObject> Get {
            get {
                var tempList = _spawnedObjects.Values.ToList();
                _spawnedObjects.Clear();
                return tempList;
            }
        }

        public void Clear() => _spawnedObjects.Clear();

        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objPrefab"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public GameObject ClientSpawn(GameObject objPrefab, Vector3 position, Quaternion rotation) {
            // this is either genius or a incredibly stupid solution... since I can't pass variables
            var guid = Guid.NewGuid().ToString();

            _spawnedObjects.Add(guid, objPrefab);

            ClientSpawnClientRpc(position, rotation, guid);

            return _spawnedObjects.TryGetValue(guid, out var netObj) ? netObj : null;
        }

        [ClientRpc] private void ClientSpawnClientRpc(Vector3 position, Quaternion rotation, string guid) {
            if (_spawnedObjects.ContainsKey(guid)) {
                _spawnedObjects[guid] = Instantiate(_spawnedObjects[guid], position, rotation);
            }
            else {
                Debug.Log("We do not have a key");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="objPrefab"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public GameObject ServerSpawn(GameObject objPrefab, Vector3 position, Quaternion rotation) {
            var guid = Guid.NewGuid().ToString();

            _spawnedObjects.Add(guid, objPrefab);

            ServerSpawnServerRpc(position, rotation, guid);

            return _spawnedObjects.TryGetValue(guid, out var netObj) ? netObj : null;
        }

        [ServerRpc/*(RequireOwnership = false)*/] private void ServerSpawnServerRpc(Vector3 position, Quaternion rotation, string guid) {
            if (_spawnedObjects.ContainsKey(guid)) {
                _spawnedObjects[guid] = Instantiate(_spawnedObjects[guid], position, rotation);
            }
        }
    }
}