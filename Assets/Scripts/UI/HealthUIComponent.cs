using Player;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
	public class HealthUIComponent : IComponent {
		public MonoBehaviour Source { get; set; }
		public bool IsServerOwner { get; set; }
		private Slider _slider;
		private HealthComponent _healthComponent;
		private Image _image;

		public void Initialize<T>(T source, bool isOwner) where T : MonoBehaviour {
			Source = source;
			IsServerOwner = isOwner;

			if (Source is PlayerController controller) {
				_healthComponent = controller.GetControllerComponent<HealthComponent>();
				_image = controller.GetChild("Fill").GetComponent<Image>();
				_image.color = Color.green;
			}

			_healthComponent.OnHealthChanged += UpdateUI;

			_slider = source.GetComponentInChildren<Slider>();
			_slider.maxValue = _healthComponent.GetHealth();
			_slider.value = _slider.maxValue;
		}

		private void UpdateUI(int currentHealth) {
			_slider.value = currentHealth;
			_image.color = Color.Lerp(Color.red, Color.green, Mathf.InverseLerp(0, _slider.maxValue, currentHealth));
		}

	}
}