using UnityEngine;

namespace CatWar
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField] private Transform spawnPoint;

        private LevelConfig currentLevelConfig;
        private float spawnTimer;
        private float currentInterval;
        private bool isActive;

        public void ConfigureSpawner(LevelConfig config)
        {
            currentLevelConfig = config;
            currentInterval = Random.Range(config.enemySpawnIntervalMin, config.enemySpawnIntervalMax);
            spawnTimer = 0f;
            isActive = true;
            Debug.Log($"[EnemySpawner] Configured with level: {config.levelName}");
        }

        private void Update()
        {
            if (!isActive || currentLevelConfig == null) return;
            if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameManager.GameState.Playing) return;

            spawnTimer += Time.deltaTime;

            // Escalation mechanic: enemies spawn slightly faster the longer the battle lasts.
            // Spawns speed up by up to 40% after 200 seconds of combat.
            float timeScaling = Mathf.Max(0.6f, 1f - (Time.timeSinceLevelLoad * 0.002f));
            float adjustedInterval = currentInterval * timeScaling;

            if (spawnTimer >= adjustedInterval)
            {
                SpawnEnemy();
                spawnTimer = 0f;
                currentInterval = Random.Range(currentLevelConfig.enemySpawnIntervalMin, currentLevelConfig.enemySpawnIntervalMax);
            }
        }

        private void SpawnEnemy()
        {
            if (currentLevelConfig.enemyPool == null || currentLevelConfig.enemyPool.Length == 0)
            {
                Debug.LogWarning("[EnemySpawner] Enemy pool is empty for this level!");
                return;
            }

            // Select random enemy from the pool
            int randomIndex = Random.Range(0, currentLevelConfig.enemyPool.Length);
            UnitData enemyData = currentLevelConfig.enemyPool[randomIndex];

            if (enemyData != null && enemyData.prefab != null)
            {
                Vector3 spawnPos = transform.position;
                if (spawnPoint != null)
                {
                    spawnPos = spawnPoint.position;
                }

                GameObject enemyObj = Instantiate(enemyData.prefab, spawnPos, Quaternion.identity);
                Unit unit = enemyObj.GetComponent<Unit>();
                if (unit != null)
                {
                    unit.data = enemyData;
                    unit.isEnemy = true;
                    // Multiply stats by difficulty multiplier if desired
                    unit.TakeDamage(-(enemyData.maxHealth * (currentLevelConfig.difficultyMultiplier - 1.0f))); // Simple way to adjust health
                }
            }
            else
            {
                Debug.LogWarning("[EnemySpawner] Selected enemy unit data or prefab is null!");
            }
        }
    }
}
