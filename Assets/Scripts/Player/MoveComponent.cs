using Unity.Netcode;
using UnityEngine;

namespace Player {
	public class MoveComponent : IComponent {

		public NetworkBehaviour Source { get; set; }

		private const float RotationSpeed = 5f;
		private const float MovementSpeed = 10f;

		private Transform _bodyTf;
		private Transform _tf;
		private Camera _camera;
		public void Initialize<T>(T source) where T : NetworkBehaviour {
			this.Source = source;
			if ((Source is PlayerController controller)) {
				_bodyTf = controller.GetChild("Body");
			}

			_tf = Source.transform;
			_camera = Camera.main;
		}

		public void Tick(float deltaTime) {
			Vector3 mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
			Rotate(mousePosition);
			Move();
		}
		private void Move() => _tf.Translate(_bodyTf.up * (MovementSpeed * Time.deltaTime));

		private void Rotate(Vector3 mousePosition) {
			Vector3 direction = mousePosition - _bodyTf.position;

			float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;

			Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

			_bodyTf.rotation = Quaternion.Slerp(_bodyTf.rotation, rotation, RotationSpeed * Time.deltaTime);
		}
	}
}