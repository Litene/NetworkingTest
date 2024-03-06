using System;
using Unity.Netcode;
using UnityEngine;

namespace Player {
	public class HealthComponent : IComponent {

		private int _health;

		public Action<int> OnHealthChanged;
		public Action OnDied;
		private NetworkVariable<int> CurrentHealth { get; set; } = new();
		
		/// <summary>
		/// Pass how much damage the target should loose.
		/// </summary>
		/// <param name="amount"></param>
		/// <returns>Returns current health</returns>
		public int TakeDamage(int amount) {
			int amountToRemove = amount > 0 ? amount : -amount;
			CurrentHealth.Value = GetHealth() - amountToRemove <= 0 ? 0 : GetHealth() - amountToRemove;
			
			OnHealthChanged?.Invoke(GetHealth());
			if (GetHealth() <= 0) OnDied?.Invoke();
			
			return GetHealth();
		}
		
		public NetworkBehaviour Source { get; set; }
		public void Initialize<T>(T source) where T : NetworkBehaviour => this.Source = source;
		public int GetHealth() => CurrentHealth.Value;
		public HealthComponent(int health) {
			_health = health;
			CurrentHealth!.Value = _health;
		}
	}
	
	public interface IDamageable {
		public HealthComponent HealthComponent { get; set; }
		public int TakeDamage(int amount);
	}

	public interface IDamager {
		public void DealDamage(IDamageable target, int amount);
	}

	public interface IComponent {
		NetworkBehaviour Source { get; set; }
		public void Initialize<T>(T source) where T : NetworkBehaviour {}
		public void Tick(float deltaTime){}

		public void Execute(){}

	}
}