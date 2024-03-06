using Unity.Netcode;
using UnityEngine;

using SF = UnityEngine.SerializeField;
class NetworkButton : MonoBehaviour {
	[SF] private bool _isHostButton; // defuq did I do here
	public void ConnectToNetWork() {
		if (_isHostButton) NetworkManager.Singleton.StartHost();
		else NetworkManager.Singleton.StartClient();
	}
}