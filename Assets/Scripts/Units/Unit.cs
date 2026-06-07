using UnityEngine;

namespace CatWar
{
    public class Unit : MonoBehaviour
    {
        public enum UnitState { Moving, Attacking, Healing, Dying }

        [Header("Unit Profile & Config")]
        public UnitData data;
        public bool isEnemy;
        
        [Header("Targeting Layers")]
        [SerializeField] private LayerMask enemyMask;
        [SerializeField] private LayerMask friendlyMask;

        [Header("State")]
        [SerializeField] private UnitState currentState = UnitState.Moving;
        private float currentHealth;
        private float attackTimer;
        private Collider2D currentTarget;

        [Header("Visuals & UI")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Animator animator;
        [SerializeField] private GameObject healthBarPrefab; // Instantiated dynamically above units
        private HealthBar healthBarInstance;

        [Header("Positions")]
        [SerializeField] private Transform projectileFirePoint; // Point where arrows/bolts spawn

        private void Start()
        {
            if (data == null)
            {
                Debug.LogError($"[Unit] UnitData is missing on {gameObject.name}");
                return;
            }

            currentHealth = data.maxHealth;
            attackTimer = data.attackCooldown; // Ready to attack immediately

            // Setup components
            if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (animator == null) animator = GetComponentInChildren<Animator>();

            // Setup sprite flip (Player walks right, Enemy walks left)
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = data.unitSprite;
                // Assume sprite naturally faces right. Flip if enemy.
                spriteRenderer.flipX = isEnemy;
            }

            // Dynamically instantiate health bar in world space
            if (healthBarPrefab != null)
            {
                GameObject hpObj = Instantiate(healthBarPrefab, transform.position, Quaternion.identity);
                healthBarInstance = hpObj.GetComponent<HealthBar>();
                if (healthBarInstance != null)
                {
                    healthBarInstance.SetTarget(transform, new Vector3(0, 1.2f, 0));
                    healthBarInstance.SetMaxHealth(data.maxHealth);
                    healthBarInstance.SetHealth(currentHealth);
                }
            }

            // Play Spawn Sound
            if (AudioManager.Instance != null && data.spawnSound != null)
            {
                AudioManager.Instance.PlaySFX(data.spawnSound);
            }
        }

        private void Update()
        {
            if (currentState == UnitState.Dying) return;

            // Increment attack timer
            attackTimer += Time.deltaTime;

            // Target search & combat loop
            if (data.isHealer)
            {
                HandleHealerBehavior();
            }
            else
            {
                HandleCombatBehavior();
            }
        }

        #region Healer Logic
        private void HandleHealerBehavior()
        {
            Collider2D targetAlly = FindDamagedAlly();

            if (targetAlly != null)
            {
                currentState = UnitState.Healing;
                if (animator != null) animator.SetBool("isMoving", false);

                if (attackTimer >= data.attackCooldown)
                {
                    PerformHeal(targetAlly);
                    attackTimer = 0f;
                }
            }
            else
            {
                // No damaged ally in range, keep moving forward
                currentState = UnitState.Moving;
                if (animator != null) animator.SetBool("isMoving", true);
                MoveForward();
            }
        }

        private Collider2D FindDamagedAlly()
        {
            // Clerics only search in front of themselves
            Vector2 checkPos = (Vector2)transform.position + (isEnemy ? Vector2.left : Vector2.right) * (data.attackRange * 0.5f);
            Collider2D[] colliders = Physics2D.OverlapBoxAll(checkPos, new Vector2(data.attackRange, 1.2f), 0, friendlyMask);

            foreach (var col in colliders)
            {
                Unit allyUnit = col.GetComponent<Unit>();
                if (allyUnit != null && allyUnit != this && allyUnit.currentHealth < allyUnit.data.maxHealth)
                {
                    return col;
                }
            }
            return null;
        }

        private void PerformHeal(Collider2D ally)
        {
            if (animator != null) animator.SetTrigger("Heal");

            Unit allyUnit = ally.GetComponent<Unit>();
            if (allyUnit != null)
            {
                allyUnit.RestoreHealth(data.healAmount);

                // Spawn spell effect at ally location
                if (data.hitEffectPrefab != null)
                {
                    Instantiate(data.hitEffectPrefab, ally.transform.position, Quaternion.identity);
                }

                // Play heal audio
                if (AudioManager.Instance != null && data.attackSound != null)
                {
                    AudioManager.Instance.PlaySFX(data.attackSound);
                }
            }
        }
        #endregion

        #region Combat Logic
        private void HandleCombatBehavior()
        {
            // Find target in front of the unit
            currentTarget = FindEnemyTarget();

            if (currentTarget != null)
            {
                currentState = UnitState.Attacking;
                if (animator != null) animator.SetBool("isMoving", false);

                if (attackTimer >= data.attackCooldown)
                {
                    PerformAttack();
                    attackTimer = 0f;
                }
            }
            else
            {
                // No enemy in range, keep marching towards the base
                currentState = UnitState.Moving;
                if (animator != null) animator.SetBool("isMoving", true);
                MoveForward();
            }
        }

        private Collider2D FindEnemyTarget()
        {
            Vector2 checkPos = (Vector2)transform.position + (isEnemy ? Vector2.left : Vector2.right) * (data.attackRange * 0.5f);
            Collider2D[] colliders = Physics2D.OverlapBoxAll(checkPos, new Vector2(data.attackRange, 1.2f), 0, enemyMask);

            // Return the first valid unit or base in list (typically closest)
            if (colliders.Length > 0)
            {
                return colliders[0];
            }
            return null;
        }

        private void PerformAttack()
        {
            if (animator != null) animator.SetTrigger("Attack");

            // Play Attack Sound
            if (AudioManager.Instance != null && data.attackSound != null)
            {
                AudioManager.Instance.PlaySFX(data.attackSound);
            }

            if (data.isRanged)
            {
                // Ranged attack spawns a projectile
                if (data.projectilePrefab != null)
                {
                    Vector3 firePoint = projectileFirePoint != null ? projectileFirePoint.position : transform.position;
                    Vector3 direction = isEnemy ? Vector3.left : Vector3.right;
                    
                    GameObject projObj = Instantiate(data.projectilePrefab, firePoint, Quaternion.identity);
                    Projectile proj = projObj.GetComponent<Projectile>();
                    if (proj != null)
                    {
                        proj.Setup(firePoint, direction, 12f, data.attackDamage, enemyMask);
                    }
                }
            }
            else
            {
                // Melee attack directly applies damage
                if (currentTarget != null)
                {
                    ApplyDamageToTarget(currentTarget, data.attackDamage);

                    // Spawn impact/slash effect
                    if (data.hitEffectPrefab != null)
                    {
                        Instantiate(data.hitEffectPrefab, currentTarget.transform.position, Quaternion.identity);
                    }
                }
            }
        }

        private void ApplyDamageToTarget(Collider2D target, float dmg)
        {
            Unit enemyUnit = target.GetComponent<Unit>();
            if (enemyUnit != null)
            {
                enemyUnit.TakeDamage(dmg);
                return;
            }

            Base enemyBase = target.GetComponent<Base>();
            if (enemyBase != null)
            {
                enemyBase.TakeDamage(dmg);
            }
        }
        #endregion

        #region Movement & Health Actions
        private void MoveForward()
        {
            float dir = isEnemy ? -1f : 1f;
            transform.Translate(Vector3.right * dir * data.movementSpeed * Time.deltaTime);
        }

        public void TakeDamage(float damage)
        {
            if (currentState == UnitState.Dying) return;

            currentHealth -= damage;
            if (healthBarInstance != null)
            {
                healthBarInstance.SetHealth(currentHealth);
            }

            // Play damage sound
            if (AudioManager.Instance != null && data.damageSound != null)
            {
                AudioManager.Instance.PlaySFX(data.damageSound);
            }

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                Die();
            }
        }

        public void RestoreHealth(float amount)
        {
            if (currentState == UnitState.Dying) return;

            currentHealth = Mathf.Min(currentHealth + amount, data.maxHealth);
            if (healthBarInstance != null)
            {
                healthBarInstance.SetHealth(currentHealth);
            }
        }

        private void Die()
        {
            currentState = UnitState.Dying;
            if (animator != null) animator.SetTrigger("Die");

            // Disable collisions and physics to avoid blockage after death
            Collider2D col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

            // Remove HP bar
            if (healthBarInstance != null)
            {
                Destroy(healthBarInstance.gameObject);
            }

            // Play death sound
            if (AudioManager.Instance != null && data.deathSound != null)
            {
                AudioManager.Instance.PlaySFX(data.deathSound);
            }

            // Notify level spawner or managers if necessary
            // Destroy after delay to let death animation finish
            Destroy(gameObject, 1.5f);
        }
        #endregion

        // Visual debug bounds for developer in Unity editor
        private void OnDrawGizmosSelected()
        {
            if (data == null) return;
            Gizmos.color = isEnemy ? Color.red : Color.green;
            Vector2 checkPos = (Vector2)transform.position + (isEnemy ? Vector2.left : Vector2.right) * (data.attackRange * 0.5f);
            Gizmos.DrawWireCube(checkPos, new Vector2(data.attackRange, 1.2f));
        }
    }
}
