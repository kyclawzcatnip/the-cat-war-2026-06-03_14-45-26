using UnityEngine;

namespace CatWar
{
    public class ResourceManager : MonoBehaviour
    {
        public static ResourceManager Instance { get; private set; }

        [Header("Starting Stats")]
        [SerializeField] private float startingGold = 100f;
        [SerializeField] private float baseMaxGold = 500f;
        [SerializeField] private float baseGoldGenRate = 10f; // Gold per second

        [Header("State")]
        [SerializeField] private float currentGold;
        [SerializeField] private float maxGold;
        [SerializeField] private float goldGenRate;
        [SerializeField] private int minerLevel = 1;

        [Header("Upgrade System")]
        [SerializeField] private int maxMinerLevel = 8;
        [SerializeField] private float upgradeCostBase = 90f;
        [SerializeField] private float upgradeCostMultiplier = 1.5f;

        public float CurrentGold => currentGold;
        public float MaxGold => maxGold;
        public float GoldGenRate => goldGenRate;
        public int MinerLevel => minerLevel;
        public int MaxMinerLevel => maxMinerLevel;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            currentGold = startingGold;
            maxGold = baseMaxGold;
            goldGenRate = baseGoldGenRate;
        }

        private void Update()
        {
            if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameManager.GameState.Playing)
                return;

            // Generate gold over time
            currentGold += goldGenRate * Time.deltaTime;
            currentGold = Mathf.Clamp(currentGold, 0f, maxGold);
        }

        public bool CanAfford(float cost)
        {
            return currentGold >= cost;
        }

        public bool SpendGold(float amount)
        {
            if (CanAfford(amount))
            {
                currentGold -= amount;
                return true;
            }
            return false;
        }

        public void AddGold(float amount)
        {
            currentGold = Mathf.Clamp(currentGold + amount, 0f, maxGold);
        }

        public float GetUpgradeCost()
        {
            if (minerLevel >= maxMinerLevel) return -1f; // Already maxed out
            return Mathf.Round(upgradeCostBase * Mathf.Pow(upgradeCostMultiplier, minerLevel - 1));
        }

        public bool UpgradeMiner()
        {
            if (minerLevel >= maxMinerLevel) return false;

            float cost = GetUpgradeCost();
            if (SpendGold(cost))
            {
                minerLevel++;
                // Scale caps and rates
                maxGold = baseMaxGold * (1f + (minerLevel - 1) * 0.5f);
                goldGenRate = baseGoldGenRate * (1f + (minerLevel - 1) * 0.4f);

                // Play upgrade sound
                if (AudioManager.Instance != null)
                {
                    // AudioManager.Instance.PlayUpgradeSound();
                }
                return true;
            }
            return false;
        }
    }
}
