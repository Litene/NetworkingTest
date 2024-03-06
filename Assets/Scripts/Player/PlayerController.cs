using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;
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
                //new ShootComponent(_serverProjectilePrefab, _clientProjectilePrefab)
            };
            
            _components.Add(GetComponent<ShootComponent>());

            InitializeLookup(transform);
        }

        private void Start() {
          // throw new NotImplementedException();
        }

        public override void OnNetworkSpawn() {
          Debug.Log("called");
             _components.ForEach(component =>
                component.Initialize(this, IsOwner)); // possible race condition, IsOwner is not set in awake. 
            transform.position = new Vector3(Random.Range(-10, 10), Random.Range(-4, 4));
        }

        // private void Start() {
        //     _components.ForEach(component =>
        //         component.Initialize(this, IsOwner)); // possible race condition, IsOwner is not set in awake. 
        //     transform.position = new Vector3(Random.Range(-10, 10), Random.Range(-4, 4));
        // }

        private void Update() => _components.ForEach(component => component.Tick(Time.deltaTime));

        public void OnShoot(InputAction.CallbackContext context) {
            if (!IsOwner) return;
            
            if (context.performed) GetControllerComponent<ShootComponent>().Execute();
            

            //if (context.performed) GetControllerComponent<HealthComponent>().TakeDamage(1); // this is damage test
        }

        public void InitializeLookup(Transform source) {
            foreach (Transform child in source) {
                TransformsLookup[child.name] = child;
                if (child.childCount > 0) InitializeLookup(child);
            }
        }

        public Transform GetChild(string childName) =>
            TransformsLookup.TryGetValue(childName, out Transform child) ? child : null;
    }
}