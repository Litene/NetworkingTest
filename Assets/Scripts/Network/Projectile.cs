using System;
using Player;
using Unity.Netcode;
using UnityEngine;

namespace Network {
	public class Projectile : NetworkBehaviour {

		private GameObject _owner;
		private float MovementSpeed { get; set; } = 20;

		public void Initialize(GameObject owner) => _owner = owner;
		private void OnTriggerEnter(Collider other) {
			if (other.CompareTag("Player") && other.gameObject != _owner) {
				Debug.Log("We do be hit");
			}
		}

		private void Update() => transform.Translate(Vector2.up * (Time.deltaTime * MovementSpeed));
	}
}