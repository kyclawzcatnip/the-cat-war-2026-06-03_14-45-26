using UnityEngine;
using UnityEngine.UI;

namespace CatWar
{
    public class SpawnButton : MonoBehaviour
    {
        [Header("Unit Configuration")]
        [SerializeField] private UnitData unitData;

        [Header("Scene References")]
        [SerializeField] private Transform spawnPoint;

        [Header("UI Bindings")]
        [SerializeField] private Button button;
        [SerializeField] private Image iconImage;
        [SerializeField] private Image cooldownImage;
        [SerializeField] private Text costText;

        private float cooldownTimer;
        private bool isCooldownActive;

        private void Start()
        {
            if (button == null) button = GetComponent<Button>();
            
            if (button != null)
            {
                button.onClick.AddListener(OnSpawnButtonClicked);
            }

            // Initialize UI elements based on UnitData
            if (unitData != null)
            {
                if (iconImage != null)
                {
                    iconImage.sprite = (unitData.icon != null) ? unitData.icon : unitData.unitSprite;
                }
                if (costText != null)
                {
                    costText.text = $"{unitData.cost} G";
                }
            }

            if (cooldownImage != null)
            {
                cooldownImage.fillAmount = 0f;
            }
        }

        private void Update()
        {
            if (unitData == null) return;

            // Handle Cooldown overlay countdown
            if (isCooldownActive)
            {
                cooldownTimer -= Time.deltaTime;
                if (cooldownImage != null)
                {
                    cooldownImage.fillAmount = Mathf.Clamp01(cooldownTimer / unitData.spawnCooldown);
                }

                if (cooldownTimer <= 0)
                {
                    cooldownTimer = 0f;
                    isCooldownActive = false;
                    if (cooldownImage != null) cooldownImage.fillAmount = 0f;
                }
            }

            // Enable/disable spawning capability based on cost and status
            if (button != null)
            {
                bool canAfford = ResourceManager.Instance != null && ResourceManager.Instance.CanAfford(unitData.cost);
                bool isPlaying = GameManager.Instance == null || GameManager.Instance.CurrentState == GameManager.GameState.Playing;
                button.interactable = !isCooldownActive && canAfford && isPlaying;
            }
        }

        private void OnSpawnButtonClicked()
        {
            if (unitData == null) return;

            if (ResourceManager.Instance != null && ResourceManager.Instance.SpendGold(unitData.cost))
            {
                SpawnUnit();
                cooldownTimer = unitData.spawnCooldown;
                isCooldownActive = true;
            }
        }

        private void SpawnUnit()
        {
            if (unitData.prefab != null)
            {
                Vector3 spawnPos = transform.position;
                if (spawnPoint != null)
                {
                    spawnPos = spawnPoint.position;
                }

                GameObject spawnedObj = Instantiate(unitData.prefab, spawnPos, Quaternion.identity);
                Unit unit = spawnedObj.GetComponent<Unit>();
                if (unit != null)
                {
                    unit.data = unitData;
                    unit.isEnemy = false;
                }
            }
            else
            {
                Debug.LogWarning($"[SpawnButton] Prefab is missing for {unitData.unitName}!");
            }
        }
    }
}
