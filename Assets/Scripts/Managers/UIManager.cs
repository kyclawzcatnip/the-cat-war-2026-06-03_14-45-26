using UnityEngine;
using UnityEngine.UI;

namespace CatWar
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("HUD Panels")]
        [SerializeField] private GameObject hudPanel;
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject victoryPanel;

        [Header("HUD Text Elements")]
        [SerializeField] private Text goldText;
        [SerializeField] private Text minerLevelText;
        [SerializeField] private Text minerUpgradeCostText;
        [SerializeField] private Text levelNameText;

        [Header("HUD Buttons")]
        [SerializeField] private Button minerUpgradeButton;

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
            if (minerUpgradeButton != null)
            {
                minerUpgradeButton.onClick.AddListener(OnUpgradeMinerClicked);
            }
        }

        private void Update()
        {
            UpdateHUD();
        }

        private void UpdateHUD()
        {
            if (ResourceManager.Instance == null) return;

            float currentGold = ResourceManager.Instance.CurrentGold;
            float maxGold = ResourceManager.Instance.MaxGold;
            int minerLevel = ResourceManager.Instance.MinerLevel;
            float upgradeCost = ResourceManager.Instance.GetUpgradeCost();

            if (goldText != null)
            {
                goldText.text = $"Gold: {currentGold:F0}/{maxGold:F0}";
            }

            if (minerLevelText != null)
            {
                minerLevelText.text = $"Miner Lv: {minerLevel}";
            }

            if (minerUpgradeCostText != null)
            {
                if (upgradeCost < 0)
                {
                    minerUpgradeCostText.text = "MAX";
                }
                else
                {
                    minerUpgradeCostText.text = $"{upgradeCost:F0} G";
                }
            }

            if (minerUpgradeButton != null)
            {
                minerUpgradeButton.interactable = (upgradeCost >= 0 && currentGold >= upgradeCost);
            }

            if (LevelManager.Instance != null && levelNameText != null)
            {
                levelNameText.text = LevelManager.Instance.GetCurrentLevel().levelName;
            }
        }

        public void UpdateUIForState(GameManager.GameState state)
        {
            if (hudPanel != null) hudPanel.SetActive(state == GameManager.GameState.Playing);
            if (pausePanel != null) pausePanel.SetActive(state == GameManager.GameState.Paused);
            if (gameOverPanel != null) gameOverPanel.SetActive(state == GameManager.GameState.GameOver);
            if (victoryPanel != null) victoryPanel.SetActive(state == GameManager.GameState.Victory);
        }

        private void OnUpgradeMinerClicked()
        {
            if (ResourceManager.Instance != null)
            {
                ResourceManager.Instance.UpgradeMiner();
            }
        }

        // Button action mappings
        public void OnPauseButtonPressed()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PauseGame();
            }
        }

        public void OnResumeButtonPressed()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ResumeGame();
            }
        }

        public void OnRestartButtonPressed()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RestartLevel();
            }
        }

        public void OnNextLevelButtonPressed()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.NextLevel();
            }
        }

        public void OnQuitButtonPressed()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.QuitGame();
            }
        }
    }
}
