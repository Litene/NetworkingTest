using Network;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.GameObject;

namespace Player {
	public class ShootComponent : IComponent {

		private readonly GameObject _serverPrefab;
		private readonly GameObject _clientPrefab;
		private Transform _bodyTf;
		private Transform _muzzleTf;
		public MonoBehaviour Source { get; set; }

		public void Initialize<T>(T source) where T : MonoBehaviour {
			Source = source;
			if (Source is not PlayerController controller) return;
			_muzzleTf = controller.GetChild("Muzzle");
			_bodyTf = controller.GetChild("Body");
		}

		public ShootComponent(GameObject serverPrefab, GameObject clientPrefab) {
			_serverPrefab = serverPrefab;
			_clientPrefab = clientPrefab;
		}

		public void Execute() {
			if (Source is not NetworkBehaviour networkBehaviour) return;
			if (networkBehaviour.IsHost) ShootServerRpc();
			else ShootClientRpc();
		}

		[ServerRpc] private void ShootServerRpc() {
			Debug.Log("we are shooting on server");
			GameObject projectile = Object.Instantiate(_serverPrefab, _muzzleTf.position, _bodyTf.rotation);
			projectile.GetComponent<Projectile>()?.Initialize(Source.gameObject);
			ShootClientRpc();
		}

		[ClientRpc] private void ShootClientRpc() {
			Debug.Log("we are shooting on client");
			GameObject projectile = Object.Instantiate(_clientPrefab, _muzzleTf.position, _bodyTf.rotation);
			projectile.GetComponent<Projectile>()?.Initialize(Source.gameObject);

		}


	}
}