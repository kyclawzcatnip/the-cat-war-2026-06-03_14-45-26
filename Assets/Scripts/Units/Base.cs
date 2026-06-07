using UnityEngine;

namespace CatWar
{
    public class Base : MonoBehaviour
    {
        [Header("Config")]
        public bool isPlayerBase;
        public float maxHealth = 1000f;
        
        [Header("State")]
        public float currentHealth;
        
        [Header("UI Reference")]
        [SerializeField] private HealthBar healthBar;

        private void Start()
        {
            currentHealth = maxHealth;
            if (healthBar != null)
            {
                healthBar.SetMaxHealth(maxHealth);
                healthBar.SetHealth(currentHealth);
            }
        }

        public void TakeDamage(float damage)
        {
            if (currentHealth <= 0) return;

            currentHealth -= damage;
            if (healthBar != null)
            {
                healthBar.SetHealth(currentHealth);
            }

            if (AudioManager.Instance != null && isPlayerBase)
            {
                // Play warning sound or hit sound
            }

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                Die();
            }
        }

        private void Die()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnBaseDestroyed(isPlayerBase);
            }

            // Play destruction sound/effects
            if (AudioManager.Instance != null)
            {
                // AudioManager.Instance.PlayBaseExplosion();
            }

            gameObject.SetActive(false);
        }
    }
}
