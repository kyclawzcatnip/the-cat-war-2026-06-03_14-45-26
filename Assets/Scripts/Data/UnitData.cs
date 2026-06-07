using UnityEngine;

namespace CatWar
{
    [CreateAssetMenu(fileName = "NewUnitData", menuName = "Cat War/Unit Data")]
    public class UnitData : ScriptableObject
    {
        [Header("Display Info")]
        public string unitName;
        public Sprite icon;
        public Sprite unitSprite;

        [Header("Economy")]
        public int cost;
        public float spawnCooldown;

        [Header("Stats")]
        public float maxHealth;
        public float attackDamage;
        public float attackRange;
        public float attackCooldown;
        public float movementSpeed;

        [Header("Special Abilities")]
        public bool isRanged;
        public bool isHealer;
        public float healAmount;

        [Header("Visual Effects & Prefabs")]
        public GameObject prefab;
        public GameObject hitEffectPrefab;
        public GameObject projectilePrefab; // Spawns for ranged projectiles like arrows/bolts

        [Header("Audio")]
        public AudioClip spawnSound;
        public AudioClip attackSound;
        public AudioClip damageSound;
        public AudioClip deathSound;
    }
}
