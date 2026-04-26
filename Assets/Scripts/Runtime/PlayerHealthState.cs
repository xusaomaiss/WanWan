using UnityEngine;

namespace Wanwan.Runtime
{
    public class PlayerHealthState
    {
        public const int DefaultMaxHealth = 10;

        public PlayerHealthState(int maxHealth = DefaultMaxHealth)
        {
            MaxHealth = Mathf.Max(1, maxHealth);
            CurrentHealth = MaxHealth;
        }

        public int MaxHealth { get; }
        public int CurrentHealth { get; private set; }
        public bool IsDepleted => CurrentHealth <= 0;

        public int ApplyDamage(int amount)
        {
            int safeAmount = Mathf.Max(0, amount);
            int damageApplied = Mathf.Min(CurrentHealth, safeAmount);
            CurrentHealth -= damageApplied;
            return damageApplied;
        }
    }
}
