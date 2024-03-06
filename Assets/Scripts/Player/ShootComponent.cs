using Network;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.GameObject;

namespace Player {
	public class ShootComponent : NetworkBehaviour, IComponent { // we need some network stuffy here, isOnwer is needed

		[SerializeField] private GameObject _serverPrefab;
		[SerializeField] private GameObject _clientPrefab;
		private Transform _bodyTf;
		private Transform _muzzleTf;
		public MonoBehaviour Source { get; set; }
		public bool IsServerOwner { get; set; }

		public void Initialize<T>(T source, bool isOwner) where T : MonoBehaviour {
			Source = source;
			IsServerOwner = isOwner;
			if (Source is not PlayerController controller) return;
			_muzzleTf = controller.GetChild("Muzzle");
			_bodyTf = controller.GetChild("Body");
			
		}

		public ShootComponent(GameObject serverPrefab, GameObject clientPrefab) {
			_serverPrefab = serverPrefab;
			_clientPrefab = clientPrefab;
		}

		public void Execute() => LocalShoot(); // entrance

		// three messages here seems messy, what goes where?
		[ServerRpc] 
		private void ShootServerRpc() {
			if (!IsServer) return;
			
			GameObject projectile = Instantiate(_serverPrefab, _muzzleTf.position, _bodyTf.rotation);
			projectile.GetComponent<Projectile>()?.Initialize(Source.gameObject); 
			ShootClientRpc();
		}

		[ClientRpc] 
		private void ShootClientRpc() {
			if (IsServerOwner) return; // IsOwner subject to possible race condition, see playerController -> Start
			
			Debug.Log("do we get in here?");
			GameObject projectile = Instantiate(_clientPrefab, _muzzleTf.position, _bodyTf.rotation);
			projectile.GetComponent<Projectile>()?.Initialize(Source.gameObject);
		}

		private void LocalShoot() { 
			GameObject projectile = Instantiate(_clientPrefab, _muzzleTf.position, _bodyTf.rotation);
			projectile.GetComponent<Projectile>()?.Initialize(Source.gameObject);
			ShootServerRpc();
		}


	}
}