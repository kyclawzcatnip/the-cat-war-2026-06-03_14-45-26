using UnityEngine;
using UnityEngine.SceneManagement;

namespace CatWar
{
    [System.Serializable]
    public class LevelConfig
    {
        public string levelName;
        public float baseHealthPlayer = 1000f;
        public float baseHealthEnemy = 1000f;
        public float enemySpawnIntervalMin = 8f;
        public float enemySpawnIntervalMax = 15f;
        public UnitData[] enemyPool; // Available enemies to spawn in this level
        public float difficultyMultiplier = 1.0f; // Scale enemy stats (health, damage)
    }

    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }

        [Header("Levels Configuration")]
        [SerializeField] private LevelConfig[] levels;
        [SerializeField] private int currentLevelIndex = 0;

        public int CurrentLevelIndex => currentLevelIndex;
        public int TotalLevels => levels.Length;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            InitializeLevel();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            InitializeLevel();
        }

        public LevelConfig GetCurrentLevel()
        {
            if (levels == null || levels.Length == 0)
            {
                Debug.LogWarning("[LevelManager] No levels configured! Creating a default fallback level.");
                return new LevelConfig
                {
                    levelName = "Default Battlements",
                    baseHealthPlayer = 1000f,
                    baseHealthEnemy = 1000f,
                    enemySpawnIntervalMin = 10f,
                    enemySpawnIntervalMax = 18f,
                    enemyPool = new UnitData[0],
                    difficultyMultiplier = 1.0f
                };
            }

            int index = Mathf.Clamp(currentLevelIndex, 0, levels.Length - 1);
            return levels[index];
        }

        public void InitializeLevel()
        {
            LevelConfig config = GetCurrentLevel();
            Debug.Log($"[LevelManager] Initializing Level: {config.levelName}");

            // Find Bases in the scene and configure their health based on the level configuration
            Base[] bases = FindObjectsOfType<Base>();
            foreach (var b in bases)
            {
                if (b.isPlayerBase)
                {
                    b.maxHealth = config.baseHealthPlayer;
                }
                else
                {
                    b.maxHealth = config.baseHealthEnemy;
                }
                b.currentHealth = b.maxHealth;
            }

            // Find Spawner and set its parameters
            EnemySpawner spawner = FindObjectOfType<EnemySpawner>();
            if (spawner != null)
            {
                spawner.ConfigureSpawner(config);
            }
        }

        public void GoToNextLevel()
        {
            currentLevelIndex++;
            if (currentLevelIndex >= levels.Length)
            {
                Debug.Log("[LevelManager] All levels cleared! Restarting from Level 1.");
                currentLevelIndex = 0;
            }

            // In a simple setup we reload the main battle scene, 
            // and the levels config will configure the bases/spawner dynamically.
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
