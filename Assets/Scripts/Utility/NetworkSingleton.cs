using Unity.Netcode;
using UnityEngine;

namespace Utility {
    public class NetworkSingleton<T> : NetworkBehaviour where T : NetworkBehaviour {
        private static T _instance;

        public static T Singleton {
            get {
                if (_instance != null) return _instance;
                
                _instance = FindObjectOfType<T>();

                if (_instance != null) return _instance;
                
                var obj = new GameObject {
                    name = typeof(T).Name
                };
                _instance = obj.AddComponent<T>();

                return _instance;
            }
        }

        private void Awake() {
            if (_instance == null) {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else Destroy(gameObject);
        }
    }
}