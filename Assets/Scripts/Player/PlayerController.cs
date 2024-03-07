using System.Collections.Generic;
using System.Linq;
using UI;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;
using Network;
using static InputScheme;
using Random = UnityEngine.Random;
using SF = UnityEngine.SerializeField;

namespace Player {
    public class PlayerController : NetworkBehaviour, IPlayerActions, IMonoExtension.ITransformLookup {
        [SF] private GameObject _serverProjectilePrefab;
        [SF] private GameObject _clientProjectilePrefab;

        private List<IComponent> _components;
        public Dictionary<string, Transform> TransformsLookup { get; set; } = new();

        public T GetControllerComponent<T>() where T : IComponent =>
            _components.OfType<T>().FirstOrDefault(); // this should be a dictionary to reduce lookup time. 

        private InputScheme _input;

        private void Awake() {
            if (_input is null) {
                _input = new();
                _input.Player.SetCallbacks(this);
            }

            _input.Player.Enable();

            _components = new List<IComponent> {
                new HealthComponent(10),
                new MoveComponent(),
                new HealthUIComponent(),
                new ShootComponent(_serverProjectilePrefab.gameObject, _clientProjectilePrefab.gameObject)
            };

            InitializeLookup(transform);

            _components.ForEach(component => component.Initialize(this));
        }

        private void Start() => transform.position = new Vector3(Random.Range(-10, 10), Random.Range(-4, 4));
        private void Update() => _components.ForEach(component => component.Tick(Time.deltaTime));

        public void OnShoot(InputAction.CallbackContext context) {
            if (!context.performed || !IsOwner) return;

            NetworkController.Singleton.ClientSpawn(_clientProjectilePrefab, GetChild("Muzzle").position,
                GetChild("Body").rotation);
            NetworkController.Singleton.ServerSpawn(_serverProjectilePrefab, GetChild("Muzzle").position,
                GetChild("Body").rotation);
        }

        public override void OnNetworkSpawn() {
            if (IsOwner) GameObject.Find("NetworkController").GetComponent<NetworkObject>().ChangeOwnership(GetComponent<NetworkObject>().OwnerClientId);
            
        }

        public void InitializeLookup(Transform source) {
            foreach (Transform child in source) {
                TransformsLookup[child.name] = child;
                if (child.childCount > 0) InitializeLookup(child);
            }
        }

        public Transform GetChild(string childName) =>
            TransformsLookup.TryGetValue(childName, out Transform child) ? child : null;

        // [ServerRpc] private void ShootServerRpc() {
        // 	GameObject projectile = Instantiate(_serverProjectilePrefab, GetChild("Muzzle").position, GetChild("Body").rotation);
        // 	projectile.GetComponent<Projectile>()?.Initialize(this.gameObject);
        // 	ShootClientRpc();
        // }
        //
        // [ClientRpc] private void ShootClientRpc() {
        // 	if (IsOwner) return;
        // 	GameObject projectile = Instantiate(_clientProjectilePrefab, GetChild("Muzzle").position, GetChild("Body").rotation);
        // 	projectile.GetComponent<Projectile>()?.Initialize(this.gameObject);
        // }
        //
        // private void ShootLocal() {
        // 	GameObject projectile = Instantiate(_clientProjectilePrefab, GetChild("Muzzle").position, GetChild("Body").rotation);
        // 	projectile.GetComponent<Projectile>()?.Initialize(this.gameObject);
        // 	
        // 	ShootServerRpc();
        // }
    }
}