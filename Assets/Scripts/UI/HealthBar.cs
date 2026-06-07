using UnityEngine;
using UnityEngine.UI;

namespace CatWar
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private Vector3 offset = new Vector3(0, 1.5f, 0);

        private Transform target;

        public void SetTarget(Transform newTarget, Vector3 newOffset)
        {
            target = newTarget;
            offset = newOffset;
        }

        private void LateUpdate()
        {
            if (target != null)
            {
                transform.position = target.position + offset;
            }
        }

        public void SetMaxHealth(float maxHealth)
        {
            if (slider != null)
            {
                slider.maxValue = maxHealth;
                slider.value = maxHealth;
            }
        }

        public void SetHealth(float health)
        {
            if (slider != null)
            {
                slider.value = health;
            }
        }
    }
}
